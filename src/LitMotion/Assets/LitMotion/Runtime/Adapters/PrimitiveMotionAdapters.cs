using Unity.Jobs;
using Unity.Mathematics;
using LitMotion;
using LitMotion.Adapters;

// TODO: 需要实现支持IntegerOptions的AnimationSpec后取消注释
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<float, float, TweenOption, TweenAnimationSpec<float>>))]
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<double, double, TweenOption, TweenAnimationSpec<double>>))]
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<int, int, IntegerOptions, IntegerAnimationSpec<int>>))]
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<long, long, IntegerOptions, IntegerAnimationSpec<long>>))]

namespace LitMotion.Adapters
{
    public readonly struct FloatMotionAdapter : IMotionAdapter<float, float, TweenOption>
    {
        public float ConvertToVector(float value) => value;
        public float ConvertFromVector(float value) => value;
        
        public float Evaluate(ref float startValue, ref float endValue, ref TweenOption options, in MotionEvaluationContext context)
        {
            return math.lerp(startValue, endValue, context.Progress);
        }
    }

    public readonly struct DoubleMotionAdapter : IMotionAdapter<double, double, TweenOption>
    {
        public double ConvertToVector(double value) => value;
        public double ConvertFromVector(double value) => value;
        
        public double Evaluate(ref double startValue, ref double endValue, ref TweenOption options, in MotionEvaluationContext context)
        {
            return math.lerp(startValue, endValue, context.Progress);
        }
    }

    public readonly struct IntMotionAdapter : IMotionAdapter<int, int, IntegerOptions>
    {
        public int ConvertToVector(int value) => value;
        public int ConvertFromVector(int value) => value;
        
        public int Evaluate(ref int startValue, ref int endValue, ref IntegerOptions options, in MotionEvaluationContext context)
        {
            var value = math.lerp(startValue, endValue, context.Progress);

            return options.RoundingMode switch
            {
                RoundingMode.AwayFromZero => value >= 0f ? (int)math.ceil(value) : (int)math.floor(value),
                RoundingMode.ToZero => (int)math.trunc(value),
                RoundingMode.ToPositiveInfinity => (int)math.ceil(value),
                RoundingMode.ToNegativeInfinity => (int)math.floor(value),
                _ => (int)math.round(value),
            };
        }
    }
    
    public readonly struct LongMotionAdapter : IMotionAdapter<long, long, IntegerOptions>
    {
        public long ConvertToVector(long value) => value;
        public long ConvertFromVector(long value) => value;
        
        public long Evaluate(ref long startValue, ref long endValue, ref IntegerOptions options, in MotionEvaluationContext context)
        {
            var value = math.lerp((double)startValue, endValue, context.Progress);

            return options.RoundingMode switch
            {
                RoundingMode.AwayFromZero => value >= 0f ? (long)math.ceil(value) : (long)math.floor(value),
                RoundingMode.ToZero => (long)math.trunc(value),
                RoundingMode.ToPositiveInfinity => (long)math.ceil(value),
                RoundingMode.ToNegativeInfinity => (long)math.floor(value),
                _ => (long)math.round(value),
            };
        }
    }
}