using System;
using UnityEngine;

namespace LitMotion
{
    public interface IVectorizedAnimationSpec<VValue, TOptions>
        where VValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
    {
        public unsafe AnimationState* State
        {
            get;
            set;
        }
        public unsafe TOptions* Options
        {
            get;
            set;
        }

        public MotionTimeKind TimeKind
        {
            get;
            set;
        }

        public bool IsInfinite { get; }
        
        // // 类型转换方法
        // VValue ConvertToVector<TValue>(TValue value) where TValue : unmanaged;
        // TValue ConvertFromVector<TValue>(VValue vector) where TValue : unmanaged;
        
        // 动画计算方法
        VValue GetValueFromNanos<TValue>(long playTimeNanos, VValue startValue, VValue targetValue, VValue startVelocity)
        where TValue : unmanaged;
        VValue GetVelocityFromNanos<TValue>(long playTimeNanos, VValue startValue, VValue targetValue, VValue startVelocity)
        where TValue : unmanaged;
        long GetDurationNanos<TValue>(VValue startValue, VValue targetValue, VValue startVelocity)
        where TValue : unmanaged;
        VValue GetEndVelocity<TValue>(VValue startValue, VValue targetValue, VValue startVelocity)
        where TValue : unmanaged;
    }
}