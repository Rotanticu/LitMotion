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
            // 调用SpringElastic函数计算新的位置和速度
            SpringUtility.SpringElastic(
                (float)context.DeltaTime,
                ref options.CurrentValue.x,
                ref options.CurrentVelocity.x,
                endValue,
                options.TargetVelocity.x,
                options.DampingRatio,
                options.Stiffness
            );
            return options.CurrentValue.x;
        }
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
            
            // 转换为float4进行计算
            float4 f4EndValue = new float4(endValue, 0, 0);
            // 使用float4版本的SpringElastic
            SpringUtility.SpringElastic(
                deltaTime,
                ref options.CurrentValue,
                ref options.CurrentVelocity,
                f4EndValue,
                options.TargetVelocity,
                options.DampingRatio,
                options.Stiffness
            );
            return options.CurrentValue.xy;
        }
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
            // 转换为float4进行计算
            float4 f4EndValue = new float4(endValue, 0);
            // 使用float4版本的SpringElastic
            SpringUtility.SpringElastic(
                deltaTime,
                ref options.CurrentValue,
                ref options.CurrentVelocity,
                f4EndValue,
                options.TargetVelocity,
                options.DampingRatio,
                options.Stiffness
            );
            return options.CurrentValue.xyz;
        }
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
            
            // 转换为float4进行计算
            float4 f4EndValue = endValue;
            // 使用float4版本的SpringElastic
            SpringUtility.SpringElastic(
                deltaTime,
                ref options.CurrentValue,
                ref options.CurrentVelocity,
                f4EndValue,
                options.TargetVelocity,
                options.DampingRatio,
                options.Stiffness
            );
            
            return options.CurrentValue;
        }
    }
}
