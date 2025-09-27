using Unity.Jobs;
using UnityEngine;
using LitMotion;
using LitMotion.Adapters;
using Unity.Mathematics;

// TODO: 需要实现支持ShakeOptions的AnimationSpec后取消注释
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<float, float, ShakeOptions, ShakeAnimationSpec<float>>))]
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Vector2, Vector2, ShakeOptions, ShakeAnimationSpec<Vector2>>))]
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Vector3, Vector3, ShakeOptions, ShakeAnimationSpec<Vector3>>))]

namespace LitMotion.Adapters
{
    // Note: Shake motion uses startValue as offset and endValue as vibration strength.

    public readonly struct FloatShakeMotionAdapter : IMotionAdapter<float, float, ShakeOptions>
    {
        public float ConvertToVector(float value) => value;
        public float ConvertFromVector(float value) => value;
        
        public float Evaluate(ref float startValue, ref float endValue, ref ShakeOptions options, in MotionEvaluationContext context)
        {
            VibrationHelper.EvaluateStrength(endValue, options.Frequency, options.DampingRatio, context.Progress, out var s);
            var multipliar = RandomHelper.NextFloat(options.RandomSeed, context.Time, -1f, 1f);
            return startValue + s * multipliar;
        }
    }

    public readonly struct Vector2ShakeMotionAdapter : IMotionAdapter<Vector2, Vector2, ShakeOptions>
    {
        public Vector2 ConvertToVector(Vector2 value) => value;
        public Vector2 ConvertFromVector(Vector2 value) => value;
        
        public Vector2 Evaluate(ref Vector2 startValue, ref Vector2 endValue, ref ShakeOptions options, in MotionEvaluationContext context)
        {
            VibrationHelper.EvaluateStrength(endValue, options.Frequency, options.DampingRatio, context.Progress, out var s);
            var multipliar = RandomHelper.NextFloat2(options.RandomSeed, context.Time, new float2(-1f, -1f), new float2(1f, 1f));
            return startValue + new Vector2(s.x * multipliar.x, s.y * multipliar.y);
        }
    }

    public readonly struct Vector3ShakeMotionAdapter : IMotionAdapter<Vector3, Vector3, ShakeOptions>
    {
        public Vector3 ConvertToVector(Vector3 value) => value;
        public Vector3 ConvertFromVector(Vector3 value) => value;
        
        public Vector3 Evaluate(ref Vector3 startValue, ref Vector3 endValue, ref ShakeOptions options, in MotionEvaluationContext context)
        {
            VibrationHelper.EvaluateStrength(endValue, options.Frequency, options.DampingRatio, context.Progress, out var s);
            var multipliar = RandomHelper.NextFloat3(options.RandomSeed, context.Time, new float3(-1f, -1f, -1f), new float3(1f, 1f, 1f));
            return startValue + new Vector3(s.x * multipliar.x, s.y * multipliar.y, s.z * multipliar.z);
        }
    }
}