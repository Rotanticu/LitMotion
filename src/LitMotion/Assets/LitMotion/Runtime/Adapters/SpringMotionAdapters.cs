using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using LitMotion;
using LitMotion.Adapters;

[assembly: RegisterGenericJobType(typeof(MotionUpdateJob<float, SpringOptions, FloatSpringMotionAdapter>))]
[assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Vector2, SpringOptions, Vector2SpringMotionAdapter>))]
[assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Vector3, SpringOptions, Vector3SpringMotionAdapter>))]
[assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Vector4, SpringOptions, Vector4SpringMotionAdapter>))]

namespace LitMotion.Adapters
{
    public readonly struct FloatSpringMotionAdapter : IMotionAdapter<float, SpringOptions>
    {
        public float Evaluate(ref float startValue, ref float endValue, ref SpringOptions options, in MotionEvaluationContext context)
        {
            ref float targetValue = ref options.TargetValue.x;
            targetValue = endValue;
            // 调用SpringElastic函数计算新的位置和速度
            SpringUtility.SpringElastic(
                (float)context.DeltaTime,
                ref options.CurrentValue.x,
                ref options.CurrentVelocity.x,
                options.TargetValue.x,
                options.TargetVelocity.x,
                options.DampingRatio,
                options.Stiffness
            );
            return options.CurrentValue.x;
        }

        public bool IsCompleted(ref float startValue, ref float endValue, ref SpringOptions options)
        {
            // 使用阈值比较，避免精确相等比较的Burst问题
            bool isCompleted = SpringUtility.Approximately(options.CurrentValue.x, options.TargetValue.x);

            return isCompleted;
        }

        public bool IsDurationBased => false;
    }

    /// <summary>
    /// Vector2 Spring适配器，使用float4版本的SpringElastic方法
    /// </summary>
    public readonly struct Vector2SpringMotionAdapter : IMotionAdapter<Vector2, SpringOptions>
    {
        public Vector2 Evaluate(ref Vector2 startValue, ref Vector2 endValue, ref SpringOptions options, in MotionEvaluationContext context)
        {
            // 使用MotionEvaluationContext中的DeltaTime
            float deltaTime = (float)context.DeltaTime;
            options.TargetValue.xy = endValue;
            // 使用float4版本的SpringElastic
            SpringUtility.SpringElastic(
                deltaTime,
                ref options.CurrentValue,
                ref options.CurrentVelocity,
                options.TargetValue,
                options.TargetVelocity,
                options.DampingRatio,
                options.Stiffness
            );
            return options.CurrentValue.xy;
        }

        public bool IsCompleted(ref Vector2 startValue, ref Vector2 endValue, ref SpringOptions options)
        {
            // 检查是否收敛到目标值（使用阈值比较）
            bool isCompleted = SpringUtility.Approximately(options.CurrentValue, options.TargetValue);
            
            return isCompleted;
        }

        public bool IsDurationBased => false;
    }

    /// <summary>
    /// Vector3 Spring适配器，使用float4版本的SpringElastic方法
    /// </summary>
    public readonly struct Vector3SpringMotionAdapter : IMotionAdapter<Vector3, SpringOptions>
    {
        public Vector3 Evaluate(ref Vector3 startValue, ref Vector3 endValue, ref SpringOptions options, in MotionEvaluationContext context)
        {
            // 使用MotionEvaluationContext中的DeltaTime
            float deltaTime = (float)context.DeltaTime;
            ref float4 targetValue = ref options.TargetValue;
            options.TargetValue.xyz = endValue;
            // 使用float4版本的SpringElastic
            SpringUtility.SpringElastic(
                deltaTime,
                ref options.CurrentValue,
                ref options.CurrentVelocity,
                options.TargetValue,
                options.TargetVelocity,
                options.DampingRatio,
                options.Stiffness
            );
            return options.CurrentValue.xyz;
        }

        public bool IsCompleted(ref Vector3 startValue, ref Vector3 endValue, ref SpringOptions options)
        {
            // 检查是否收敛到目标值（使用阈值比较）
            bool isCompleted = SpringUtility.Approximately(options.CurrentValue, options.TargetValue);
            
            return isCompleted;
        }

        public bool IsDurationBased => false;
    }

    /// <summary>
    /// Vector4 Spring适配器，使用float4版本的SpringElastic方法
    /// </summary>
    public readonly struct Vector4SpringMotionAdapter : IMotionAdapter<Vector4, SpringOptions>
    {
        public Vector4 Evaluate(ref Vector4 startValue, ref Vector4 endValue, ref SpringOptions options, in MotionEvaluationContext context)
        {
            // 使用MotionEvaluationContext中的DeltaTime
            float deltaTime = (float)context.DeltaTime;
            ref float4 targetValue = ref options.TargetValue;
            targetValue = endValue;
            // 使用float4版本的SpringElastic
            SpringUtility.SpringElastic(
                deltaTime,
                ref options.CurrentValue,
                ref options.CurrentVelocity,
                options.TargetValue,
                options.TargetVelocity,
                options.DampingRatio,
                options.Stiffness
            );
            
            return options.CurrentValue;
        }

        public bool IsCompleted(ref Vector4 startValue, ref Vector4 endValue, ref SpringOptions options)
        {
            bool isCompleted = SpringUtility.Approximately(options.CurrentValue, options.TargetValue);
            return isCompleted;
        }

        public bool IsDurationBased => false;
    }
}
