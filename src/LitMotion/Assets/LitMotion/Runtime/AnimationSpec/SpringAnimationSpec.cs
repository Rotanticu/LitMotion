using System;
using UnityEngine;

namespace LitMotion
{
    /// <summary>
    /// 弹簧动画规范，实现基于物理的弹性动画。
    /// </summary>
    /// <typeparam name="T">动画值类型</typeparam>
    public class SpringAnimationSpec<TValue, TOptions> : IVectorizedAnimationSpec<TValue, TOptions>
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
    {
        private MotionTimeKind timeKind;
        public MotionTimeKind TimeKind
        {
            get => timeKind;
            set => timeKind = value;
        }

        public long _delayNanos;
        public long DelayNanos
        {
            get => _delayNanos;
            set => _delayNanos = value < 0 ? 0 : value;
        }

        public long DelayMillis
        {
            get => _delayNanos / AnimationConstants.MillisToNanos;
            set => DelayNanos = value * AnimationConstants.MillisToNanos;
        }

        public int DelaySeconds
        {
            get => (int)(_delayNanos / AnimationConstants.SecendsToNanos);
            set => DelayNanos = value * AnimationConstants.SecendsToNanos;
        }
        private bool _isInfinite;
        public bool IsInfinite => _isInfinite;

        public bool IsDurationBased => false;

        public TValue GetValueFromNanos<TAdapter>(long playTimeNanos, TValue startValue, TValue targetValue, TValue startVelocity) where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            throw new NotImplementedException();
        }

        public TValue GetVelocityFromNanos<TAdapter>(long playTimeNanos, TValue startValue, TValue targetValue, TValue startVelocity) where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            throw new NotImplementedException();
        }

        public long GetDurationNanos<TAdapter>(TValue startValue, TValue targetValue, TValue startVelocity) where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            throw new NotImplementedException();
        }

        public TValue GetEndVelocity<TAdapter>(TValue startValue, TValue targetValue, TValue startVelocity) where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            throw new NotImplementedException();
        }
    }
} 