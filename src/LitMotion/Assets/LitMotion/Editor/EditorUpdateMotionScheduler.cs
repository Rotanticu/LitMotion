using UnityEditor;

namespace LitMotion.Editor
{
    internal sealed class EditorUpdateMotionScheduler : IMotionScheduler
    {
        public double Time => EditorApplication.timeSinceStartup;

        public MotionHandle Schedule<TValue, VValue, TOptions, TAnimationSpec>(ref MotionBuilder<TValue, VValue, TOptions, TAnimationSpec> builder)
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
        {
            return EditorMotionDispatcher.Schedule<TValue, VValue, TOptions, TAnimationSpec>(ref builder);
        }
    }
}