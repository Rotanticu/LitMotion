using System;
using LitMotion.Collections;

namespace LitMotion
{
    /// <summary>
    /// Options for integer type motion.
    /// </summary>
    [Serializable]
    public struct IntegerOptions : ITweenOptions, IEquatable<IntegerOptions>
    {
        private TweenOption tweenOptions;
        
        // IntegerOptions特有的属性
        public RoundingMode RoundingMode { get; set; }

        // ITweenOptions接口实现 - 委托给tweenOptions
        public long DurationNanos 
        { 
            get => tweenOptions.DurationNanos; 
            set => tweenOptions.DurationNanos = value; 
        }
        
        public int Loops 
        { 
            get => tweenOptions.Loops; 
            set => tweenOptions.Loops = value; 
        }
        
        public long DelayNanos 
        { 
            get => tweenOptions.DelayNanos; 
            set => tweenOptions.DelayNanos = value; 
        }
        
        public DelayType DelayType 
        { 
            get => tweenOptions.DelayType; 
            set => tweenOptions.DelayType = value; 
        }
        
        public LoopType LoopType 
        { 
            get => tweenOptions.LoopType; 
            set => tweenOptions.LoopType = value; 
        }
        
        public Ease Ease 
        { 
            get => tweenOptions.Ease; 
            set => tweenOptions.Ease = value; 
        }

        #if LITMOTION_COLLECTIONS_2_0_OR_NEWER
                public NativeAnimationCurve AnimationCurve 
                { 
                    get => tweenOptions.AnimationCurve; 
                    set => tweenOptions.AnimationCurve = value; 
                }
        #else
                public UnsafeAnimationCurve AnimationCurve 
                { 
                    get => tweenOptions.AnimationCurve; 
                    set => tweenOptions.AnimationCurve = value; 
                }
        #endif

        public readonly bool Equals(IntegerOptions other)
        {
            return tweenOptions.Equals(other.tweenOptions) && RoundingMode == other.RoundingMode;
        }

        public override readonly bool Equals(object obj)
        {
            if (obj is IntegerOptions integerOptions) return Equals(integerOptions);
            return false;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(tweenOptions, RoundingMode);
        }
    }

    /// <summary>
    /// Specifies the rounding format for values after the decimal point.
    /// </summary>
    public enum RoundingMode : byte
    {
        ToEven,
        AwayFromZero,
        ToZero,
        ToPositiveInfinity,
        ToNegativeInfinity
    }
}