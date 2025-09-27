namespace LitMotion
{
    /// <summary>
    /// Implement this interface to define animating values of a particular type.
    /// </summary>
    /// <typeparam name="TValue">The type of value to animate</typeparam>
    /// <typeparam name="VValue">The vectorized type for internal calculations</typeparam>
    /// <typeparam name="TOptions">The type of special parameters given to the motion entity</typeparam>
    public interface IMotionAdapter<TValue, VValue, TOptions> : ITwoWayConverter<TValue, VValue>
        where TValue : unmanaged
        where VValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
    {
        /// <summary>
        /// Define the process to interpolate the values between two points.
        /// </summary>
        /// <param name="startValue">Start value</param>
        /// <param name="endValue">End value</param>
        /// <param name="options">Option value to specify</param>
        /// <param name="context">Animation context</param>
        /// <returns>Current value</returns>
        TValue Evaluate(ref TValue startValue, ref TValue endValue, ref TOptions options, in MotionEvaluationContext context);

        bool IsCompleted(ref TValue startValue, ref TValue endValue, ref TOptions options)
        {
            return false;
        }

        bool IsDurationBased => true;
    }
}