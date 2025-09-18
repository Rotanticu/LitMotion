using UnityEngine;
using LitMotion.Adapters;

namespace LitMotion
{
    /// <summary>
    /// The main class of the LitMotion library that creates and configures motion.
    /// </summary>
    public static partial class LMotion
    {
        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="from">Start value</param>
        /// <param name="to">End value</param>
        /// <param name="duration">Duration</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<float, float, TweenOption, TweenAnimationSpec<float, TweenOption>> Create(float from, float to, float duration)
        {
            var options = new TweenOption
            {
                DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                Loops = 1,
                DelayNanos = 0,
                DelayType = DelayType.FirstLoop,
                LoopType = LoopType.Restart,
                Ease = Ease.Linear
            };
            return Create<float, float, TweenOption, TweenAnimationSpec<float, TweenOption>>(from, to, options);
        }

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="from">Start value</param>
        /// <param name="to">End value</param>
        /// <param name="duration">Duration</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<double, double, TweenOption, TweenAnimationSpec<double, TweenOption>> Create(double from, double to, float duration)
        {
            var options = new TweenOption
            {
                DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                Loops = 1,
                DelayNanos = 0,
                DelayType = DelayType.FirstLoop,
                LoopType = LoopType.Restart,
                Ease = Ease.Linear
            };
            return Create<double, double, TweenOption, TweenAnimationSpec<double, TweenOption>>(from, to, options);
        }

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="from">Start value</param>
        /// <param name="to">End value</param>
        /// <param name="duration">Duration</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<int, float, IntegerOptions, TweenAnimationSpec<float, IntegerOptions>> Create(int from, int to, float duration)
        {
            var options = new IntegerOptions
            {
                DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                Loops = 1,
                DelayNanos = 0,
                DelayType = DelayType.FirstLoop,
                LoopType = LoopType.Restart,
                Ease = Ease.Linear
            };
            return Create<int, float, IntegerOptions, TweenAnimationSpec<float, IntegerOptions>>(from, to, options);
        }

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="from">Start value</param>
        /// <param name="to">End value</param>
        /// <param name="duration">Duration</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<long, float, IntegerOptions, TweenAnimationSpec<float, IntegerOptions>> Create(long from, long to, float duration)
        {
            var options = new IntegerOptions
            {
                DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                Loops = 1,
                DelayNanos = 0,
                DelayType = DelayType.FirstLoop,
                LoopType = LoopType.Restart,
                Ease = Ease.Linear
            };
            return Create<long, float, IntegerOptions, TweenAnimationSpec<float, IntegerOptions>>(from, to, options);
        }

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="from">Start value</param>
        /// <param name="to">End value</param>
        /// <param name="duration">Duration</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Vector2, Vector2, TweenOption, TweenAnimationSpec<Vector2, TweenOption>> Create(Vector2 from, Vector2 to, float duration)
        {
            var options = new TweenOption
            {
                DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                Loops = 1,
                DelayNanos = 0,
                DelayType = DelayType.FirstLoop,
                LoopType = LoopType.Restart,
                Ease = Ease.Linear
            };
            return Create<Vector2, Vector2, TweenOption, TweenAnimationSpec<Vector2, TweenOption>>(from, to, options);
        }

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="from">Start value</param>
        /// <param name="to">End value</param>
        /// <param name="duration">Duration</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Vector3, Vector3, TweenOption, TweenAnimationSpec<Vector3, TweenOption>> Create(Vector3 from, Vector3 to, float duration)
        {
            var options = new TweenOption
            {
                DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                Loops = 1,
                DelayNanos = 0,
                DelayType = DelayType.FirstLoop,
                LoopType = LoopType.Restart,
                Ease = Ease.Linear
            };
            return Create<Vector3, Vector3, TweenOption, TweenAnimationSpec<Vector3, TweenOption>>(from, to, options);
        }

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="from">Start value</param>
        /// <param name="to">End value</param>
        /// <param name="duration">Duration</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Vector4, Vector4, TweenOption, TweenAnimationSpec<Vector4, TweenOption>> Create(Vector4 from, Vector4 to, float duration)
        {
            var options = new TweenOption
            {
                DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                Loops = 1,
                DelayNanos = 0,
                DelayType = DelayType.FirstLoop,
                LoopType = LoopType.Restart,
                Ease = Ease.Linear
            };
            return Create<Vector4, Vector4, TweenOption, TweenAnimationSpec<Vector4, TweenOption>>(from, to, options);
        }

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="from">Start value</param>
        /// <param name="to">End value</param>
        /// <param name="duration">Duration</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Quaternion, Quaternion, TweenOption, TweenAnimationSpec<Quaternion, TweenOption>> Create(Quaternion from, Quaternion to, float duration)
        {
            var options = new TweenOption
            {
                DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                Loops = 1,
                DelayNanos = 0,
                DelayType = DelayType.FirstLoop,
                LoopType = LoopType.Restart,
                Ease = Ease.Linear
            };
            return Create<Quaternion, Quaternion, TweenOption, TweenAnimationSpec<Quaternion, TweenOption>>(from, to, options);
        }

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="from">Start value</param>
        /// <param name="to">End value</param>
        /// <param name="duration">Duration</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Color, Color, TweenOption, TweenAnimationSpec<Color, TweenOption>> Create(Color from, Color to, float duration)
        {
            var options = new TweenOption
            {
                DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                Loops = 1,
                DelayNanos = 0,
                DelayType = DelayType.FirstLoop,
                LoopType = LoopType.Restart,
                Ease = Ease.Linear
            };
            return Create<Color, Color, TweenOption, TweenAnimationSpec<Color, TweenOption>>(from, to, options);
        }

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="from">Start value</param>
        /// <param name="to">End value</param>
        /// <param name="duration">Duration</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Rect, Rect, TweenOption, TweenAnimationSpec<Rect, TweenOption>> Create(Rect from, Rect to, float duration)
        {
            var options = new TweenOption
            {
                DurationNanos = (long)(duration * AnimationConstants.SecondsToNanos),
                Loops = 1,
                DelayNanos = 0,
                DelayType = DelayType.FirstLoop,
                LoopType = LoopType.Restart,
                Ease = Ease.Linear
            };
            return Create<Rect, Rect, TweenOption, TweenAnimationSpec<Rect, TweenOption>>(from, to, options);
        }


