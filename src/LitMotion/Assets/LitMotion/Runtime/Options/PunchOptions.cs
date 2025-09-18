using System;
using LitMotion.Collections;

namespace LitMotion
{
    /// <summary>
    /// Options for punch motion.
    /// </summary>
    [Serializable]
    public struct PunchOptions : ITweenOptions, IEquatable<PunchOptions>
    {
        private TweenOption tweenOptions;
        
        // PunchOptions特有的属性
        public int Frequency { get; set; }
        public float DampingRatio { get; set; }

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

        public static PunchOptions Default
        {
            get
            {
                return new PunchOptions()
                {
                    Frequency = 10,
                    DampingRatio = 1f
                };
            }
        }

        public readonly bool Equals(PunchOptions other)
        {
            return tweenOptions.Equals(other.tweenOptions) 
                && Frequency == other.Frequency 
                && DampingRatio == other.DampingRatio;
        }

        public override readonly bool Equals(object obj)
        {
            if (obj is PunchOptions options) return Equals(options);
            return false;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(tweenOptions, Frequency, DampingRatio);
        }
    }
}