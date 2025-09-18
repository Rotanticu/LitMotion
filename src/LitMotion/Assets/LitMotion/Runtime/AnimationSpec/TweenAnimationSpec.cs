using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst.CompilerServices;
using LitMotion.Collections;

namespace LitMotion
{
    /// <summary>
    /// Tween animation specification that combines MotionData functionality with vectorized animation support.
    /// </summary>
    /// <typeparam name="VValue">Internal vectorized value type</typeparam>
    /// <typeparam name="TOptions">Animation options type</typeparam>
    public struct TweenAnimationSpec<VValue,TOptions> : IVectorizedAnimationSpec<VValue, TOptions>
        where VValue : unmanaged
        where TOptions : unmanaged, ITweenOptions
    {
        private unsafe AnimationState state;
        private unsafe TOptions options;
        private MotionTimeKind timeKind;


        public unsafe AnimationState* State
        {
            get
            {
                fixed (AnimationState* statePtr = &state)
                {
                    return statePtr;
                }
            }
            set => state = *value;
        }
        public unsafe TOptions* Options
        {
            get
            {
                fixed (TOptions* optionsPtr = &options)
                {
                    return optionsPtr;
                }
            }
            set => options = *value;
        }
        public MotionTimeKind TimeKind { get => timeKind; set => timeKind = value; }

        public unsafe long DelayNanos
        {
            readonly get => options.DelayNanos;
            set => options.DelayNanos = value < 0 ? 0 : value;
        }

        public long DelayMillis
        {
            readonly get => DelayNanos / AnimationConstants.MillisToNanos;
            set => DelayNanos = value * AnimationConstants.MillisToNanos;
        }

        public int DelaySeconds
        {
            readonly get => (int)(DelayNanos / AnimationConstants.SecondsToNanos);
            set => DelayNanos = value * AnimationConstants.SecondsToNanos;
        }

        public unsafe long DurationNanos
        {
            readonly get => options.DurationNanos;
            set => options.DurationNanos = value < 0 ? 0 : value;
        }

        public long DurationMillis
        {
            readonly get => DurationNanos / AnimationConstants.MillisToNanos;
            set => DurationNanos = value * AnimationConstants.MillisToNanos;
        }

        public int DurationSeconds
        {
            get => (int)(DurationNanos / AnimationConstants.SecondsToNanos);
            set => DurationNanos = value * AnimationConstants.SecondsToNanos;
        }

        public unsafe int LoopCount
        {
            readonly get => options.Loops;
            set => options.Loops = value < 0 ? -1 : value; // -1 for infinite loops
        }
        public unsafe DelayType DelayType
        {
            readonly get => options.DelayType;
            set => options.DelayType = value;
        }
        public unsafe LoopType LoopType
        {
            readonly get => options.LoopType;
            set => options.LoopType = value;
        }
        public Ease Ease
        {
            readonly get => options.Ease;
            set => options.Ease = value;
        }

#if LITMOTION_COLLECTIONS_2_0_OR_NEWER
        public NativeAnimationCurve AnimationCurve
        {
            readonly get => options.AnimationCurve;
            set => options.AnimationCurve = value;
        }
#else
        public UnsafeAnimationCurve AnimationCurve
        {
            readonly get => options.AnimationCurve;
            set => options.AnimationCurve = value;
        }
#endif

        public bool IsInfinite => LoopCount < 0;

        public readonly bool IsDurationBased => true;

        public long GetDurationNanos()
        {
            if (IsInfinite) return long.MaxValue;
            return DelayNanos * (DelayType == DelayType.EveryLoop ? LoopCount : 1) + DurationNanos * LoopCount;
        }



        public VValue GetValueFromNanos<TValue>(long playTimeNanos, VValue startValue, VValue targetValue, VValue startVelocity)
            where TValue : unmanaged
        {
            // 更新状态
            state.PrevCompletedLoops = state.CompletedLoops;
            state.PrevStatus = state.Status;
            state.playTimeNanos = playTimeNanos;
            
            // 确保时间不为负数
            playTimeNanos = math.max(playTimeNanos, 0L);

            double t;
            bool isCompleted;
            bool isDelayed;
            int completedLoops;
            int clampedCompletedLoops;

            // 处理瞬时动画（Duration <= 0）
            if (Hint.Unlikely(DurationNanos <= 0L))
            {
                if (DelayType == DelayType.FirstLoop || DelayNanos == 0L)
                {
                    isCompleted = LoopCount >= 0 && TimeSinceStart > 0L;
                    if (isCompleted)
                    {
                        t = 1.0;
                        completedLoops = LoopCount;
                    }
                    else
                    {
                        t = 0.0;
                        completedLoops = TimeSinceStart < 0L ? -1 : 0;
                    }
                    clampedCompletedLoops = GetClampedCompletedLoops(completedLoops);
                    isDelayed = TimeSinceStart < 0L;
                }
                else
                {
                    completedLoops = (int)math.floor(playTimeNanos / DelayNanos);
                    clampedCompletedLoops = GetClampedCompletedLoops(completedLoops);
                    isCompleted = LoopCount >= 0 && clampedCompletedLoops > LoopCount - 1;
                    isDelayed = !isCompleted;
                    t = isCompleted ? 1.0 : 0.0;
                }
            }
            else
            {
                // 处理正常动画
                if (DelayType == DelayType.FirstLoop)
                {
                    completedLoops = (int)math.floor(TimeSinceStart / DurationNanos);
                    clampedCompletedLoops = GetClampedCompletedLoops(completedLoops);
                    isCompleted = LoopCount >= 0 && clampedCompletedLoops > LoopCount - 1;
                    isDelayed = TimeSinceStart < 0L;

                    if (isCompleted)
                    {
                        t = 1.0;
                    }
                    else
                    {
                        var currentLoopTime = TimeSinceStart - DurationNanos * clampedCompletedLoops;
                        t = math.clamp(currentLoopTime / (double)DurationNanos, 0.0, 1.0);
                    }
                }
                else
                {
                    var currentLoopTime = math.fmod(playTimeNanos, DurationNanos + DelayNanos) - DelayNanos;
                    completedLoops = (int)math.floor(playTimeNanos / (DurationNanos + DelayNanos));
                    clampedCompletedLoops = GetClampedCompletedLoops(completedLoops);
                    isCompleted = LoopCount >= 0 && clampedCompletedLoops > LoopCount - 1;
                    isDelayed = currentLoopTime < 0L;

                    if (isCompleted)
                    {
                        t = 1.0;
                    }
                    else
                    {
                        t = math.clamp(currentLoopTime / (double)DurationNanos, 0.0, 1.0);
                    }
                }
            }

            // 更新完成循环数
            state.CompletedLoops = (ushort)clampedCompletedLoops;

            // 计算最终进度值（float 类型，用于缓动计算）
            float progress;
            switch (LoopType)
            {
                default:
                case LoopType.Restart:
                    progress = GetEasedValue((float)t);
                    break;
                case LoopType.Flip:
                    progress = GetEasedValue((float)t);
                    if ((clampedCompletedLoops + (int)t) % 2 == 1) 
                        progress = 1f - progress; // 翻转时反转进度
                    break;
                case LoopType.Incremental:
                    var incrementalProgress = GetEasedValue(1f) * clampedCompletedLoops + GetEasedValue((float)math.fmod(t, 1f));
                    progress = incrementalProgress;
                    break;
                case LoopType.Yoyo:
                    progress = (clampedCompletedLoops + (int)t) % 2 == 1
                        ? GetEasedValue((float)(1f - t))
                        : GetEasedValue((float)t);
                    break;
            }

            // 更新状态
            if (isCompleted)
            {
                state.Status = MotionStatus.Completed;
            }
            else if (isDelayed || state.playTimeNanos < 0L)
            {
                state.Status = MotionStatus.Delayed;
            }
            else
            {
                state.Status = MotionStatus.Playing;
            }

            // 使用计算出的进度值进行向量插值
            return Lerp(startValue, targetValue, progress);
        }

        // 通用的线性插值方法，支持常见的 Unity 类型
        private VValue Lerp(VValue start, VValue end, float t)
        {
            // 使用泛型约束和类型检查来实现类型安全的插值
            if (typeof(VValue) == typeof(float))
            {
                var startFloat = Unsafe.As<VValue, float>(ref start);
                var endFloat = Unsafe.As<VValue, float>(ref end);
                var result = math.lerp(startFloat, endFloat, t);
                return Unsafe.As<float, VValue>(ref result);
            }
            else if (typeof(VValue) == typeof(float2))
            {
                var startFloat2 = Unsafe.As<VValue, float2>(ref start);
                var endFloat2 = Unsafe.As<VValue, float2>(ref end);
                var result = math.lerp(startFloat2, endFloat2, t);
                return Unsafe.As<float2, VValue>(ref result);
            }
            else if (typeof(VValue) == typeof(float3))
            {
                var startFloat3 = Unsafe.As<VValue, float3>(ref start);
                var endFloat3 = Unsafe.As<VValue, float3>(ref end);
                var result = math.lerp(startFloat3, endFloat3, t);
                return Unsafe.As<float3, VValue>(ref result);
            }
            else if (typeof(VValue) == typeof(float4))
            {
                var startFloat4 = Unsafe.As<VValue, float4>(ref start);
                var endFloat4 = Unsafe.As<VValue, float4>(ref end);
                var result = math.lerp(startFloat4, endFloat4, t);
                return Unsafe.As<float4, VValue>(ref result);
            }
            else if (typeof(VValue) == typeof(quaternion))
            {
                var startQuat = Unsafe.As<VValue, quaternion>(ref start);
                var endQuat = Unsafe.As<VValue, quaternion>(ref end);
                var result = math.nlerp(startQuat, endQuat, t);
                return Unsafe.As<quaternion, VValue>(ref result);
            }
            else
            {
                // 对于不支持的类型，返回默认值
                // 在实际使用中，应该为每种 VValue 类型创建专门的 TweenAnimationSpec 实现
                return default;
            }
        }

        public VValue GetVelocityFromNanos<TValue>(long playTimeNanos, VValue startValue, VValue targetValue, VValue startVelocity)
            where TValue : unmanaged
        {
            throw new NotImplementedException();
        }

        public VValue GetEndVelocity<TValue>(VValue startValue, VValue targetValue, VValue startVelocity)
            where TValue : unmanaged
        {
            throw new NotImplementedException();
        }

        public long GetDurationNanos<TValue>(VValue startValue, VValue targetValue, VValue startVelocity)
            where TValue : unmanaged
        {
            return GetDurationNanos();
        }

        public VValue ConvertToVector<TValue>(TValue value) where TValue : unmanaged
        {
            // 对于TweenAnimationSpec，我们需要根据VValue类型来实现转换
            // 这是一个抽象方法，具体的转换逻辑应该在具体的实现中
            throw new NotImplementedException("ConvertToVector should be implemented in concrete TweenAnimationSpec implementations");
        }

        public TValue ConvertFromVector<TValue>(VValue vector) where TValue : unmanaged
        {
            // 对于TweenAnimationSpec，我们需要根据VValue类型来实现转换
            // 这是一个抽象方法，具体的转换逻辑应该在具体的实现中
            throw new NotImplementedException("ConvertFromVector should be implemented in concrete TweenAnimationSpec implementations");
        }

        public unsafe long TimeSinceStart => state.playTimeNanos - options.DelayNanos;

        // MotionData interface - Complete method
        public unsafe void Complete(out float progress)
        {
            state.Status = MotionStatus.Completed;
            state.playTimeNanos = DurationNanos;
            state.CompletedLoops = (ushort)LoopCount;

            progress = GetEasedValue(LoopType switch
            {
                LoopType.Restart => 1f,
                LoopType.Flip or LoopType.Yoyo => LoopCount % 2 == 0 ? 0f : 1f,
                LoopType.Incremental => LoopCount,
                _ => 1f
            });
        }
        unsafe int GetClampedCompletedLoops(int completedLoops)
        {
            return LoopCount < 0
                ? math.max(0, completedLoops)
                : math.clamp(completedLoops, 0, LoopCount);
        }

        // 获取缓动后的标量值
        private float GetEasedValue(float t)
        {
            return Ease switch
            {
                Ease.CustomAnimationCurve => AnimationCurve.Evaluate(t),
                _ => EaseUtility.Evaluate(t, Ease)
            };
        }
        
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public void Complete<TAdapter>(out TValue result)
        //     where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        // {
        //     Core.Complete(out var progress);

        //     result = default(TAdapter).Evaluate(
        //         ref StartValue,
        //         ref EndValue,
        //         ref Options,
        //         new()
        //         {
        //             Progress = progress,
        //             Time = Core.State.playTimeNanos,
        //         }
        //     );
        // }
    }
}

    