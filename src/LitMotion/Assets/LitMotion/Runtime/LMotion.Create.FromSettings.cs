using UnityEngine;
using LitMotion.Adapters;

namespace LitMotion
{
    public static partial class LMotion
    {
        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<float, float, TweenOption, TweenAnimationSpec<float, TweenOption>> Create(MotionSettings<float, TweenOption> settings) => Create<float, float, TweenOption, TweenAnimationSpec<float, TweenOption>>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<double, float, TweenOption, TweenAnimationSpec<float, TweenOption>> Create(MotionSettings<double, TweenOption> settings) => Create<double, float, TweenOption, TweenAnimationSpec<float, TweenOption>>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<int, float, IntegerOptions, TweenAnimationSpec<float, IntegerOptions>> Create(MotionSettings<int, IntegerOptions> settings) => Create<int, float, IntegerOptions, TweenAnimationSpec<float, IntegerOptions>>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<long, float, IntegerOptions, TweenAnimationSpec<float, IntegerOptions>> Create(MotionSettings<long, IntegerOptions> settings) => Create<long, float, IntegerOptions, TweenAnimationSpec<float, IntegerOptions>>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Vector2, Vector2, TweenOption, TweenAnimationSpec<Vector2, TweenOption>> Create(MotionSettings<Vector2, TweenOption> settings) => Create<Vector2, Vector2, TweenOption, TweenAnimationSpec<Vector2, TweenOption>>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Vector3, Vector3, TweenOption, TweenAnimationSpec<Vector3, TweenOption>> Create(MotionSettings<Vector3, TweenOption> settings) => Create<Vector3, Vector3, TweenOption, TweenAnimationSpec<Vector3, TweenOption>>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Vector4, Vector4, TweenOption, TweenAnimationSpec<Vector4, TweenOption>> Create(MotionSettings<Vector4, TweenOption> settings) => Create<Vector4, Vector4, TweenOption, TweenAnimationSpec<Vector4, TweenOption>>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Quaternion, Quaternion, TweenOption, TweenAnimationSpec<Quaternion, TweenOption>> Create(MotionSettings<Quaternion, TweenOption> settings) => Create<Quaternion, Quaternion, TweenOption, TweenAnimationSpec<Quaternion, TweenOption>>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Color, Color, TweenOption, TweenAnimationSpec<Color, TweenOption>> Create(MotionSettings<Color, TweenOption> settings) => Create<Color, Color, TweenOption, TweenAnimationSpec<Color, TweenOption>>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Rect, Rect, TweenOption, TweenAnimationSpec<Rect, TweenOption>> Create(MotionSettings<Rect, TweenOption> settings) => Create<Rect, Rect, TweenOption, TweenAnimationSpec<Rect, TweenOption>>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="VValue">The type of vectorized value for internal processing</typeparam>
        /// <typeparam name="TOptions">The type of special parameters given to the motion entity</typeparam>
        /// <typeparam name="TAnimationSpec">The type of animation specification</typeparam>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<TValue, VValue, TOptions, TAnimationSpec> Create<TValue, VValue, TOptions, TAnimationSpec>(MotionSettings<TValue, TOptions> settings)
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
        {
            var buffer = MotionBuilderBuffer<TValue, TOptions>.Rent();
            buffer.StartValue = settings.StartValue;
            buffer.EndValue = settings.EndValue;
            buffer.Options = settings.Options;
            buffer.CancelOnError = settings.CancelOnError;
            buffer.SkipValuesDuringDelay = settings.SkipValuesDuringDelay;
            buffer.ImmediateBind = settings.ImmediateBind;
            buffer.Scheduler = settings.Scheduler;
            return new MotionBuilder<TValue, VValue, TOptions, TAnimationSpec>(buffer);
        }
    }
}