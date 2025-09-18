using UnityEngine;
using LitMotion.Adapters;

namespace LitMotion
{
    public static partial class LMotion
    {
        /// <summary>
        /// API for creating Shake motions.
        /// </summary>
        public static class Shake
        {
            /// <summary>
            /// Create a builder for building Shake motion.
            /// </summary>
            /// <param name="startValue">Start value</param>
            /// <param name="strength">Vibration strength</param>
            /// <param name="duration">Duration</param>
            /// <returns>Created motion builder</returns>
            public static MotionBuilder<float, float, ShakeOptions, TweenAnimationSpec<float, ShakeOptions>> Create(float startValue, float strength, float duration)
            {
                var options = new ShakeOptions
                {
                    DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                    Loops = 1,
                    DelayNanos = 0,
                    DelayType = DelayType.FirstLoop,
                    LoopType = LoopType.Restart,
                    Ease = Ease.Linear,
                    Frequency = 10,
                    DampingRatio = 1f
                };
                return Create<float, float, ShakeOptions, TweenAnimationSpec<float, ShakeOptions>>(startValue, startValue, options);
            }

            /// <summary>
            /// Create a builder for building Shake motion.
            /// </summary>
            /// <param name="startValue">Start value</param>
            /// <param name="strength">Vibration strength</param>
            /// <param name="duration">Duration</param>
            /// <returns>Created motion builder</returns>
            public static MotionBuilder<Vector2, Vector2, ShakeOptions, TweenAnimationSpec<Vector2, ShakeOptions>> Create(Vector2 startValue, Vector2 strength, float duration)
            {
                var options = new ShakeOptions
                {
                    DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                    Loops = 1,
                    DelayNanos = 0,
                    DelayType = DelayType.FirstLoop,
                    LoopType = LoopType.Restart,
                    Ease = Ease.Linear,
                    Frequency = 10,
                    DampingRatio = 1f
                };
                return Create<Vector2, Vector2, ShakeOptions, TweenAnimationSpec<Vector2, ShakeOptions>>(startValue, startValue, options);
            }

            /// <summary>
            /// Create a builder for building Shake motion.
            /// </summary>
            /// <param name="startValue">Start value</param>
            /// <param name="strength">Vibration strength</param>
            /// <param name="duration">Duration</param>
            /// <returns>Created motion builder</returns>
            public static MotionBuilder<Vector3, Vector3, ShakeOptions, TweenAnimationSpec<Vector3, ShakeOptions>> Create(Vector3 startValue, Vector3 strength, float duration)
            {
                var options = new ShakeOptions
                {
                    DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                    Loops = 1,
                    DelayNanos = 0,
                    DelayType = DelayType.FirstLoop,
                    LoopType = LoopType.Restart,
                    Ease = Ease.Linear,
                    Frequency = 10,
                    DampingRatio = 1f
                };
                return Create<Vector3, Vector3, ShakeOptions, TweenAnimationSpec<Vector3, ShakeOptions>>(startValue, startValue, options);
            }
        }
    }
}