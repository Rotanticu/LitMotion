using System;
using UnityEngine;

namespace LitMotion
{
    public abstract class LoopBasedAnimationSpec<TValue, TOptions> : DurationBasedAnimationSpec<TValue, TOptions>
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
    {
        public abstract int LoopCount { get; set; }
        public abstract DelayType DelayType { get; set; }
        public abstract LoopType LoopType { get; set; }

        public override bool IsInfinite => LoopCount < 0;

        public long GetDurationNanos()
        {
            if (IsInfinite) return long.MaxValue;
            return DelayNanos * (DelayType == DelayType.EveryLoop ? LoopCount : 1) + DurationNanos * LoopCount;
        }

        public override long GetDurationNanos<TAdapter>(TValue startValue, TValue targetValue, TValue startVelocity)
        {
            return GetDurationNanos();
        }
    }
}

    