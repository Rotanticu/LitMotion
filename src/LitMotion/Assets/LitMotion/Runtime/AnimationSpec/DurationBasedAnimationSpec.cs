using System;
using UnityEngine;

namespace LitMotion
{
    /// <summary>
    /// 基于时长的动画规范抽象基类，适用于Tween、Keyframes等有限时长动画。
    /// </summary>
    public abstract class DurationBasedAnimationSpec<TValue, TOptions> : IVectorizedAnimationSpec<TValue, TOptions>
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
    {
        public abstract MotionTimeKind TimeKind { get; set; }

        public abstract long DelayNanos { get; set; }
        public abstract long DurationNanos { get; set; }

        public abstract bool IsInfinite { get; }
        public bool IsDurationBased => true;

        public abstract TValue GetValueFromNanos<TAdapter>(long playTimeNanos, TValue startValue, TValue targetValue, TValue startVelocity)
                where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>;
        public abstract TValue GetVelocityFromNanos<TAdapter>(long playTimeNanos, TValue startValue, TValue targetValue, TValue startVelocity)
                where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>;
        public virtual long GetDurationNanos<TAdapter>(TValue startValue, TValue targetValue, TValue startVelocity)
                where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            return DurationNanos;
        }
        public abstract TValue GetEndVelocity<TAdapter>(TValue startValue, TValue targetValue, TValue startVelocity)
        where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>;
    }
} 