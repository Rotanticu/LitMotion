using System;
using UnityEngine;

namespace LitMotion
{
    // /// <summary>
    // /// Abstract base class for duration-based animation specifications, suitable for Tween, Keyframes and other finite duration animations.
    // /// </summary>
    // /// <typeparam name="VValue">Internal vectorized value type</typeparam>
    // /// <typeparam name="TOptions">Animation options type</typeparam>
    // public abstract class DurationBasedAnimationSpec<VValue, TOptions> : IVectorizedAnimationSpec<VValue, TOptions>
    //     where VValue : unmanaged
    //     where TOptions : unmanaged, IMotionOptions
    // {
    //     public abstract long DelayNanos { get; set; }
    //     public abstract long DurationNanos { get; set; }

    //     public abstract bool IsInfinite { get; }
    //     public bool IsDurationBased => true;

    //     public abstract unsafe AnimationState* State { get; set; }
    //     public abstract unsafe TOptions* Options { get; set; }
    //     public abstract MotionTimeKind TimeKind { get; set; }

    //     public abstract void Initialize(TOptions options);

    //     public abstract VValue GetValueFromNanos<TValue, TAdapter>(long playTimeNanos, VValue startValue, VValue targetValue, VValue startVelocity)
    //             where TValue : unmanaged
    //             where TAdapter : unmanaged, IMotionAdapter<TValue, VValue, TOptions>;
    //     public abstract VValue GetVelocityFromNanos<TValue, TAdapter>(long playTimeNanos, VValue startValue, VValue targetValue, VValue startVelocity)
    //             where TValue : unmanaged
    //             where TAdapter : unmanaged, IMotionAdapter<TValue, VValue, TOptions>;
    //     public virtual long GetDurationNanos<TValue, TAdapter>(VValue startValue, VValue targetValue, VValue startVelocity)
    //             where TValue : unmanaged
    //             where TAdapter : unmanaged, IMotionAdapter<TValue, VValue, TOptions>
    //     {
    //         return DurationNanos;
    //     }
    //     public abstract VValue GetEndVelocity<TValue, TAdapter>(VValue startValue, VValue targetValue, VValue startVelocity)
    //     where TValue : unmanaged
    //     where TAdapter : unmanaged, IMotionAdapter<TValue, VValue, TOptions>;
    // }
} 