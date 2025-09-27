using Unity.Jobs;
using UnityEngine;
using LitMotion;
using LitMotion.Adapters;

// TODO: 需要实现支持PunchOptions的AnimationSpec后取消注释
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<float, float, PunchOptions, PunchAnimationSpec<float>>))]
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Vector2, Vector2, PunchOptions, PunchAnimationSpec<Vector2>>))]
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Vector3, Vector3, PunchOptions, PunchAnimationSpec<Vector3>>))]

namespace LitMotion.Adapters
{
    // Note: Punch motion uses startValue as offset and endValue as vibration strength.

    public readonly struct FloatPunchMotionAdapter : IMotionAdapter<float, float, PunchOptions>
    {
        public float ConvertToVector(float value) => value;
        public float ConvertFromVector(float value) => value;
        
        public float Evaluate(ref float startValue, ref float endValue, ref PunchOptions options, in MotionEvaluationContext context)
        {
            VibrationHelper.EvaluateStrength(endValue, options.Frequency, options.DampingRatio, context.Progress, out var result);
            return startValue + result;
        }
    }

    public readonly struct Vector2PunchMotionAdapter : IMotionAdapter<Vector2, Vector2, PunchOptions>
    {
        public Vector2 ConvertToVector(Vector2 value) => value;
        public Vector2 ConvertFromVector(Vector2 value) => value;
        
        public Vector2 Evaluate(ref Vector2 startValue, ref Vector2 endValue, ref PunchOptions options, in MotionEvaluationContext context)
        {
            VibrationHelper.EvaluateStrength(endValue, options.Frequency, options.DampingRatio, context.Progress, out var result);
            return startValue + result;
        }
    }

    public readonly struct Vector3PunchMotionAdapter : IMotionAdapter<Vector3, Vector3, PunchOptions>
    {
        public Vector3 ConvertToVector(Vector3 value) => value;
        public Vector3 ConvertFromVector(Vector3 value) => value;
        
        public Vector3 Evaluate(ref Vector3 startValue, ref Vector3 endValue, ref PunchOptions options, in MotionEvaluationContext context)
        {
            VibrationHelper.EvaluateStrength(endValue, options.Frequency, options.DampingRatio, context.Progress, out var result);
            return startValue + result;
        }
    }
}