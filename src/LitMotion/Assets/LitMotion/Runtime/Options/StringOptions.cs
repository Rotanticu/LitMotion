using System;
using Unity.Collections;
using LitMotion.Collections;

namespace LitMotion
{
    /// <summary>
    /// Type of characters used to fill in invisible strings.
    /// </summary>
    public enum ScrambleMode : byte
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// A-Z
        /// </summary>
        Uppercase = 1,
        /// <summary>
        /// a-z
        /// </summary>
        Lowercase = 2,
        /// <summary>
        /// 0-9
        /// </summary>
        Numerals = 3,
        /// <summary>
        /// A-Z, a-z, 0-9
        /// </summary>
        All = 4,
        /// <summary>
        /// Custom characters.
        /// </summary>
        Custom = 5
    }

    /// <summary>
    /// Options for string type motion.
    /// </summary>
    [Serializable]
    public struct StringOptions : ITweenOptions, IEquatable<StringOptions>
    {
        private TweenOption tweenOptions;
        
        // StringOptions特有的属性
        public ScrambleMode ScrambleMode { get; set; }
        public bool RichTextEnabled { get; set; }
        public FixedString64Bytes CustomScrambleChars { get; set; }
        public uint RandomSeed { get; set; }

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

        public readonly bool Equals(StringOptions other)
        {
            return tweenOptions.Equals(other.tweenOptions) 
                && ScrambleMode == other.ScrambleMode
                && RichTextEnabled == other.RichTextEnabled
                && CustomScrambleChars == other.CustomScrambleChars
                && RandomSeed == other.RandomSeed;
        }

        public override readonly bool Equals(object obj)
        {
            if (obj is StringOptions options) return Equals(options);
            return false;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(tweenOptions, ScrambleMode, RichTextEnabled, CustomScrambleChars, RandomSeed);
        }
    }
}