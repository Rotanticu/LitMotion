using System;
using Unity.Collections;

namespace LitMotion
{
    /// <summary>
    /// Provides additional Bind methods for MotionBuilder.
    /// </summary>
    public static class MotionBuilderExtensions
    {
        /// <summary>
        /// Create motion and bind it to a specific object. Unlike the regular Bind method, it avoids allocation by closure by passing an object.
        /// </summary>
        /// <typeparam name="TState">Type of state</typeparam>
        /// <param name="state">Motion state</param>
        /// <param name="action">Action that handles binding</param>
        /// <returns>Handle of the created motion data.</returns>
        public unsafe static MotionHandle Bind<TValue, VValue, TOptions, TAnimationSpec, TState>(this MotionBuilder<TValue, VValue, TOptions, TAnimationSpec> builder, TState state, Action<TValue, TState> action)
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
            where TState : struct
        {
            return builder.Bind(Box.Create(state), action, (value, state, action) => action(value, state.Value));
        }

        /// <summary>
        /// Specifies the rounding format for decimal values when animating integer types.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="VValue">The type of vectorized value for internal processing</typeparam>
        /// <typeparam name="TAnimationSpec">The type of animation specification</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="roundingMode">Rounding mode</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, VValue, IntegerOptions, TAnimationSpec> WithRoundingMode<TValue, VValue, TAnimationSpec>(this MotionBuilder<TValue, VValue, IntegerOptions, TAnimationSpec> builder, RoundingMode roundingMode)
            where TValue : unmanaged
            where VValue : unmanaged
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, IntegerOptions>
        {
            var options = builder.buffer.Options;
            options.RoundingMode = roundingMode;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Specify the frequency of vibration.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="VValue">The type of vectorized value for internal processing</typeparam>
        /// <typeparam name="TAnimationSpec">The type of animation specification</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="frequency">Frequency</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, VValue, PunchOptions, TAnimationSpec> WithFrequency<TValue, VValue, TAnimationSpec>(this MotionBuilder<TValue, VValue, PunchOptions, TAnimationSpec> builder, int frequency)
            where TValue : unmanaged
            where VValue : unmanaged
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, PunchOptions>
        {
            var options = builder.buffer.Options;
            options.Frequency = frequency;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Specify the vibration damping ratio.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="VValue">The type of vectorized value for internal processing</typeparam>
        /// <typeparam name="TAnimationSpec">The type of animation specification</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="dampingRatio">Damping ratio</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, VValue, PunchOptions, TAnimationSpec> WithDampingRatio<TValue, VValue, TAnimationSpec>(this MotionBuilder<TValue, VValue, PunchOptions, TAnimationSpec> builder, float dampingRatio)
            where TValue : unmanaged
            where VValue : unmanaged
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, PunchOptions>
        {
            var options = builder.buffer.Options;
            options.DampingRatio = dampingRatio;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Specify the frequency of vibration.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="VValue">The type of vectorized value for internal processing</typeparam>
        /// <typeparam name="TAnimationSpec">The type of animation specification</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="frequency">Frequency</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, VValue, ShakeOptions, TAnimationSpec> WithFrequency<TValue, VValue, TAnimationSpec>(this MotionBuilder<TValue, VValue, ShakeOptions, TAnimationSpec> builder, int frequency)
            where TValue : unmanaged
            where VValue : unmanaged
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, ShakeOptions>
        {
            var options = builder.buffer.Options;
            options.Frequency = frequency;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Specify the vibration damping ratio.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="VValue">The type of vectorized value for internal processing</typeparam>
        /// <typeparam name="TAnimationSpec">The type of animation specification</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="dampingRatio">Damping ratio</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, VValue, ShakeOptions, TAnimationSpec> WithDampingRatio<TValue, VValue, TAnimationSpec>(this MotionBuilder<TValue, VValue, ShakeOptions, TAnimationSpec> builder, float dampingRatio)
            where TValue : unmanaged
            where VValue : unmanaged
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, ShakeOptions>
        {
            var options = builder.buffer.Options;
            options.DampingRatio = dampingRatio;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Specify the random number seed that determines the shake motion value.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="VValue">The type of vectorized value for internal processing</typeparam>
        /// <typeparam name="TAnimationSpec">The type of animation specification</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="seed">Random number seed</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, VValue, ShakeOptions, TAnimationSpec> WithRandomSeed<TValue, VValue, TAnimationSpec>(this MotionBuilder<TValue, VValue, ShakeOptions, TAnimationSpec> builder, uint seed)
            where TValue : unmanaged
            where VValue : unmanaged
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, ShakeOptions>
        {
            var options = builder.buffer.Options;
            options.RandomSeed = seed;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Enable support for Rich Text tags.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="VValue">The type of vectorized value for internal processing</typeparam>
        /// <typeparam name="TAnimationSpec">The type of animation specification</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="richTextEnabled">Whether to support Rich Text tags</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, VValue, StringOptions, TAnimationSpec> WithRichText<TValue, VValue, TAnimationSpec>(this MotionBuilder<TValue, VValue, StringOptions, TAnimationSpec> builder, bool richTextEnabled = true)
            where TValue : unmanaged
            where VValue : unmanaged
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, StringOptions>
        {
            var options = builder.buffer.Options;
            options.RichTextEnabled = richTextEnabled;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Specify the random number seed used to display scramble characters.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="VValue">The type of vectorized value for internal processing</typeparam>
        /// <typeparam name="TAnimationSpec">The type of animation specification</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="seed">Rrandom number seed</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, VValue, StringOptions, TAnimationSpec> WithRandomSeed<TValue, VValue, TAnimationSpec>(this MotionBuilder<TValue, VValue, StringOptions, TAnimationSpec> builder, uint seed)
           where TValue : unmanaged
           where VValue : unmanaged
           where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, StringOptions>
        {
            var options = builder.buffer.Options;
            options.RandomSeed = seed;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Fill in the parts that are not yet displayed with random strings.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="VValue">The type of vectorized value for internal processing</typeparam>
        /// <typeparam name="TAnimationSpec">The type of animation specification</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="scrambleMode">Type of characters used for blank padding</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, VValue, StringOptions, TAnimationSpec> WithScrambleChars<TValue, VValue, TAnimationSpec>(this MotionBuilder<TValue, VValue, StringOptions, TAnimationSpec> builder, ScrambleMode scrambleMode)
            where TValue : unmanaged
            where VValue : unmanaged
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, StringOptions>
        {
            if (scrambleMode == ScrambleMode.Custom) throw new ArgumentException("ScrambleMode.Custom cannot be specified explicitly. Use WithScrambleMode(FixedString64Bytes) instead.");

            var options = builder.buffer.Options;
            options.ScrambleMode = scrambleMode;
            builder.buffer.Options = options;

            return builder;
        }

        /// <summary>
        /// Fill in the parts that are not yet displayed with random strings.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="VValue">The type of vectorized value for internal processing</typeparam>
        /// <typeparam name="TAnimationSpec">The type of animation specification</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="customScrambleChars">Characters used for blank padding</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, VValue, StringOptions, TAnimationSpec> WithScrambleChars<TValue, VValue, TAnimationSpec>(this MotionBuilder<TValue, VValue, StringOptions, TAnimationSpec> builder, FixedString64Bytes customScrambleChars)
            where TValue : unmanaged
            where VValue : unmanaged
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, StringOptions>
        {
            var options = builder.buffer.Options;
            options.ScrambleMode = ScrambleMode.Custom;
            options.CustomScrambleChars = customScrambleChars;
            builder.buffer.Options = options;
            return builder;
        }
    }
}