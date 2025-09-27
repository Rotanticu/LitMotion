using Unity.Collections;
using LitMotion.Adapters;

namespace LitMotion
{
    public static partial class LMotion
    {
        /// <summary>
        /// API for creating string motions.
        /// </summary>
        public static class String
        {
            /// <summary>
            /// Create a builder for building motion.
            /// </summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="duration">Duration</param>
            /// <returns>Created motion builder</returns>
            public static MotionBuilder<FixedString32Bytes, FixedString32Bytes, StringOptions, TweenAnimationSpec<FixedString32Bytes, StringOptions>> Create32Bytes(in FixedString32Bytes from, in FixedString32Bytes to, float duration)
            {
                var options = new StringOptions
                {
                    DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                    Loops = 1,
                    DelayNanos = 0,
                    DelayType = DelayType.FirstLoop,
                    LoopType = LoopType.Restart,
                    Ease = Ease.Linear
                };
                return Create<FixedString32Bytes, FixedString32Bytes, StringOptions, TweenAnimationSpec<FixedString32Bytes, StringOptions>>(from, to, options);
            }

            /// <summary>
            /// Create a builder for building motion.
            /// </summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="duration">Duration</param>
            /// <returns>Created motion builder</returns>
            public static MotionBuilder<FixedString64Bytes, FixedString64Bytes, StringOptions, TweenAnimationSpec<FixedString64Bytes, StringOptions>> Create64Bytes(in FixedString64Bytes from, in FixedString64Bytes to, float duration)
            {
                var options = new StringOptions
                {
                    DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                    Loops = 1,
                    DelayNanos = 0,
                    DelayType = DelayType.FirstLoop,
                    LoopType = LoopType.Restart,
                    Ease = Ease.Linear
                };
                return Create<FixedString64Bytes, FixedString64Bytes, StringOptions, TweenAnimationSpec<FixedString64Bytes, StringOptions>>(from, to, options);
            }

            /// <summary>
            /// Create a builder for building motion.
            /// </summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="duration">Duration</param>
            /// <returns>Created motion builder</returns>
            public static MotionBuilder<FixedString128Bytes, FixedString128Bytes, StringOptions, TweenAnimationSpec<FixedString128Bytes, StringOptions>> Create128Bytes(in FixedString128Bytes from, in FixedString128Bytes to, float duration)
            {
                var options = new StringOptions
                {
                    DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                    Loops = 1,
                    DelayNanos = 0,
                    DelayType = DelayType.FirstLoop,
                    LoopType = LoopType.Restart,
                    Ease = Ease.Linear
                };
                return Create<FixedString128Bytes, FixedString128Bytes, StringOptions, TweenAnimationSpec<FixedString128Bytes, StringOptions>>(from, to, options);
            }

            /// <summary>
            /// Create a builder for building motion.
            /// </summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="duration">Duration</param>
            /// <returns>Created motion builder</returns>
            public static MotionBuilder<FixedString512Bytes, FixedString512Bytes, StringOptions, TweenAnimationSpec<FixedString512Bytes, StringOptions>> Create512Bytes(in FixedString512Bytes from, in FixedString512Bytes to, float duration)
            {
                var options = new StringOptions
                {
                    DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                    Loops = 1,
                    DelayNanos = 0,
                    DelayType = DelayType.FirstLoop,
                    LoopType = LoopType.Restart,
                    Ease = Ease.Linear
                };
                return Create<FixedString512Bytes, FixedString512Bytes, StringOptions, TweenAnimationSpec<FixedString512Bytes, StringOptions>>(from, to, options);
            }

            /// <summary>
            /// Create a builder for building motion.
            /// </summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="duration">Duration</param>
            /// <returns>Created motion builder</returns>
            public static MotionBuilder<FixedString4096Bytes, FixedString4096Bytes, StringOptions, TweenAnimationSpec<FixedString4096Bytes, StringOptions>> Create4096Bytes(in FixedString4096Bytes from, in FixedString4096Bytes to, float duration)
            {
                var options = new StringOptions
                {
                    DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                    Loops = 1,
                    DelayNanos = 0,
                    DelayType = DelayType.FirstLoop,
                    LoopType = LoopType.Restart,
                    Ease = Ease.Linear
                };
                return Create<FixedString4096Bytes, FixedString4096Bytes, StringOptions, TweenAnimationSpec<FixedString4096Bytes, StringOptions>>(from, to, options);
            }
        }
    }
}