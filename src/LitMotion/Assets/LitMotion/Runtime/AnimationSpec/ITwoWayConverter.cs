namespace LitMotion
{
    /// <summary>
    /// Two-way converter between animation values and vectorized values.
    /// </summary>
    /// <typeparam name="TValue">Animation value type</typeparam>
    /// <typeparam name="VValue">Vectorized value type</typeparam>
    public interface ITwoWayConverter<TValue, VValue>
        where TValue : unmanaged
        where VValue : unmanaged
    {
        /// <summary>
        /// Defines how a type TValue should be converted to a vectorized type VValue.
        /// </summary>
        /// <param name="tValue">The value to convert</param>
        /// <returns>The vectorized value</returns>
        VValue ConvertToVector(TValue tValue);

        /// <summary>
        /// Defines how to convert a vectorized type VValue back to type TValue.
        /// </summary>
        /// <param name="vectorizedValue">The vectorized value to convert</param>
        /// <returns>The converted value</returns>
        TValue ConvertFromVector(VValue vectorizedValue);
    }
}
