using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace LitMotion
{
    /// <summary>
    /// A job that updates the status of the motion data and outputs the current value.
    /// </summary>
    /// <typeparam name="TValue">The type of value to animate (user-facing)</typeparam>
    /// <typeparam name="VValue">The type of vectorized value for internal processing</typeparam>
    /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
    /// <typeparam name="TAnimationSpec">The type of vectorized animation specification</typeparam>
    [BurstCompile]
    public unsafe struct MotionUpdateJob<TValue, VValue, TOptions, TAnimationSpec> : IJobParallelFor
        where TValue : unmanaged
        where VValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
        where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
    {
        [NativeDisableUnsafePtrRestriction] internal TargetBasedAnimation<TValue, VValue, TOptions, TAnimationSpec>* DataPtr;
        [ReadOnly] public double DeltaTime;
        [ReadOnly] public double UnscaledDeltaTime;
        [ReadOnly] public double RealDeltaTime;

        [WriteOnly] public NativeList<int>.ParallelWriter CompletedIndexList;
        [WriteOnly] public NativeArray<TValue> Output;

        [WriteOnly] public NativeArray<VValue> OutputVelocity;

        public void Execute([AssumeRange(0, int.MaxValue)] int index)
        {
            var ptr = DataPtr + index;
            var state = ptr->Core.State;

            if (Hint.Likely(state->Status is MotionStatus.Scheduled or MotionStatus.Delayed or MotionStatus.Playing) ||
                Hint.Unlikely(state->IsPreserved && state->Status is MotionStatus.Completed))
            {
                if (Hint.Unlikely(state->IsInSequence)) return;

                var deltaTime = ptr->Core.TimeKind switch
                {
                    MotionTimeKind.Time => DeltaTime,
                    MotionTimeKind.UnscaledTime => UnscaledDeltaTime,
                    MotionTimeKind.Realtime => RealDeltaTime,
                    _ => default
                };

                var time = state->playTimeNanos + (long)(deltaTime * state->PlaybackSpeed * 1_000_000_000); // Convert to nanoseconds
                ptr->GetValueFromNanos(time,out var result);
                Output[index] = result;
                ptr->GetVelocityVectorFromNanos(time,out var velocity);
                OutputVelocity[index] = velocity;
            }
            else if ((!state->IsPreserved && state->Status is MotionStatus.Completed) || state->Status is MotionStatus.Canceled)
            {
                CompletedIndexList.AddNoResize(index);
                state->Status = MotionStatus.Disposed;
            }
        }
    }
}