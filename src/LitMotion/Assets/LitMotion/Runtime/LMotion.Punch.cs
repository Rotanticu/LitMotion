using UnityEngine;
using LitMotion.Adapters;

namespace LitMotion
{
    public static partial class LMotion
    {
        /// <summary>
        /// API for creating Punch motions.
        /// </summary>
        public static class Punch
        {
            /// <summary>
            /// Create a builder for building Punch motion.
            /// </summary>
            /// <param name="startValue">Start value</param>
            /// <param name="strength">Vibration strength</param>
            /// <param name="duration">Duration</param>
            /// <returns>Created motion builder</returns>
            public static MotionBuilder<float, float, PunchOptions, TweenAnimationSpec<float, PunchOptions>> Create(float startValue, float strength, float duration)
            {
                var options = new PunchOptions
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
                return Create<float, float, PunchOptions, TweenAnimationSpec<float, PunchOptions>>(startValue, startValue, options);
            }

            /// <summary>
            /// Create a builder for building Punch motion.
            /// </summary>
            /// <param name="startValue">Start value</param>
            /// <param name="strength">Vibration strength</param>
            /// <param name="duration">Duration</param>
            /// <returns>Created motion builder</returns>
            public static MotionBuilder<Vector2, Vector2, PunchOptions, TweenAnimationSpec<Vector2, PunchOptions>> Create(Vector2 startValue, Vector2 strength, float duration)
            {
                var options = new PunchOptions
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
                return Create<Vector2, Vector2, PunchOptions, TweenAnimationSpec<Vector2, PunchOptions>>(startValue, startValue, options);
            }

            /// <summary>
            /// Create a builder for building Punch motion.
            /// </summary>
            /// <param name="startValue">Start value</param>
            /// <param name="strength">Vibration strength</param>
            /// <param name="duration">Duration</param>
            /// <returns>Created motion builder</returns>
            public static MotionBuilder<Vector3, Vector3, PunchOptions, TweenAnimationSpec<Vector3, PunchOptions>> Create(Vector3 startValue, Vector3 strength, float duration)
            {
                var options = new PunchOptions
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
                return Create<Vector3, Vector3, PunchOptions, TweenAnimationSpec<Vector3, PunchOptions>>(startValue, startValue, options);
            }
        }
    }
}