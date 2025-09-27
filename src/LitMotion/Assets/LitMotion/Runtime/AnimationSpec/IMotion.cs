namespace LitMotion
{
    /// <summary>
    /// Interface for motion animations that support both user-facing values (TValue) and internal vectorized values (VValue).
    /// </summary>
    /// <typeparam name="TValue">User-facing animation value type</typeparam>
    /// <typeparam name="VValue">Internal vectorized value type</typeparam>
    public interface IMotion<TValue, VValue>
        where TValue : unmanaged
        where VValue : unmanaged
    {
        /// <summary>
        /// The time kind for this motion (scaled, unscaled, or realtime).
        /// </summary>
        public MotionTimeKind MotionTimeKind
        {
            get;
            set;
        }

        /// <summary>
        /// Whether this motion is infinite (loops indefinitely).
        /// </summary>
        public bool IsInfinite
        {
            get;
        }

        /// <summary>
        /// The duration of this motion in nanoseconds.
        /// </summary>
        public long DurationNanos
        {
            get;
        }


        /// <summary>
        /// Get the current value at the specified play time.
        /// </summary>
        /// <param name="playTimeNanos">Play time in nanoseconds</param>
        /// <returns>Current value</returns>
        public void GetValueFromNanos(long playTimeNanos,out TValue currentValue);

        /// <summary>
        /// Get the current velocity vector at the specified play time.
        /// </summary>
        /// <param name="playTimeNanos">Play time in nanoseconds</param>
        /// <returns>Current velocity vector</returns>
        public void GetVelocityVectorFromNanos(long playTimeNanos,out VValue currentVelocity);

        /// <summary>
        /// Check if the motion is finished at the specified play time.
        /// </summary>
        /// <param name="playTimeNanos">Play time in nanoseconds</param>
        /// <returns>True if the motion is finished</returns>
        public void IsFinishedFromNanos(long playTimeNanos,out bool isFinished);

        /// <summary>
        /// Complete the motion and return the final value.
        /// </summary>
        /// <returns>Final value</returns>
        public void Complete(out TValue currentValue);
    }
}