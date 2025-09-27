using System;
using UnityEngine;

namespace LitMotion
{
    /// <summary>
    /// Spring animation specification that implements physics-based elastic animations.
    /// </summary>
    /// <typeparam name="VValue">Internal vectorized value type</typeparam>
    /// <typeparam name="TOptions">Animation options type</typeparam>
    public class SpringAnimationSpec<VValue, TOptions> : IVectorizedAnimationSpec<VValue, TOptions>
        where VValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
    {
        // public long _delayNanos;
        // public long DelayNanos
        // {
        //     get => _delayNanos;
        //     set => _delayNanos = value < 0 ? 0 : value;
        // }

        // public long DelayMillis
        // {
        //     get => _delayNanos / AnimationConstants.MillisToNanos;
        //     set => DelayNanos = value * AnimationConstants.MillisToNanos;
        // }

        // public int DelaySeconds
        // {
        //     get => (int)(_delayNanos / AnimationConstants.SecondsToNanos);
        //     set => DelayNanos = value * AnimationConstants.SecondsToNanos;
        // }
        private bool _isInfinite;
        public bool IsInfinite => _isInfinite;

        public bool IsDurationBased => false;

        public unsafe AnimationState* State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public unsafe TOptions* Options { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public MotionTimeKind TimeKind { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //public AnimationState State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Initialize(TOptions options)
        {
            throw new NotImplementedException();
        }
        public VValue GetValueFromNanos<TValue>(long playTimeNanos, VValue startValue, VValue targetValue, VValue startVelocity) 
            where TValue : unmanaged
        {
            throw new NotImplementedException();
        }

        public VValue GetVelocityFromNanos<TValue>(long playTimeNanos, VValue startValue, VValue targetValue, VValue startVelocity) 
            where TValue : unmanaged
        {
            throw new NotImplementedException();
        }

        public long GetDurationNanos<TValue>(VValue startValue, VValue targetValue, VValue startVelocity) 
            where TValue : unmanaged
        {
            throw new NotImplementedException();
        }

        public VValue GetEndVelocity<TValue>(VValue startValue, VValue targetValue, VValue startVelocity) 
            where TValue : unmanaged
        {
            throw new NotImplementedException();
        }
    }
} 