using System;
using LitMotion.Collections;

namespace LitMotion
{
    /// <summary>
    /// A type indicating that motion has no special options. Specify in the type argument of MotionAdapter when the option is not required.
    /// </summary>
    [Serializable]
    public struct TweenOption : ITweenOptions, IEquatable<TweenOption>
    {
        public long DurationNanos { get; set; }
        public int Loops { get; set; }
        public long DelayNanos { get; set; }
        public DelayType DelayType { get; set; }
        public LoopType LoopType { get; set; }
        public Ease Ease { get; set; }

        #if LITMOTION_COLLECTIONS_2_0_OR_NEWER
                public NativeAnimationCurve AnimationCurve { get; set; }
        #else
                public UnsafeAnimationCurve AnimationCurve { get; set; }
        #endif

        public bool Equals(TweenOption other)
        {
            return DurationNanos.Equals(other.DurationNanos)
                && DelayNanos.Equals(other.DelayNanos)
                && DelayType == other.DelayType
                && Ease == other.Ease
                && Loops == other.Loops
                && LoopType == other.LoopType
                && AnimationCurve.Equals(other.AnimationCurve);
        }

        public override bool Equals(object obj)
        {
            return obj is TweenOption other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DurationNanos, DelayNanos, DelayType, Ease, Loops, LoopType);
        }
    }
}