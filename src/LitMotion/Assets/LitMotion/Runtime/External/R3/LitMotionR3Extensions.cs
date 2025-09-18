#if LITMOTION_SUPPORT_R3
using R3;

namespace LitMotion
{
    public static class LitMotionR3Extensions
    {
        /// <summary>
        /// Create the motion as Observable.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="VValue">The type of vectorized value for internal processing</typeparam>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAnimationSpec">The type of animation specification</typeparam>
        /// <param name="builder">This builder</param>
        /// <returns>Observable of the created motion.</returns>
        public static Observable<TValue> ToObservable<TValue, VValue, TOptions, TAnimationSpec>(this MotionBuilder<TValue, VValue, TOptions, TAnimationSpec> builder)
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
        {
            var subject = new Subject<TValue>();
            builder.SetCallbackData(subject, static (x, subject) => subject.OnNext(x));
            builder.buffer.OnCompleteAction += () => subject.OnCompleted();
            builder.buffer.OnCancelAction += () => subject.OnCompleted();
            builder.ScheduleMotion();
            return subject;
        }

        /// <summary>
        /// Create a motion data and bind it to ReactiveProperty.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="VValue">The type of vectorized value for internal processing</typeparam>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAnimationSpec">The type of animation specification</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="reactiveProperty">Target ReactiveProperty to bind to</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToReactiveProperty<TValue, VValue, TOptions, TAnimationSpec>(this MotionBuilder<TValue, VValue, TOptions, TAnimationSpec> builder, ReactiveProperty<TValue> reactiveProperty)
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
        {
            Error.IsNull(reactiveProperty);
            return builder.Bind(reactiveProperty, static (x, target) =>
            {
                target.Value = x;
            });
        }
    }
}
#endif
