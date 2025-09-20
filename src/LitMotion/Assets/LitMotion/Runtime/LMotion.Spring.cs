using UnityEngine;
using LitMotion.Adapters;

namespace LitMotion
{
    public static partial class LMotion
    {
        /// <summary>
        /// API for creating Spring motions.
        /// </summary>
        public static class Spring
        {
            /// <summary>
            /// Create a builder for building Spring motion.
            /// </summary>
            /// <param name="startValue">Start value</param>
            /// <param name="endValue">End value</param>
            /// <param name="duration">Duration</param>
            /// <returns>Created motion builder</returns>
            public static MotionBuilder<float, SpringOptions, FloatSpringMotionAdapter> Create(float startValue, float endValue, float duration, SpringOptions options)
            {
                return Create<float, SpringOptions, FloatSpringMotionAdapter>(startValue, endValue, duration)
                    .WithOptions(options);
            }
        }
    }
}
