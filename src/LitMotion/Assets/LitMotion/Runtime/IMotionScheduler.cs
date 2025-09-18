namespace LitMotion
{
    /// <summary>
    /// Provides the function to schedule the execution of a motion.
    /// </summary>
    public interface IMotionScheduler
    {
        /// <summary>
        /// Schedule the motion.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="VValue">The type of vectorized value for internal processing</typeparam>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAnimationSpec">The type of animation specification</typeparam>
        /// <param name="builder">Motion builder</param>
        /// <returns>Motion handle</returns>
        MotionHandle Schedule<TValue, VValue, TOptions, TAnimationSpec>(ref MotionBuilder<TValue, VValue, TOptions, TAnimationSpec> builder)
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>;
    }

    /// <summary>
    /// Type of time used to play the motion
    /// </summary>
    public enum MotionTimeKind : byte
    {
        Time = 0,
        UnscaledTime = 1,
        Realtime = 2
    }
}