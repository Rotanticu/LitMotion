#if DEVELOPMENT_BUILD || UNITY_EDITOR
#define LITMOTION_DEBUG
#endif

using System;
using System.Runtime.CompilerServices;
using LitMotion.Collections;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace LitMotion
{
    internal record MotionDebugInfo
    {
        public object StartValue;
        public object EndValue;
        public object Options;
    }

    internal interface IMotionStorage
    {
        bool IsActive(MotionHandle handle);
        bool IsPlaying(MotionHandle handle);
        bool TryCancel(MotionHandle handle, bool checkIsInSequence = true);
        bool TryComplete(MotionHandle handle, bool checkIsInSequence = true);
        void Cancel(MotionHandle handle, bool checkIsInSequence = true);
        void Complete(MotionHandle handle, bool checkIsInSequence = true);
        void SetTime(MotionHandle handle, double time, bool checkIsInSequence = true);
        ref MotionData GetDataRef(MotionHandle handle, bool checkIsInSequence = true);
        ref MotionData<ValueType, OptionsType> GetTypeDataRef<ValueType, OptionsType>(MotionHandle handle, bool checkIsInSequence = true)
            where ValueType : unmanaged
            where OptionsType : unmanaged, IMotionOptions;
        ref ManagedMotionData GetManagedDataRef(MotionHandle handle, bool checkIsInSequence = true);
        void AddToSequence(MotionHandle handle, out double motionDuration);
        MotionDebugInfo GetDebugInfo(MotionHandle handle);
        void Reset();
    }

    internal sealed class MotionStorage<TValue, VValue, TOptions, TAnimationSpec> : IMotionStorage
        where TValue : unmanaged
        where VValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
        where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
    {
        const int InitialCapacity = 32;

        public int Id { get; }
        public int Count => tail;

        SparseSetCore sparseSetCore = new(InitialCapacity);
        SparseIndex[] sparseIndexLookup = new SparseIndex[InitialCapacity];
        TargetBasedAnimation<TValue, VValue, TOptions, TAnimationSpec>[] unmanagedDataArray = new TargetBasedAnimation<TValue, VValue, TOptions, TAnimationSpec>[InitialCapacity];
        ManagedMotionData[] managedDataArray = new ManagedMotionData[InitialCapacity];
        AllocatorHelper<RewindableAllocator> allocator;
        int tail;

        public MotionStorage(int id)
        {
            Id = id;
            allocator = RewindableAllocatorFactory.CreateAllocator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<TargetBasedAnimation<TValue, VValue, TOptions, TAnimationSpec>> GetDataSpan()
        {
            return unmanagedDataArray.AsSpan();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<ManagedMotionData> GetManagedDataSpan()
        {
            return managedDataArray.AsSpan();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureCapacity(int minimumCapacity)
        {
            sparseSetCore.EnsureCapacity(minimumCapacity);
            ArrayHelper.EnsureCapacity(ref sparseIndexLookup, minimumCapacity);
            ArrayHelper.EnsureCapacity(ref unmanagedDataArray, minimumCapacity);
            ArrayHelper.EnsureCapacity(ref managedDataArray, minimumCapacity);
        }

        public unsafe MotionHandle Create<TweenOptions,TTweenAnimationSpec>(ref MotionBuilder<TValue, VValue, TweenOptions, TTweenAnimationSpec> builder)
            where TweenOptions : unmanaged, ITweenOptions
            where TTweenAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TweenOptions>
        {
            EnsureCapacity(tail + 1);
            var buffer = builder.buffer;

            ref var dataRef = ref unmanagedDataArray[tail];
            ref var managedDataRef = ref managedDataArray[tail];
            
            // 初始化非托管数据
            var options = Unsafe.As<TweenOptions, TOptions>(ref buffer.Options);
            dataRef.Initialize(options, buffer.StartValue, buffer.EndValue);

            // 初始化托管数据
            dataRef.GetValueFromNanos(0L,out var startValue);
            managedDataRef.Initialize(buffer,startValue);
            var sparseIndex = sparseSetCore.Alloc(tail);
            sparseIndexLookup[tail] = sparseIndex;

            tail++;
            return new MotionHandle()
            {
                Index = sparseIndex.Index,
                Version = sparseIndex.Version,
                StorageId = Id
            };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void RemoveAt(int denseIndex)
        {
            tail--;

            // swap elements
            unmanagedDataArray[denseIndex] = unmanagedDataArray[tail];
            unmanagedDataArray[tail] = default;
            managedDataArray[denseIndex] = managedDataArray[tail];
            managedDataArray[tail] = default;

            // swap sparse index
            var prevSparseIndex = sparseIndexLookup[denseIndex];
            var currentSparseIndex = sparseIndexLookup[denseIndex] = sparseIndexLookup[tail];
            sparseIndexLookup[tail] = default;

            // update slot
            if (currentSparseIndex.Version != 0)
            {
                ref var slot = ref sparseSetCore.GetSlotRefUnchecked(currentSparseIndex.Index);
                slot.DenseIndex = denseIndex;
            }

            // free slot
            if (prevSparseIndex.Version != 0)
            {
                sparseSetCore.Free(prevSparseIndex);
            }
        }

        public void RemoveAll(NativeList<int> denseIndexList)
        {
            var list = new NativeArray<SparseIndex>(denseIndexList.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = sparseIndexLookup[denseIndexList[i]];
            }

            for (int i = 0; i < list.Length; i++)
            {
                RemoveAt(sparseSetCore.GetSlotRefUnchecked(list[i].Index).DenseIndex);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool IsActive(MotionHandle handle)
        {
            ref var slot = ref sparseSetCore.GetSlotRefUnchecked(handle.Index);
            if (IsDenseIndexOutOfRange(slot.DenseIndex)) return false;
            if (IsInvalidVersion(slot.Version, handle)) return false;

            var state = unmanagedDataArray[slot.DenseIndex].Core.State;
            return state->Status is MotionStatus.Scheduled or MotionStatus.Delayed or MotionStatus.Playing ||
                (state->Status is MotionStatus.Completed && state->IsPreserved);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool IsPlaying(MotionHandle handle)
        {
            ref var slot = ref sparseSetCore.GetSlotRefUnchecked(handle.Index);
            if (IsDenseIndexOutOfRange(slot.DenseIndex)) return false;
            if (IsInvalidVersion(slot.Version, handle)) return false;

            var state = unmanagedDataArray[slot.DenseIndex].Core.State;
            return state->Status is MotionStatus.Scheduled or MotionStatus.Delayed or MotionStatus.Playing;
        }

        public bool TryCancel(MotionHandle handle, bool checkIsInSequence = true)
        {
            return TryCancelCore(handle, checkIsInSequence) == 0;
        }

        public void Cancel(MotionHandle handle, bool checkIsInSequence = true)
        {
            switch (TryCancelCore(handle, checkIsInSequence))
            {
                case 1:
                    Error.MotionNotExists();
                    return;
                case 2:
                    Error.MotionHasBeenCanceledOrCompleted();
                    return;
                case 3:
                    Error.MotionIsInSequence();
                    return;
            }
        }

        unsafe int TryCancelCore(MotionHandle handle, bool checkIsInSequence)
        {
            ref var slot = ref sparseSetCore.GetSlotRefUnchecked(handle.Index);
            var denseIndex = slot.DenseIndex;
            if (IsDenseIndexOutOfRange(denseIndex) || IsInvalidVersion(slot.Version, handle))
            {
                return 1;
            }

            ref var dataRef = ref unmanagedDataArray[denseIndex];
            var state = dataRef.Core.State;

            if (state->Status is MotionStatus.None or MotionStatus.Canceled ||
                (state->Status is MotionStatus.Completed && !state->IsPreserved))
            {
                return 2;
            }

            if (checkIsInSequence && state->IsInSequence)
            {
                return 3;
            }

            state->Status = MotionStatus.Canceled;

            ref var managedData = ref managedDataArray[denseIndex];
            managedData.InvokeOnCancel();

            return 0;
        }

        public bool TryComplete(MotionHandle handle, bool checkIsInSequence = true)
        {
            return TryCompleteCore(handle, checkIsInSequence) == 0;
        }

        public void Complete(MotionHandle handle, bool checkIsInSequence = true)
        {
            switch (TryCompleteCore(handle, checkIsInSequence))
            {
                case 1:
                    Error.MotionNotExists();
                    return;
                case 2:
                    Error.MotionHasBeenCanceledOrCompleted();
                    return;
                case 3:
                    Error.MotionIsInSequence();
                    return;
                case 4:
                    throw new InvalidOperationException("Complete was ignored because it is not possible to complete a motion that loops infinitely. If you want to end the motion, call Cancel() instead.");
            }
        }

        unsafe int TryCompleteCore(MotionHandle handle, bool checkIsInSequence)
        {
            ref var slot = ref sparseSetCore.GetSlotRefUnchecked(handle.Index);

            if (IsDenseIndexOutOfRange(slot.DenseIndex) || IsInvalidVersion(slot.Version, handle))
            {
                return 1;
            }

            ref var dataRef = ref unmanagedDataArray[slot.DenseIndex];
            var state = dataRef.Core.State;

            if (state->Status is MotionStatus.None or MotionStatus.Canceled or MotionStatus.Completed)
            {
                return 2;
            }

            if (checkIsInSequence && state->IsInSequence)
            {
                return 3;
            }

            if (dataRef.IsInfinite)
            {
                return 4;
            }

            dataRef.Complete(out var result);

            ref var managedData = ref managedDataArray[slot.DenseIndex];
            try
            {
                managedData.UpdateUnsafe(result);
            }
            catch (Exception ex)
            {
                MotionDispatcher.GetUnhandledExceptionHandler()?.Invoke(ex);
            }

            if (state->WasLoopCompleted)
            {
                managedData.InvokeOnLoopComplete(state->CompletedLoops);
            }

            managedData.InvokeOnComplete();

            return 0;
        }

        public unsafe void SetTime(MotionHandle handle, double time, bool checkIsInSequence = true)
        {
            ref var slot = ref sparseSetCore.GetSlotRefUnchecked(handle.Index);

            var denseIndex = slot.DenseIndex;
            if (IsDenseIndexOutOfRange(denseIndex)) Error.MotionNotExists();

            var version = slot.Version;
            if (version <= 0 || version != handle.Version) Error.MotionNotExists();

            fixed (TargetBasedAnimation<TValue, VValue, TOptions, TAnimationSpec>* arrayPtr = unmanagedDataArray)
            {
                var dataPtr = arrayPtr + denseIndex;
                var state = dataPtr->Core.State;

                if (checkIsInSequence && state->IsInSequence) Error.MotionIsInSequence();

                dataPtr->Update<TAdapter>(time, time - state.Time, out var result);

                var status = state->Status;
                ref var managedData = ref managedDataArray[denseIndex];

                if (status is MotionStatus.Playing or MotionStatus.Completed || (status == MotionStatus.Delayed && !managedData.SkipValuesDuringDelay))
                {
                    try
                    {
                        managedData.UpdateUnsafe(result);
                    }
                    catch (Exception ex)
                    {
                        MotionDispatcher.GetUnhandledExceptionHandler()?.Invoke(ex);
                        if (managedData.CancelOnError)
                        {
                            state->Status = MotionStatus.Canceled;
                            managedData.OnCancelAction?.Invoke();
                            return;
                        }
                    }

                    if (state->WasLoopCompleted)
                    {
                        managedData.InvokeOnLoopComplete(state->CompletedLoops);
                    }

                    if (status is MotionStatus.Completed && state->WasStatusChanged)
                    {
                        managedData.InvokeOnComplete();
                    }
                }
            }
        }

        public unsafe void AddToSequence(MotionHandle handle, out double motionDuration)
        {
            ref var slot = ref GetSlotWithVarify(handle, true);
            ref var dataRef = ref unmanagedDataArray[slot.DenseIndex];

            if (dataRef.Core.State->Status is not MotionStatus.Scheduled)
            {
                throw new ArgumentException("Cannot add a running motion to a sequence.");
            }

            motionDuration = handle.TotalDuration;
            //if (double.IsInfinity(motionDuration))
            if (handle.IsInfinite)
            {
                throw new ArgumentException("Cannot add an infinitely looping motion to a sequence.");
            }

            dataRef.Core.State->IsPreserved = true;
            dataRef.Core.State->IsInSequence = true;
        }

        public ref ManagedMotionData GetManagedDataRef(MotionHandle handle, bool checkIsInSequence = true)
        {
            ref var slot = ref GetSlotWithVarify(handle, checkIsInSequence);
            return ref managedDataArray[slot.DenseIndex];
        }

        public ref MotionData GetDataRef(MotionHandle handle, bool checkIsInSequence = true)
        {
            ref var slot = ref GetSlotWithVarify(handle, checkIsInSequence);
            return ref UnsafeUtility.As<MotionData<TValue, TOptions>, MotionData>(ref unmanagedDataArray[slot.DenseIndex]);
        }

        public ref MotionData<ValueType, OptionsType> GetTypeDataRef<ValueType, OptionsType>(MotionHandle handle, bool checkIsInSequence = true)
            where ValueType : unmanaged
            where OptionsType : unmanaged, IMotionOptions
        {
            ref var slot = ref GetSlotWithVarify(handle, checkIsInSequence);
            return ref UnsafeUtility.As<MotionData<TValue, TOptions>, MotionData<ValueType, OptionsType>>(ref unmanagedDataArray[slot.DenseIndex]);
        }

        public unsafe MotionDebugInfo GetDebugInfo(MotionHandle handle)
        {
            ref var slot = ref GetSlotWithVarify(handle, false);
            ref var dataRef = ref unmanagedDataArray[slot.DenseIndex];

            return new()
            {
                StartValue = dataRef.StartValue,
                EndValue = dataRef.TargetValue,
                Options = UnsafeUtility.AsRef<TOptions>(dataRef.Core.Options)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe ref SparseSetCore.Slot GetSlotWithVarify(MotionHandle handle, bool checkIsInSequence = true)
        {
            ref var slot = ref sparseSetCore.GetSlotRefUnchecked(handle.Index);
            if (IsDenseIndexOutOfRange(slot.DenseIndex)) Error.MotionNotExists();

            ref var dataRef = ref unmanagedDataArray[slot.DenseIndex];

            if (IsInvalidVersion(slot.Version, handle) || dataRef.Core.State->Status == MotionStatus.None)
            {
                Error.MotionNotExists();
            }

            if (checkIsInSequence && dataRef.Core.State->IsInSequence)
            {
                Error.MotionIsInSequence();
            }

            return ref slot;
        }

        public void Reset()
        {
            sparseSetCore.Reset();
            sparseIndexLookup.AsSpan().Clear();
            unmanagedDataArray.AsSpan().Clear();
            managedDataArray.AsSpan().Clear();
            tail = 0;
            allocator.Allocator.Rewind();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsDenseIndexOutOfRange(int denseIndex)
        {
            return denseIndex < 0 || denseIndex >= tail;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool IsInvalidVersion(int version, MotionHandle handle)
        {
            return version <= 0 || version != handle.Version;
        }
    }
}