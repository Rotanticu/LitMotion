
using System;
using LitMotion.Adapters;
using UnityEngine;

namespace LitMotion.Animation.Components
{
    public abstract class TransformPositionAnimationBase<TOptions, TAnimationSpec> : PropertyAnimationComponent<Transform, Vector3, Vector3, TOptions, TAnimationSpec>
        where TOptions : unmanaged, ITweenOptions
        where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<Vector3, TOptions>
    {
        [SerializeField] bool useWorldSpace;

        protected override Vector3 GetValue(Transform target)
        {
            return useWorldSpace ? target.position : target.localPosition;
        }

        protected override void SetValue(Transform target, in Vector3 value)
        {
            if (useWorldSpace) target.position = value;
            else target.localPosition = value;
        }

        protected override Vector3 GetRelativeValue(in Vector3 startValue, in Vector3 relativeValue)
        {
            return startValue + relativeValue;
        }
    }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Position")]
    public sealed class TransformPositionAnimation : TransformPositionAnimationBase<TweenOption, TweenAnimationSpec<Vector3, TweenOption>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Position (Punch)")]
    public sealed class TransformPositionPunchAnimation : TransformPositionAnimationBase<PunchOptions, TweenAnimationSpec<Vector3, PunchOptions>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Position (Shake)")]
    public sealed class TransformPositionShakeAnimation : TransformPositionAnimationBase<ShakeOptions, TweenAnimationSpec<Vector3, ShakeOptions>> { }

    public abstract class TransformRotationAnimationBase<TOptions, TAnimationSpec> : PropertyAnimationComponent<Transform, Vector3, Vector3, TOptions, TAnimationSpec>
        where TOptions : unmanaged, ITweenOptions
        where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<Vector3, TOptions>
    {
        [SerializeField] bool useWorldSpace;

        protected override Vector3 GetValue(Transform target)
        {
            return useWorldSpace ? target.eulerAngles : target.localEulerAngles;
        }

        protected override void SetValue(Transform target, in Vector3 value)
        {
            if (useWorldSpace) target.eulerAngles = value;
            else target.localEulerAngles = value;
        }

        protected override Vector3 GetRelativeValue(in Vector3 startValue, in Vector3 relativeValue)
        {
            return startValue + relativeValue;
        }
    }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Rotation")]
    public sealed class TransformRotationAnimation : TransformRotationAnimationBase<TweenOption, TweenAnimationSpec<Vector3, TweenOption>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Rotation (Punch)")]
    public sealed class TransformRotationPunchAnimation : TransformRotationAnimationBase<PunchOptions, TweenAnimationSpec<Vector3, PunchOptions>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Rotation (Shake)")]
    public sealed class TransformRotationShakeAnimation : TransformRotationAnimationBase<ShakeOptions, TweenAnimationSpec<Vector3, ShakeOptions>> { }

    public abstract class TransformScaleAnimationBase<TOptions, TAnimationSpec> : PropertyAnimationComponent<Transform, Vector3, Vector3, TOptions, TAnimationSpec>
        where TOptions : unmanaged, ITweenOptions
        where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<Vector3, TOptions>
    {
        protected override Vector3 GetValue(Transform target)
        {
            return target.localScale;
        }

        protected override void SetValue(Transform target, in Vector3 value)
        {
            target.localScale = value;
        }

        protected override Vector3 GetRelativeValue(in Vector3 startValue, in Vector3 relativeValue)
        {
            return startValue + relativeValue;
        }
    }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Scale")]
    public sealed class TransformScaleAnimation : TransformScaleAnimationBase<TweenOption, TweenAnimationSpec<Vector3, TweenOption>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Scale (Punch)")]
    public sealed class TransformScalePunchAnimation : TransformScaleAnimationBase<PunchOptions, TweenAnimationSpec<Vector3, PunchOptions>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Scale (Shake)")]
    public sealed class TransformScaleShakeAnimation : TransformScaleAnimationBase<ShakeOptions, TweenAnimationSpec<Vector3, ShakeOptions>> { }
}