using System;
using LitMotion.Collections;

namespace LitMotion
{
    /// <summary>
    /// Interface for tween animation options that provides common properties for all tween-based animations.
    /// </summary>
    public interface ITweenOptions : IMotionOptions
    {
        /// <summary>
        /// Duration of the animation in nanoseconds.
        /// </summary>
        long DurationNanos { get; set; }

        /// <summary>
        /// Number of loops. Use -1 for infinite loops.
        /// </summary>
        int Loops { get; set; }

        /// <summary>
        /// Delay before the animation starts in nanoseconds.
        /// </summary>
        long DelayNanos { get; set; }

        /// <summary>
        /// Type of delay behavior.
        /// </summary>
        DelayType DelayType { get; set; }

        /// <summary>
        /// Type of loop behavior.
        /// </summary>
        LoopType LoopType { get; set; }

        /// <summary>
        /// Easing function for the animation.
        /// </summary>
        Ease Ease { get; set; }

        /// <summary>
        /// Custom animation curve for the easing.
        /// </summary>
        #if LITMOTION_COLLECTIONS_2_0_OR_NEWER
            NativeAnimationCurve AnimationCurve { get; set; }
        #else
            UnsafeAnimationCurve AnimationCurve { get; set; }
        #endif
    }
} 