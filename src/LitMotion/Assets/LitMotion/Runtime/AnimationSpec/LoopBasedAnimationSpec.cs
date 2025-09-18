using System;
using UnityEngine;

namespace LitMotion
{
    // /// <summary>
    // /// Abstract base class for loop-based animation specifications that support repeating animations.
    // /// </summary>
    // /// <typeparam name="VValue">Internal vectorized value type</typeparam>
    // /// <typeparam name="TOptions">Animation options type</typeparam>
    // public abstract class LoopBasedAnimationSpec<VValue, TOptions> : DurationBasedAnimationSpec<VValue, TOptions>
    //     where VValue : unmanaged
    //     where TOptions : unmanaged, IMotionOptions
    // {
    //     public abstract int LoopCount { get; set; }
    //     public abstract DelayType DelayType { get; set; }
    //     public abstract LoopType LoopType { get; set; }

    //     public override bool IsInfinite => LoopCount < 0;

    //     public long GetDurationNanos()
    //     {
    //         if (IsInfinite) return long.MaxValue;
    //         return DelayNanos * (DelayType == DelayType.EveryLoop ? LoopCount : 1) + DurationNanos * LoopCount;
    //     }

    //     public override long GetDurationNanos<TAdapter>(VValue startValue, VValue targetValue, VValue startVelocity)
    //     {
    //         return GetDurationNanos();
    //     }
    // }
}

    