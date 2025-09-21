using System;
using UnityEngine;

namespace LitMotion
{
    /// <summary>
    /// Options for spring motion.
    /// </summary>
    [Serializable]
    public struct SpringOptions : IEquatable<SpringOptions>, IMotionOptions
    {
        public Vector4 CurrentVelocity;
        public Vector4 TargetVelocity;
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
                    CurrentVelocity = new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
                    TargetVelocity = new Vector4(1.0f, 0.0f, 0.0f, 0.0f)
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
                    CurrentVelocity = new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
                    TargetVelocity = new Vector4(1.0f, 0.0f, 0.0f, 0.0f)
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
                    CurrentVelocity = new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
                    TargetVelocity = new Vector4(1.0f, 0.0f, 0.0f, 0.0f)
                };
            }
        }

        public readonly bool Equals(SpringOptions other)
        {
            return Stiffness == other.Stiffness &&
                   DampingRatio == other.DampingRatio &&
                   TargetVelocity == other.TargetVelocity &&
                   CurrentVelocity == other.CurrentVelocity &&
                   TargetVelocity == other.TargetVelocity;
        }

        public override readonly bool Equals(object obj)
        {
            if (obj is SpringOptions options) return Equals(options);
            return false;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Stiffness, DampingRatio, TargetVelocity, CurrentVelocity, TargetVelocity);
        }
    }
}
