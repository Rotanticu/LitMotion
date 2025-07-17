using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst.CompilerServices;
using LitMotion.Collections;

namespace LitMotion
{
    internal struct TweenAnimationSpec
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
            public float PlaybackSpeed;

            public readonly bool WasStatusChanged => Status != PrevStatus;
            public readonly bool WasLoopCompleted => CompletedLoops > PrevCompletedLoops;

        }
        public struct TweenSpecParameters
        {
            public MotionTimeKind TimeKind;
            public long DurationNanos;
            public long DelayNanos;
            public DelayType DelayType;
            public Ease Ease;
            public int Loops;
            public LoopType LoopType;

#if LITMOTION_COLLECTIONS_2_0_OR_NEWER
            public NativeAnimationCurve AnimationCurve;
#else
            public UnsafeAnimationCurve AnimationCurve;
#endif
        }
        public AnimationState State;
        public TweenSpecParameters Parameters;
        public Ease Ease
        {
            get => Parameters.Ease;
            set => Parameters.Ease = value;
        }
        public long TimeSinceStart => State.playTimeNanos - Parameters.DelayNanos;

        // MotionData interface - Update method
        public void Update(long playTimeNanos, out float progress)
        {
            State.PrevCompletedLoops = State.CompletedLoops;
            State.PrevStatus = State.Status;

            State.playTimeNanos = playTimeNanos;
            playTimeNanos = Math.Max(playTimeNanos, 0);

            double t;
            bool isCompleted;
            bool isDelayed;
            int completedLoops;
            int clampedCompletedLoops;

            if (Hint.Unlikely(Parameters.DurationNanos <= 0))
            {
                if (Parameters.DelayType == DelayType.FirstLoop || Parameters.DelayNanos == 0f)
                {
                    isCompleted = Parameters.Loops >= 0 && TimeSinceStart > 0f;
                    if (isCompleted)
                    {
                        t = 1f;
                        completedLoops = Parameters.Loops;
                    }
                    else
                    {
                        t = 0f;
                        completedLoops = TimeSinceStart < 0f ? -1 : 0;
                    }
                    clampedCompletedLoops = GetClampedCompletedLoops(completedLoops);
                    isDelayed = TimeSinceStart < 0;
                }
                else
                {
                    completedLoops = (int)Math.Floor((decimal)(playTimeNanos / Parameters.DelayNanos));
                    clampedCompletedLoops = GetClampedCompletedLoops(completedLoops);
                    isCompleted = Parameters.Loops >= 0 && clampedCompletedLoops > Parameters.Loops - 1;
                    isDelayed = !isCompleted;
                    t = isCompleted ? 1f : 0f;
                }
            }
            else
            {
                if (Parameters.DelayType == DelayType.FirstLoop)
                {
                    completedLoops = (int)Math.Floor((decimal)(TimeSinceStart / Parameters.DurationNanos));
                    clampedCompletedLoops = GetClampedCompletedLoops(completedLoops);
                    isCompleted = Parameters.Loops >= 0 && clampedCompletedLoops > Parameters.Loops - 1;
                    isDelayed = TimeSinceStart < 0f;

                    if (isCompleted)
                    {
                        t = 1f;
                    }
                    else
                    {
                        var currentLoopTime = TimeSinceStart - Parameters.DurationNanos * clampedCompletedLoops;
                        t = Math.Clamp(currentLoopTime / Parameters.DurationNanos, 0f, 1f);
                    }
                }
                else
                {
                    var currentLoopTime = (playTimeNanos % (Parameters.DurationNanos + Parameters.DelayNanos)) - Parameters.DelayNanos;
                    completedLoops = (int)Math.Floor((decimal)(playTimeNanos / (Parameters.DurationNanos + Parameters.DelayNanos)));
                    clampedCompletedLoops = GetClampedCompletedLoops(completedLoops);
                    isCompleted = Parameters.Loops >= 0 && clampedCompletedLoops > Parameters.Loops - 1;
                    isDelayed = currentLoopTime < 0;

                    if (isCompleted)
                    {
                        t = 1f;
                    }
                    else
                    {
                        t = math.clamp(currentLoopTime / Parameters.DurationNanos, 0f, 1f);
                    }
                }
            }
            State.CompletedLoops = (ushort)clampedCompletedLoops;

            switch (Parameters.LoopType)
            {
                default:
                case LoopType.Restart:
                    progress = GetEasedValue((float)t);
                    break;
                case LoopType.Flip:
                    progress = GetEasedValue((float)t);
                    if ((clampedCompletedLoops + (int)t) % 2 == 1) progress = 1f - progress;
                    break;
                case LoopType.Incremental:
                    progress = GetEasedValue(1f) * clampedCompletedLoops + GetEasedValue((float)math.fmod(t, 1f));
                    break;
                case LoopType.Yoyo:
                    progress = (clampedCompletedLoops + (int)t) % 2 == 1
                        ? GetEasedValue((float)(1f - t))
                        : GetEasedValue((float)t);
                    break;
            }

            if (isCompleted)
            {
                State.Status = MotionStatus.Completed;
            }
            else if (isDelayed || State.playTimeNanos < 0)
            {
                State.Status = MotionStatus.Delayed;
            }
            else
            {
                State.Status = MotionStatus.Playing;
            }
        }

        // MotionData interface - Complete method
        public void Complete(out float progress)
        {
            State.Status = MotionStatus.Completed;
            State.playTimeNanos = Parameters.DurationNanos;
            State.CompletedLoops = (ushort)Parameters.Loops;

            progress = GetEasedValue(Parameters.LoopType switch
            {
                LoopType.Restart => 1f,
                LoopType.Flip or LoopType.Yoyo => Parameters.Loops % 2 == 0 ? 0f : 1f,
                LoopType.Incremental => Parameters.Loops,
                _ => 1f
            });
        }
        int GetClampedCompletedLoops(int completedLoops)
        {
            return Parameters.Loops < 0
                ? math.max(0, completedLoops)
                : math.clamp(completedLoops, 0, Parameters.Loops);
        }

        float GetEasedValue(float value)
        {
            return Parameters.Ease switch
            {
                Ease.CustomAnimationCurve => Parameters.AnimationCurve.Evaluate(value),
                _ => EaseUtility.Evaluate(value, Parameters.Ease)
            };
        }
    }
    /// <summary>
    /// Tween animation specification that combines MotionData functionality with vectorized animation support.
    /// </summary>
    /// <typeparam name="V">Type of the animation vector</typeparam>
    internal class TweenAnimationSpec<TValue, TOptions> : LoopBasedAnimationSpec<TValue, TOptions>
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
    {
        // Because of pointer casting, this field must always be placed at the beginning.
        public TweenAnimationSpec Core;

        public TValue StartValue;
        public TValue EndValue;
        public TOptions Options;


        public override MotionTimeKind TimeKind
        {
            get => Core.Parameters.TimeKind;
            set => Core.Parameters.TimeKind = value;
        }

        public override long DelayNanos
        {
            get => Core.Parameters.DelayNanos;
            set => Core.Parameters.DelayNanos = value < 0 ? 0 : value;
        }

        public long DelayMillis
        {
            get => DelayNanos / AnimationConstants.MillisToNanos;
            set => DelayNanos = value * AnimationConstants.MillisToNanos;
        }

        public int DelaySeconds
        {
            get => (int)(DelayNanos / AnimationConstants.SecendsToNanos);
            set => DelayNanos = value * AnimationConstants.SecendsToNanos;
        }

        public override long DurationNanos
        {
            get => Core.Parameters.DurationNanos;
            set => Core.Parameters.DurationNanos = value < 0 ? 0 : value;
        }

        public long DurationMillis
        {
            get => DurationNanos / AnimationConstants.MillisToNanos;
            set => DurationNanos = value * AnimationConstants.MillisToNanos;
        }

        public int DurationSeconds
        {
            get => (int)(DurationNanos / AnimationConstants.SecendsToNanos);
            set => DurationNanos = value * AnimationConstants.SecendsToNanos;
        }

        public override int LoopCount
        {
            get => Core.Parameters.Loops;
            set => Core.Parameters.Loops = value < 0 ? -1 : value; // -1 for infinite loops
        }
        public override DelayType DelayType
        {
            get => Core.Parameters.DelayType;
            set => Core.Parameters.DelayType = value;
        }
        public override LoopType LoopType
        {
            get => Core.Parameters.LoopType;
            set => Core.Parameters.LoopType = value;
        }
        public override TValue GetValueFromNanos<TAdapter>(long playTimeNanos, TValue startValue, TValue targetValue, TValue startVelocity)
        {
            long clampedPlayTimeNanos = Math.Clamp(playTimeNanos, 0, DurationNanos);
            Core.Update(clampedPlayTimeNanos, out float fraction);
            return default(TAdapter).Evaluate(ref StartValue, ref EndValue, ref Options, new MotionEvaluationContext()
            {
                Progress = fraction,
                Time = clampedPlayTimeNanos,
            });
        }

        public override TValue GetVelocityFromNanos<TAdapter>(long playTimeNanos, TValue startValue, TValue targetValue, TValue startVelocity)
        {
            throw new NotImplementedException();
        }

        public override TValue GetEndVelocity<TAdapter>(TValue startValue, TValue targetValue, TValue startVelocity)
        {
            throw new NotImplementedException();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Complete<TAdapter>(out TValue result)
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            Core.Complete(out var progress);

            result = default(TAdapter).Evaluate(
                ref StartValue,
                ref EndValue,
                ref Options,
                new()
                {
                    Progress = progress,
                    Time = Core.State.playTimeNanos,
                }
            );
        }
    }
}

    