        /// <summary>
        /// Create a builder for building motion with explicit animation specification.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="VValue">The type of vectorized value for internal processing</typeparam>
        /// <typeparam name="TOptions">The type of special parameters given to the motion entity</typeparam>
        /// <typeparam name="TAnimationSpec">The type of animation specification</typeparam>
        /// <param name="from">Start value</param>
        /// <param name="to">End value</param>
        /// <param name="options">Animation options</param>
        /// <returns>Created motion builder</returns>
        // public static MotionBuilder<TValue, VValue, TOptions, TAnimationSpec> Create<TValue, VValue, TOptions, TAnimationSpec>(in TValue from, in TValue to, TOptions options)
        //     where TValue : unmanaged
        //     where VValue : unmanaged
        //     where TOptions : unmanaged, IMotionOptions
        //     where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
        // {
        //     var buffer = MotionBuilderBuffer<TValue, TOptions>.Rent();
        //     buffer.StartValue = from;
        //     buffer.EndValue = to;
        //     buffer.Options = options;
        //     return new MotionBuilder<TValue, VValue, TOptions, TAnimationSpec>(buffer);
        // }

        public static MotionBuilder<TValue, VValue, TOptions, TAnimationSpec> Create<TValue, VValue, TOptions, TAnimationSpec>(in TValue from, in TValue to, TOptions options)
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
        {
            var buffer = MotionBuilderBuffer<TValue, TOptions>.Rent();
            buffer.StartValue = from;
            buffer.EndValue = to;
            buffer.Options = options;
            return new MotionBuilder<TValue, VValue, TOptions, TAnimationSpec>(buffer);
        }
    }
}