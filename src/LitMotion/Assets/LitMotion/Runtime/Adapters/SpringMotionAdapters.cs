using Unity.Jobs;
using UnityEngine;
using LitMotion;
using LitMotion.Adapters;

[assembly: RegisterGenericJobType(typeof(MotionUpdateJob<float, SpringOptions, FloatSpringMotionAdapter>))]

namespace LitMotion.Adapters
{
    public readonly struct FloatSpringMotionAdapter : IMotionAdapter<float, SpringOptions>
    {
        public float Evaluate(ref float startValue, ref float endValue, ref SpringOptions options, in MotionEvaluationContext context)
        {
            // 更新SpringOptions中的当前状态
            options.CurrentValue = startValue;
            options.TargetValue = endValue;
            
            // 使用MotionEvaluationContext中的DeltaTime
            double deltaTime = context.DeltaTime;
            
            // 调用SpringElastic函数计算新的位置和速度
            double newVelocity;
            double newPosition = SpringUtility.SpringElastic(
                deltaTime,
                options.CurrentValue,
                options.CurrentVelocity,
                options.TargetValue,
                out newVelocity,
                options.TargetVelocity,
                options.DampingRatio,
                options.Stiffness
            );
            
            // 更新SpringOptions中的状态
            options.CurrentValue = (float)newPosition;
            options.CurrentVelocity = (float)newVelocity;
            
            return (float)newPosition;
        }
    }
}
