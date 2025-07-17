using System;
using UnityEngine;

namespace LitMotion
{
    public interface IVectorizedAnimationSpec<TValue, TOptions>
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
    {
        TValue GetValueFromNanos<TAdapter>(long playTimeNanos, TValue startValue, TValue targetValue, TValue startVelocity)
        where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>;
        TValue GetVelocityFromNanos<TAdapter>(long playTimeNanos, TValue startValue, TValue targetValue, TValue startVelocity)
        where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>;
        long GetDurationNanos<TAdapter>(TValue startValue, TValue targetValue, TValue startVelocity)
        where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>;
        TValue GetEndVelocity<TAdapter>(TValue startValue, TValue targetValue, TValue startVelocity)
        where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>;

    }
}