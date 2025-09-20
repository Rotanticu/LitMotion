using System;

namespace LitMotion
{
    /// <summary>
    /// Options for spring motion.
    /// </summary>
    [Serializable]
    public struct SpringOptions : IEquatable<SpringOptions>, IMotionOptions
    {
        public float CurrentValue;
        public float CurrentVelocity;
        public float TargetValue;
        public float TargetVelocity;
        public float Stiffness;
        public float DampingRatio;

        public static SpringOptions Critical
        {
            get
            {
                return new SpringOptions()
                {
                    Stiffness = 10.0f,
                    DampingRatio = 1.0f,
                    TargetVelocity = 0.0f,
                    CurrentValue = 0.0f,
                    CurrentVelocity = 0.0f,
                    TargetValue = 1.0f
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
                    TargetVelocity = 0.0f,
                    CurrentValue = 0.0f,
                    CurrentVelocity = 0.0f,
                    TargetValue = 1.0f
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
                    TargetVelocity = 0.0f,
                    CurrentValue = 0.0f,
                    CurrentVelocity = 0.0f,
                    TargetValue = 1.0f
                };
            }
        }

        public readonly bool Equals(SpringOptions other)
        {
            return Stiffness == other.Stiffness &&
                   DampingRatio == other.DampingRatio &&
                   TargetVelocity == other.TargetVelocity &&
                   CurrentValue == other.CurrentValue &&
                   CurrentVelocity == other.CurrentVelocity &&
                   TargetValue == other.TargetValue;
        }

        public override readonly bool Equals(object obj)
        {
            if (obj is SpringOptions options) return Equals(options);
            return false;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Stiffness, DampingRatio, TargetVelocity, CurrentValue, CurrentVelocity, TargetValue);
        }
    }
}
