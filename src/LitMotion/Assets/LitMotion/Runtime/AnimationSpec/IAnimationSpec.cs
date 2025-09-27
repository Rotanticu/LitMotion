using System;

namespace LitMotion
{
    public static class AnimationConstants
    {
        /// <summary>
        /// Default animation duration in milliseconds. Used in [VectorizedAnimationSpec]s and [AnimationSpec]
        /// </summary>
        public const int DefaultDurationMillis = 300;

        /// <summary>
        /// Unspecified time constant, indicating that animation time has not been set.
        /// </summary>
        public const long UnspecifiedTime = long.MinValue;

        internal const long MillisToNanos = 1_000_000L;

        internal const long NanosToSeconds = 1_000_000_000L;

        internal const long SecondsToNanos = 1_000_000_000L;
    }
    
    /// <summary>
    /// Animation specification interface that describes how to animate from start to end.
    /// </summary>
    /// <typeparam name="TValue">Animation value type</typeparam>
    /// <typeparam name="TOptions">Animation options type</typeparam>
    public interface IAnimationSpec<TValue, TOptions>
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
    {
        /// <summary>
        /// Create a vectorized animation specification using the given two-way converter.
        /// The underlying animation system operates on VValue.
        /// TValue animation values are converted to VValue for animation processing. IVectorizedAnimationSpec describes how the converted VValue should be animated.
        /// For example: animations can simply interpolate between start and end values (like TweenSpec), or apply spring physics effects to generate motion (like SpringSpec), etc.
        /// </summary>
        /// <typeparam name="VValue">Vectorized value type</typeparam>
        /// <param name="converter">Converter used to convert between TValue type and VValue type</param>
        /// <returns>Vectorized animation specification</returns>
        IVectorizedAnimationSpec<VValue, TOptions> Vectorize<VValue>(ITwoWayConverter<TValue, VValue> converter)
            where VValue : unmanaged;
    }
} 