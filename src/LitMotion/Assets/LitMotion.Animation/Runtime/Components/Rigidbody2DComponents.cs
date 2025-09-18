#if LITMOTION_ANIMATION_PHYSICS_2D

using System;
using LitMotion.Adapters;
using UnityEngine;

namespace LitMotion.Animation.Components
{
    public abstract class Rigidbody2DPositionAnimationBase<TOptions, TAnimationSpec> : PropertyAnimationComponent<Rigidbody2D, Vector2, Vector2, TOptions, TAnimationSpec>
        where TOptions : unmanaged, ITweenOptions
        where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<Vector2, TOptions>
    {
        [SerializeField] bool useMovePosition = true;

        protected override Vector2 GetValue(Rigidbody2D target)
        {
            return target.position;
        }

        protected override void SetValue(Rigidbody2D target, in Vector2 value)
        {
            if (useMovePosition) target.MovePosition(value);
            else target.position = value;
        }

        protected override Vector2 GetRelativeValue(in Vector2 startValue, in Vector2 relativeValue)
        {
            return startValue + relativeValue;
        }
    }

    [Serializable]
    [LitMotionAnimationComponentMenu("Rigidbody2D/Position")]
    public sealed class Rigidbody2DPositionAnimation : Rigidbody2DPositionAnimationBase<TweenOption, TweenAnimationSpec<Vector2, TweenOption>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Rigidbody2D/Position (Punch)")]
    public sealed class Rigidbody2DPositionPunchAnimation : Rigidbody2DPositionAnimationBase<PunchOptions, TweenAnimationSpec<Vector2, PunchOptions>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Rigidbody2D/Position (Shake)")]
    public sealed class Rigidbody2DPositionShakeAnimation : Rigidbody2DPositionAnimationBase<ShakeOptions, TweenAnimationSpec<Vector2, ShakeOptions>> { }

    public abstract class Rigidbody2DRotationAnimationBase<TOptions, TAnimationSpec> : PropertyAnimationComponent<Rigidbody2D, float, float, TOptions, TAnimationSpec>
        where TOptions : unmanaged, ITweenOptions
        where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<float, TOptions>
    {
        [SerializeField] bool useMoveRotation;

        protected override float GetValue(Rigidbody2D target)
        {
            return target.rotation;
        }

        protected override void SetValue(Rigidbody2D target, in float value)
        {
            if (useMoveRotation) target.MoveRotation(value);
            else target.rotation = value;
        }

        protected override float GetRelativeValue(in float startValue, in float relativeValue)
        {
            return startValue + relativeValue;
        }
    }

    [Serializable]
    [LitMotionAnimationComponentMenu("Rigidbody2D/Rotation")]
    public sealed class Rigidbody2DRotationAnimation : Rigidbody2DRotationAnimationBase<TweenOption, TweenAnimationSpec<float, TweenOption>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Rigidbody2D/Rotation (Punch)")]
    public sealed class Rigidbody2DRotationPunchAnimation : Rigidbody2DRotationAnimationBase<PunchOptions, TweenAnimationSpec<float, PunchOptions>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Rigidbody2D/Rotation (Shake)")]
    public sealed class Rigidbody2DRotationShakeAnimation : Rigidbody2DRotationAnimationBase<ShakeOptions, TweenAnimationSpec<float, ShakeOptions>> { }
}

#endif