using Unity.Collections;
using Unity.Jobs;
using LitMotion;
using LitMotion.Adapters;

// TODO: 需要实现支持StringOptions的AnimationSpec后取消注释
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<FixedString32Bytes, FixedString32Bytes, StringOptions, FixedString32BytesMotionAdapter, StringAnimationSpec<FixedString32Bytes>>))]
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<FixedString64Bytes, FixedString64Bytes, StringOptions, FixedString64BytesMotionAdapter, StringAnimationSpec<FixedString64Bytes>>))]
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<FixedString128Bytes, FixedString128Bytes, StringOptions, FixedString128BytesMotionAdapter, StringAnimationSpec<FixedString128Bytes>>))]
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<FixedString512Bytes, FixedString512Bytes, StringOptions, FixedString512BytesMotionAdapter, StringAnimationSpec<FixedString512Bytes>>))]
// [assembly: RegisterGenericJobType(typeof(MotionUpdateJob<FixedString4096Bytes, FixedString4096Bytes, StringOptions, FixedString4096BytesMotionAdapter, StringAnimationSpec<FixedString4096Bytes>>))]

namespace LitMotion.Adapters
{
    public readonly struct FixedString32BytesMotionAdapter : IMotionAdapter<FixedString32Bytes, FixedString32Bytes, StringOptions>
    {
        public FixedString32Bytes ConvertToVector(FixedString32Bytes value)
        {
            return value;
        }

        public FixedString32Bytes ConvertFromVector(FixedString32Bytes value)
        {
            return value;
        }

        public FixedString32Bytes Evaluate(ref FixedString32Bytes startValue, ref FixedString32Bytes endValue, ref StringOptions options, in MotionEvaluationContext context)
        {
            var start = startValue;
            var end = endValue;
            var customScrambleChars = options.CustomScrambleChars;
            var randomState = RandomHelper.Create(options.RandomSeed, context.Time);
            FixedStringHelper.Interpolate(ref start, ref end, context.Progress, options.ScrambleMode, options.RichTextEnabled, ref randomState, ref customScrambleChars, out var result);
            return result;
        }
    }

    public readonly struct FixedString64BytesMotionAdapter : IMotionAdapter<FixedString64Bytes, FixedString64Bytes, StringOptions>
    {
        public FixedString64Bytes ConvertToVector(FixedString64Bytes value)
        {
            return value;
        }

        public FixedString64Bytes ConvertFromVector(FixedString64Bytes value)
        {
            return value;
        }

        public FixedString64Bytes Evaluate(ref FixedString64Bytes startValue, ref FixedString64Bytes endValue, ref StringOptions options, in MotionEvaluationContext context)
        {
            var start = startValue;
            var end = endValue;
            var customScrambleChars = options.CustomScrambleChars;
            var randomState = RandomHelper.Create(options.RandomSeed, context.Time);
            FixedStringHelper.Interpolate(ref start, ref end, context.Progress, options.ScrambleMode, options.RichTextEnabled, ref randomState, ref customScrambleChars, out var result);
            return result;
        }
    }

    public readonly struct FixedString128BytesMotionAdapter : IMotionAdapter<FixedString128Bytes, FixedString128Bytes, StringOptions>
    {
        public FixedString128Bytes ConvertToVector(FixedString128Bytes value)
        {
            return value;
        }

        public FixedString128Bytes ConvertFromVector(FixedString128Bytes value)
        {
            return value;
        }

        public FixedString128Bytes Evaluate(ref FixedString128Bytes startValue, ref FixedString128Bytes endValue, ref StringOptions options, in MotionEvaluationContext context)
        {
            var start = startValue;
            var end = endValue;
            var customScrambleChars = options.CustomScrambleChars;
            var randomState = RandomHelper.Create(options.RandomSeed, context.Time);
            FixedStringHelper.Interpolate(ref start, ref end, context.Progress, options.ScrambleMode, options.RichTextEnabled, ref randomState, ref customScrambleChars, out var result);
            return result;
        }
    }

    public readonly struct FixedString512BytesMotionAdapter : IMotionAdapter<FixedString512Bytes, FixedString512Bytes, StringOptions>
    {
        public FixedString512Bytes ConvertToVector(FixedString512Bytes value)
        {
            return value;
        }

        public FixedString512Bytes ConvertFromVector(FixedString512Bytes value)
        {
            return value;
        }

        public FixedString512Bytes Evaluate(ref FixedString512Bytes startValue, ref FixedString512Bytes endValue, ref StringOptions options, in MotionEvaluationContext context)
        {
            var start = startValue;
            var end = endValue;
            var customScrambleChars = options.CustomScrambleChars;
            var randomState = RandomHelper.Create(options.RandomSeed, context.Time);
            FixedStringHelper.Interpolate(ref start, ref end, context.Progress, options.ScrambleMode, options.RichTextEnabled, ref randomState, ref customScrambleChars, out var result);
            return result;
        }
    }

    public readonly struct FixedString4096BytesMotionAdapter : IMotionAdapter<FixedString4096Bytes, FixedString4096Bytes, StringOptions>
    {
        public FixedString4096Bytes ConvertToVector(FixedString4096Bytes value)
        {
            return value;
        }

        public FixedString4096Bytes ConvertFromVector(FixedString4096Bytes value)
        {
            return value;
        }

        public FixedString4096Bytes Evaluate(ref FixedString4096Bytes startValue, ref FixedString4096Bytes endValue, ref StringOptions options, in MotionEvaluationContext context)
        {
            var start = startValue;
            var end = endValue;
            var customScrambleChars = options.CustomScrambleChars;
            var randomState = RandomHelper.Create(options.RandomSeed, context.Time);
            FixedStringHelper.Interpolate(ref start, ref end, context.Progress, options.ScrambleMode, options.RichTextEnabled, ref randomState, ref customScrambleChars, out var result);
            return result;
        }
    }
}