using System;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;

namespace LitMotion
{
    public struct AnimationState
    {
        public MotionStatus Status;
        public MotionStatus PrevStatus;
        public bool IsPreserved;
        public bool IsInSequence;

        public ushort CompletedLoops;
        public ushort PrevCompletedLoops;

        public long playTimeNanos;

        public double Time
        {
            get
            {
                return playTimeNanos / AnimationConstants.NanosToSeconds;
            }
            set
            {
                playTimeNanos = (long)(value * AnimationConstants.SecondsToNanos);
            }
        }
        public float PlaybackSpeed;

        public readonly bool WasStatusChanged => Status != PrevStatus;
        public readonly bool WasLoopCompleted => CompletedLoops > PrevCompletedLoops;
    }

    public struct TargetBasedAnimation<TValue, VValue, TOptions, TAnimationSpec> : IMotion<TValue, VValue>
    where TValue : unmanaged
    where VValue : unmanaged
    where TOptions : unmanaged, IMotionOptions
    where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
    {

        public TAnimationSpec Core;

        // 移除接口依赖，改为使用函数指针或泛型约束
        // 暂时使用简单的转换逻辑，后续可以通过泛型约束或函数指针优化

        private TValue _mutableTargetValue;
        private TValue _mutableInitialValue;

        private VValue initialValueVector;
        private VValue targetValueVector;
        private VValue initialVelocityVector;

        //private long _durationNanos;
        private VValue _endVelocity;

        public MotionTimeKind MotionTimeKind
        {
            get => Core.TimeKind;
            set => Core.TimeKind = value;
        }

        public unsafe void Initialize(TOptions options, TValue startValue, TValue targetValue, VValue startVelocityVector = default)
        {
            Core.State->Status = MotionStatus.Scheduled;
            Core.State->Time = 0;
            Core.State->PlaybackSpeed = 1f;
            Core.State->IsPreserved = false;

            Core.Options = &options;
            _mutableInitialValue = startValue;
            _mutableTargetValue = targetValue;
            ConvertToVector(startValue,out initialValueVector);
            ConvertToVector(targetValue,out targetValueVector);
            initialVelocityVector = startVelocityVector;
            _endVelocity = default;
        }

        // 简单的转换方法，假设TValue和VValue是相同的类型
        private static void ConvertToVector(in TValue value,out VValue vectorValue)
        {
            // 这里需要根据具体类型实现转换逻辑
            // 暂时使用unsafe转换，假设内存布局相同
            vectorValue = default;
        }

        private static void ConvertFromVector(in VValue vectorValue, out TValue value)
        {
            value = default;
        }

        public TValue StartValue
        {
            get => _mutableInitialValue;
            set
            {
                if (!Equals(_mutableInitialValue, value))   
                {
                    _mutableInitialValue = value;
                    ConvertToVector(value,out initialValueVector);
                    _endVelocity = default;
                }
            }
        }

        public TValue TargetValue
        {
            get => _mutableTargetValue;
            set
            {
                if (!Equals(_mutableTargetValue, value))
                {
                    _mutableTargetValue = value;
                    ConvertToVector(value,out targetValueVector);
                    _endVelocity = default;
                }
            }
        }

        public bool IsInfinite => Core.IsInfinite;

        public void IsFinishedFromNanos(long playTimeNanos,out bool isFinished)
        {
            isFinished = Hint.Unlikely(playTimeNanos >= DurationNanos);
        }

        public void GetValueFromNanos(long playTimeNanos,out TValue currentValue)
        {
            IsFinishedFromNanos(playTimeNanos,out bool isFinished);
            if (Hint.Likely(!isFinished))
            {
                var vec = Core.GetValueFromNanos<TValue>(
                    playTimeNanos,
                    initialValueVector,
                    targetValueVector,
                    initialVelocityVector
                );
                ConvertFromVector(vec,out currentValue);
            }
            else
            {
                currentValue = TargetValue;
            }
        }
        public long DurationNanos
        {
            get
            {
                return Core.GetDurationNanos<TValue>(
        initialValueVector,
        targetValueVector,
        initialVelocityVector);
            }
        }

        private VValue EndVelocity
        {
            get
            {
                if (_endVelocity.Equals(default(VValue)))
                {
                    _endVelocity = Core.GetEndVelocity<TValue>(
                        initialValueVector,
                        targetValueVector,
                        initialVelocityVector
                    );
                }
                return _endVelocity;
            }
        }
        public void GetVelocityVectorFromNanos(long playTimeNanos,out VValue currentVelocity)
        {
            IsFinishedFromNanos(playTimeNanos,out bool isFinished);
            if (Hint.Likely(!isFinished))
            {
                currentVelocity = Core.GetVelocityFromNanos<TValue>(
                    playTimeNanos,
                    initialValueVector,
                    targetValueVector,
                    initialVelocityVector
                );
            }
            else
            {
                currentVelocity = EndVelocity;
            }
        }

        public void Complete(out TValue currentValue)
        {
            ConvertFromVector(Core.GetValueFromNanos<TValue>(
                DurationNanos,
                initialValueVector,
                targetValueVector,
                initialVelocityVector
            ),out currentValue);
        }

        public override string ToString()
        {
            return $"TargetBasedAnimation: {StartValue} -> {TargetValue}, initial velocity: {initialVelocityVector}, duration: {DurationNanos / 1_000_000} ms, animationSpec: {Core}";
        }
    }
}