using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using LitMotion;
using LitMotion.Adapters;

// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Vector2, Vector2, TweenOption, TweenAnimationSpec<Vector2>>))]
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Vector3, Vector3, TweenOption, TweenAnimationSpec<Vector3>>))]
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Vector4, Vector4, TweenOption, TweenAnimationSpec<Vector4>>))]
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Quaternion, Quaternion, TweenOption, TweenAnimationSpec<Quaternion>>))]
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Color, Color, TweenOption, TweenAnimationSpec<Color>>))]
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Rect, Vector4, TweenOption, TweenAnimationSpec<Vector4>>))]

namespace LitMotion.Adapters
{
    public readonly struct Vector2MotionAdapter : IMotionAdapter<Vector2, Vector2, TweenOption>
    {
        public Vector2 ConvertToVector(Vector2 value) => value;
        public Vector2 ConvertFromVector(Vector2 value) => value;
        
        public Vector2 Evaluate(ref Vector2 startValue, ref Vector2 endValue, ref TweenOption options, in MotionEvaluationContext context)
        {
            return Vector2.LerpUnclamped(startValue, endValue, context.Progress);
        }
    }

    public readonly struct Vector3MotionAdapter : IMotionAdapter<Vector3, Vector3, TweenOption>
    {
        public Vector3 ConvertToVector(Vector3 value) => value;
        public Vector3 ConvertFromVector(Vector3 value) => value;
        
        public Vector3 Evaluate(ref Vector3 startValue, ref Vector3 endValue, ref TweenOption options, in MotionEvaluationContext context)
        {
            return Vector3.LerpUnclamped(startValue, endValue, context.Progress);
        }
    }

    public readonly struct Vector4MotionAdapter : IMotionAdapter<Vector4, Vector4, TweenOption>
    {
        public Vector4 ConvertToVector(Vector4 value) => value;
        public Vector4 ConvertFromVector(Vector4 value) => value;
        
        public Vector4 Evaluate(ref Vector4 startValue, ref Vector4 endValue, ref TweenOption options, in MotionEvaluationContext context)
        {
            return Vector4.LerpUnclamped(startValue, endValue, context.Progress);
        }
    }

    public readonly struct QuaternionMotionAdapter : IMotionAdapter<Quaternion, Quaternion, TweenOption>
    {
        public Quaternion ConvertToVector(Quaternion value) => value;
        public Quaternion ConvertFromVector(Quaternion value) => value;
        
        public Quaternion Evaluate(ref Quaternion startValue, ref Quaternion endValue, ref TweenOption options, in MotionEvaluationContext context)
        {
            return Quaternion.LerpUnclamped(startValue, endValue, context.Progress);
        }
    }

    public readonly struct ColorMotionAdapter : IMotionAdapter<Color, Color, TweenOption>
    {
        public Color ConvertToVector(Color value) => value;
        public Color ConvertFromVector(Color value) => value;
        
        public Color Evaluate(ref Color startValue, ref Color endValue, ref TweenOption options, in MotionEvaluationContext context)
        {
            return Color.LerpUnclamped(startValue, endValue, context.Progress);
        }
    }



    public readonly struct RectMotionAdapter : IMotionAdapter<Rect, Vector4, TweenOption>
    {
        public Vector4 ConvertToVector(Rect value)
        {
            return new Vector4(value.x, value.y, value.width, value.height);
        }

        public Rect ConvertFromVector(Vector4 value)
        {
            return new Rect(value.x, value.y, value.z, value.w);
        }

        public Rect Evaluate(ref Rect startValue, ref Rect endValue, ref TweenOption options, in MotionEvaluationContext context)
        {
            var x = math.lerp(startValue.x, endValue.x, context.Progress);
            var y = math.lerp(startValue.y, endValue.y, context.Progress);
            var width = math.lerp(startValue.width, endValue.width, context.Progress);
            var height = math.lerp(startValue.height, endValue.height, context.Progress);

            return new Rect(x, y, width, height);
        }
    }
}