using System;
using LitMotion.Adapters;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LitMotion.Animation.Components
{
    public abstract class ValueAnimationComponent<TValue, VValue, TOptions, TAnimationSpec> : LitMotionAnimationComponent
        where TValue : unmanaged
        where VValue : unmanaged
        where TOptions : unmanaged, ITweenOptions
        where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
    {
        [SerializeField] SerializableMotionSettings<TValue, TOptions> settings;
        [SerializeField] UnityEvent<TValue> onValueChanged;

        public override MotionHandle Play()
        {
            return LMotion.Create<TValue, VValue, TOptions, TAnimationSpec>(settings)
                .Bind(this, (x, state) =>
                {
                    state.onValueChanged.Invoke(x);
                });
        }

        public override void OnStop() { }
    }

    [Serializable]
    [LitMotionAnimationComponentMenu("Value/Float")]
    public sealed class FloatValueAnimation : ValueAnimationComponent<float, float, TweenOption, TweenAnimationSpec<float, TweenOption>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Value/Double")]
    public sealed class DoubleValueAnimation : ValueAnimationComponent<double, double, TweenOption, TweenAnimationSpec<double, TweenOption>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Value/Int")]
    public sealed class IntValueAnimation : ValueAnimationComponent<int, float, IntegerOptions, TweenAnimationSpec<float, IntegerOptions>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Value/Long")]
    public sealed class LongValueAnimation : ValueAnimationComponent<long, float, IntegerOptions, TweenAnimationSpec<float, IntegerOptions>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Value/Vector2")]
    public sealed class Vector2ValueAnimation : ValueAnimationComponent<Vector2, Vector2, TweenOption, TweenAnimationSpec<Vector2, TweenOption>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Value/Vector3")]
    public sealed class Vector3ValueAnimation : ValueAnimationComponent<Vector3, Vector3, TweenOption, TweenAnimationSpec<Vector3, TweenOption>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Value/Vector4")]
    public sealed class Vector4ValueAnimation : ValueAnimationComponent<Vector4, Vector4, TweenOption, TweenAnimationSpec<Vector4, TweenOption>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Value/Color")]
    public sealed class ColorValueAnimation : ValueAnimationComponent<Color, Color, TweenOption, TweenAnimationSpec<Color, TweenOption>> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Value/String")]
    public sealed class StringValueAnimation : LitMotionAnimationComponent
    {
        [SerializeField] SerializableMotionSettings<FixedString512Bytes, StringOptions> settings;
        [SerializeField] UnityEvent<string> onValueChanged;

        public override MotionHandle Play()
        {
            return LMotion.Create<FixedString512Bytes, FixedString512Bytes, StringOptions, TweenAnimationSpec<FixedString512Bytes, StringOptions>>(settings)
                .Bind(this, static (x, state) =>
                {
                    // TODO: avoid allocation
                    state.onValueChanged.Invoke(x.ConvertToString());
                });
        }

        public override void OnStop() { }
    }
}