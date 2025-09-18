using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace LitMotion
{
    /// <summary>
    /// Float tween animation specification that implements vectorized animation support for float values.
    /// This replaces the FloatMotionAdapter functionality.
    /// </summary>
    public struct FloatingTweenAnimationSpec : IVectorizedAnimationSpec<float, TweenOption>
    {
        private TweenAnimationSpec<float,TweenOption> core;

        public unsafe AnimationState* State
        {
            get => core.State;
            set => core.State = value;
        }

        public unsafe TweenOption* Options
        {
            get => core.Options;
            set => core.Options = value;
        }

        public MotionTimeKind TimeKind
        {
            get => core.TimeKind;
            set => core.TimeKind = value;
        }

        public unsafe long DelayNanos
        {
            get => core.DelayNanos;
            set => core.DelayNanos = value;
        }

        public long DelayMillis
        {
            get => core.DelayMillis;
            set => core.DelayMillis = value;
        }

        public int DelaySeconds
        {
            get => core.DelaySeconds;
            set => core.DelaySeconds = value;
        }

        public unsafe long DurationNanos
        {
            get => core.DurationNanos;
            set => core.DurationNanos = value;
        }

        public long DurationMillis
        {
            get => core.DurationMillis;
            set => core.DurationMillis = value;
        }

        public int DurationSeconds
        {
            get => core.DurationSeconds;
            set => core.DurationSeconds = value;
        }

        public unsafe int LoopCount
        {
            get => core.LoopCount;
            set => core.LoopCount = value;
        }

        public unsafe DelayType DelayType
        {
            get => core.DelayType;
            set => core.DelayType = value;
        }

        public unsafe LoopType LoopType
        {
            get => core.LoopType;
            set => core.LoopType = value;
        }

        public bool IsInfinite => core.IsInfinite;

        public bool IsDurationBased => core.IsDurationBased;

        public long GetDurationNanos()
        {
            return core.GetDurationNanos();
        }

        // 重写动画计算方法
        public float GetValueFromNanos<TValue>(long playTimeNanos, float startValue, float targetValue, float startVelocity)
            where TValue : unmanaged
        {
            // 处理延迟
            if (playTimeNanos < DelayNanos)
            {
                return startValue;
            }

            var adjustedTime = playTimeNanos - DelayNanos;
            var durationNanos = DurationNanos;
            
            if (durationNanos <= 0)
            {
                return targetValue;
            }

            // 计算进度
            var progress = (float)(adjustedTime / (double)durationNanos);
            
            // 应用缓动
            var easedProgress = GetEasedValue(progress);
            
            // 线性插值
            return Mathf.LerpUnclamped(startValue, targetValue, easedProgress);
        }

        public float GetVelocityFromNanos<TValue>(long playTimeNanos, float startValue, float targetValue, float startVelocity)
            where TValue : unmanaged
        {
            // 处理延迟
            if (playTimeNanos < DelayNanos)
            {
                return 0f;
            }

            var adjustedTime = playTimeNanos - DelayNanos;
            var durationNanos = DurationNanos;
            
            if (durationNanos <= 0)
            {
                return 0f;
            }

            // 计算进度
            var progress = (float)(adjustedTime / (double)durationNanos);
            
            // 应用缓动导数
            var easedProgress = GetEasedValue(progress);
            var velocity = (targetValue - startValue) / durationNanos * AnimationConstants.NanosToSeconds;
            
            return velocity;
        }

        public float GetEndVelocity<TValue>(float startValue, float targetValue, float startVelocity)
            where TValue : unmanaged
        {
            return 0f; // Tween动画结束时速度为0
        }

        public long GetDurationNanos<TValue>(float startValue, float targetValue, float startVelocity)
            where TValue : unmanaged
        {
            return DurationNanos;
        }

        public unsafe long TimeSinceStart => core.TimeSinceStart;

        public unsafe void Complete(out float progress)
        {
            core.Complete(out progress);
        }

        private unsafe float GetEasedValue(float value)
        {
            // 从core获取缓动函数并应用
            // 这里需要根据Options中的缓动设置来计算
            return value; // 暂时返回线性值
        }
    }
} 