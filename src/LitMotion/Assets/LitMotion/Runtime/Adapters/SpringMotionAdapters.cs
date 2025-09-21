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
            // 使用MotionEvaluationContext中的DeltaTime
            double deltaTime = context.DeltaTime;
            
            // 调用SpringElastic函数计算新的位置和速度
            double newVelocity;
            double newPosition = SpringUtility.SpringElastic(
                deltaTime,
                startValue,
                options.CurrentVelocity.x,
                endValue,
                out newVelocity,
                options.TargetVelocity.x,
                options.DampingRatio,
                options.Stiffness
            );
            options.CurrentVelocity.x = (float)newVelocity;
            startValue = (float)newPosition;
            return (float)newPosition;
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
            float4 f4StartValue = new float4(startValue, 0, 0);
            float4 f4EndValue = new float4(endValue, 0, 0);
            float4 f4CurrentVelocity = new float4(options.CurrentVelocity.x, options.CurrentVelocity.y, 0, 0);
            float4 f4TargetVelocity = new float4(options.TargetVelocity.x, options.TargetVelocity.y, 0, 0);
            
            // 使用float4版本的SpringElastic
            float4 f4NewPosition = SpringUtility.SpringElastic(
                deltaTime,
                f4StartValue,
                f4CurrentVelocity,
                f4EndValue,
                out float4 f4NewVelocity,
                f4TargetVelocity,
                options.DampingRatio,
                options.Stiffness
            );
            
            // 更新速度和位置（通过ref修改，不使用new）
            options.CurrentVelocity.x = f4NewVelocity.x;
            options.CurrentVelocity.y = f4NewVelocity.y;
            startValue.x = f4NewPosition.x;
            startValue.y = f4NewPosition.y;
            
            return startValue;
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
            float4 f4StartValue = new float4(startValue, 0);
            float4 f4EndValue = new float4(endValue, 0);
            float4 f4CurrentVelocity = new float4(options.CurrentVelocity.x, options.CurrentVelocity.y, options.CurrentVelocity.z, 0);
            float4 f4TargetVelocity = new float4(options.TargetVelocity.x, options.TargetVelocity.y, options.TargetVelocity.z, 0);
            
            // 使用float4版本的SpringElastic
            float4 f4NewPosition = SpringUtility.SpringElastic(
                deltaTime,
                f4StartValue,
                f4CurrentVelocity,
                f4EndValue,
                out float4 f4NewVelocity,
                f4TargetVelocity,
                options.DampingRatio,
                options.Stiffness
            );
            
            // 更新速度和位置（通过ref修改，不使用new）
            options.CurrentVelocity.x = f4NewVelocity.x;
            options.CurrentVelocity.y = f4NewVelocity.y;
            options.CurrentVelocity.z = f4NewVelocity.z;
            startValue.x = f4NewPosition.x;
            startValue.y = f4NewPosition.y;
            startValue.z = f4NewPosition.z;
            
            return startValue;
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
            float4 f4StartValue = startValue;
            float4 f4EndValue = endValue;
            float4 f4CurrentVelocity = options.CurrentVelocity;
            float4 f4TargetVelocity = options.TargetVelocity;
            
            // 使用float4版本的SpringElastic
            float4 f4NewPosition = SpringUtility.SpringElastic(
                deltaTime,
                f4StartValue,
                f4CurrentVelocity,
                f4EndValue,
                out float4 f4NewVelocity,
                f4TargetVelocity,
                options.DampingRatio,
                options.Stiffness
            );
            
            // 更新速度和位置（通过ref修改，不使用new）
            options.CurrentVelocity.x = f4NewVelocity.x;
            options.CurrentVelocity.y = f4NewVelocity.y;
            options.CurrentVelocity.z = f4NewVelocity.z;
            options.CurrentVelocity.w = f4NewVelocity.w;
            startValue.x = f4NewPosition.x;
            startValue.y = f4NewPosition.y;
            startValue.z = f4NewPosition.z;
            startValue.w = f4NewPosition.w;
            
            return startValue;
        }
    }
}
