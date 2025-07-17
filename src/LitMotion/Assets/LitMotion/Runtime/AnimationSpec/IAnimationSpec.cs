using System;

namespace LitMotion
{
    public static class AnimationConstants
    {
        /// <summary>
        /// 默认动画持续时间，单位为毫秒。used in [VectorizedAnimationSpec]s and [AnimationSpec]
        /// </summary>
        public const int DefaultDurationMillis = 300;

        /// <summary>
        /// 未指定的时间常量，表示动画时间尚未设置。
        /// </summary>
        public const long UnspecifiedTime = long.MinValue;

        internal const long MillisToNanos = 1_000_000L;

        internal const long SecendsToNanos = 1_000_000_000L;
}
    /// <summary>
    /// 动画规范接口，描述如何从起点到终点进行动画。
    /// </summary>
    /// <typeparam name="T">动画值类型</typeparam>
    public interface IAnimationSpec<TValue, TOptions>
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
    {

        /// <summary>
        /// 使用给定的双向转换器创建一个向量化动画规范。
        /// 底层动画系统基于 AnimationVector 进行操作。
        /// T 类型的动画值会被转换为 AnimationVector 进行动画处理。VectorizedAnimationSpec 描述了转换后的 AnimationVector 应该如何被动画化。
        /// 例如：动画可以简单地在起始值和结束值之间插值（如 TweenSpec），也可以应用弹簧物理效果产生运动（如 SpringSpec）等。
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="converter">用于在 T 类型和 AnimationVector 类型之间转换的转换器</param>
        /// <returns></returns>
        IVectorizedAnimationSpec<TValue, TOptions> vectorize<TVector>(ITwoWayConverter<TValue, TVector> converter)
        where TVector : unmanaged;
    }

    /// <summary>
    /// 动画值与向量的双向转换器
    /// </summary>
    /// <typeparam name="T">动画值类型</typeparam>
    /// <typeparam name="V">向量类型</typeparam>
    public interface ITwoWayConverter<TValue, TVector>
    where TValue : unmanaged
    where TVector : unmanaged
    {
        TVector ConvertToVector(TValue value);
        TValue ConvertFromVector(TVector vector);
    }
} 