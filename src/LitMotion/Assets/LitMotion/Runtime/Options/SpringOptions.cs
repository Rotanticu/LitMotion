using System;
using Unity.Mathematics;
using UnityEngine;

namespace LitMotion
{
    /// <summary>
    /// Options for spring motion.
    /// </summary>
    [Serializable]
    public struct SpringOptions : IEquatable<SpringOptions>, IMotionOptions
    {
        private float4 _startValue;
        private float4 _startVelocity;
        public float4 CurrentValue;
        public float4 CurrentVelocity;
        public float4 TargetValue;
        public float4 TargetVelocity;
        public float Stiffness;
        public float DampingRatio;

        /// <summary>
        /// 默认构造函数，创建临界阻尼的Spring选项
        /// </summary>
        public SpringOptions(float stiffness = 10.0f, float dampingRatio = 1.0f,float4 startVelocity = default,float4 targetVelocity = default)
        {
            Stiffness = stiffness;
            DampingRatio = dampingRatio;
            CurrentValue = new float4(0.0f, 0.0f, 0.0f, 0.0f);
            CurrentVelocity = startVelocity;
            TargetValue = new float4(0.0f, 0.0f, 0.0f, 0.0f);
            TargetVelocity = targetVelocity;
            _startValue = new float4(0.0f, 0.0f, 0.0f, 0.0f);
            _startVelocity = startVelocity;
        }
        //public CompleteMode CompleteMode;
        public static SpringOptions Critical
        {
            get
            {
                return new SpringOptions()
                {
                    Stiffness = 10.0f,
                    DampingRatio = 1.0f,
                    CurrentVelocity = new float4(0.0f, 0.0f, 0.0f, 0.0f),
                    TargetVelocity = new float4(0.0f, 0.0f, 0.0f, 0.0f),
                    CurrentValue = new float4(0.0f, 0.0f, 0.0f, 0.0f)
                };
            }
        }

        public static SpringOptions Overdamped
        {
            get
            {
                return new SpringOptions()
                {
                    Stiffness = 10.0f,
                    DampingRatio = 1.2f,
                    CurrentVelocity = new float4(0.0f, 0.0f, 0.0f, 0.0f),
                    TargetVelocity = new float4(0.0f, 0.0f, 0.0f, 0.0f),
                    CurrentValue = new float4(0.0f, 0.0f, 0.0f, 0.0f)
                };
            }
        }

        public static SpringOptions Underdamped
        {
            get
            {
                return new SpringOptions()
                {
                    Stiffness = 10.0f,
                    DampingRatio = 0.6f,
                    CurrentVelocity = new float4(0.0f, 0.0f, 0.0f, 0.0f),
                    TargetVelocity = new float4(0.0f, 0.0f, 0.0f, 0.0f),
                    CurrentValue = new float4(0.0f, 0.0f, 0.0f, 0.0f)
                };
            }
        }

        public void Init(float4 startValue, float4 targetValue, float4 startVelocity = default, float4 targetVelocity = default)
        {
            CurrentValue = startValue;
            TargetValue = targetValue;
            CurrentVelocity = startVelocity;
            TargetVelocity = targetVelocity;
            _startValue = startValue;
            _startVelocity = startVelocity;
        }

        public void Restart()
        {
            CurrentValue = _startValue;
            CurrentVelocity = _startVelocity;
        }

        public readonly bool Equals(SpringOptions other)
        {
            return Stiffness == other.Stiffness &&
                   DampingRatio == other.DampingRatio &&
                   math.all(TargetVelocity == other.TargetVelocity) &&
                   math.all(CurrentVelocity == other.CurrentVelocity) &&
                   math.all(CurrentValue == other.CurrentValue);
        }

        public override readonly bool Equals(object obj)
        {
            if (obj is SpringOptions options) return Equals(options);
            return false;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Stiffness, DampingRatio, TargetVelocity, CurrentVelocity, CurrentValue);
        }
    }

    public enum CompleteMode : byte
    {
        /// <summary>
        /// 根据给出的持续时间计算出适当的刚度，只支持临界阻尼 
        /// </summary>
        DurationBased,
        /// <summary>
        /// 持续时间不定，收敛到阈值后结束。支持三种阻尼
        /// </summary>
        ConvergeToThreshold,
        /// <summary>
        /// 无线持续，必须手动结束。支持三种阻尼
        /// </summary>
        Infinite,
    }
}
