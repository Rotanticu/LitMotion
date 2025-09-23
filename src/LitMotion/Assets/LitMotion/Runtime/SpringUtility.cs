using System;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
namespace LitMotion
{
    [BurstCompile]
    public static class SpringUtility
    {
        /// <summary>
        /// 简易弹簧阻尼器，只有临界阻尼，没有过阻尼和欠阻尼
        /// </summary>
        /// <param name="timeElapsed">经过时间（毫秒）</param>
        /// <param name="currentValue">当前值</param>
        /// <param name="currentVelocity">当前速度</param>
        /// <param name="targetValue">目标值</param>
        /// <param name="newVelocity">输出新的速度</param>
        /// <param name="dampingRatio">阻尼比</param>
        /// <param name="stiffness">弹簧刚度（实际上直接作为自然频率使用）</param>
        /// <returns>新的位移值</returns>
        public static double SpringSimple(
            double deltaTime,
            double currentValue,
            double currentVelocity,
            double targetValue,
            out double newVelocity,
            double stiffness = 1.0d)
        {
            // SpringSimple是专门为临界阻尼设计的简化版本
            // 直接使用stiffness作为自然频率，阻尼系数的一半等于自然频率
            double naturalFreq = stiffness;

            // 临界阻尼的计算逻辑
            double displacementFromTarget = currentValue - targetValue;  // 当前位置与目标的位移差
            double velocityWithDamping = currentVelocity + displacementFromTarget * naturalFreq;  // 考虑阻尼的初始速度
            double exponentialDecay = FastNegExp(naturalFreq * deltaTime); // 指数衰减因子

            // 临界阻尼的更新公式
            double newPosition = exponentialDecay * (displacementFromTarget + velocityWithDamping * deltaTime) + targetValue;  // 新位置
            double newVel = exponentialDecay * (currentVelocity - velocityWithDamping * naturalFreq * deltaTime);    // 新速度

            newVelocity = newVel;
            return newPosition;
        }

        /// <summary>
        /// 近似弹性弹簧阻尼器，有过阻尼和欠阻尼和临界阻尼，采用高性能近似算法
        /// </summary>
        /// <param name="deltaTime">时间步长（秒）</param>
        /// <param name="currentValue">当前值</param>
        /// <param name="currentVelocity">当前速度</param>
        /// <param name="targetValue">目标值</param>
        /// <param name="newVelocity">输出新的速度</param>
        /// <param name="targetVelocity">目标速度（到达目标位置时的期望速度）</param>
        /// <param name="dampingRatio">阻尼比 0.6 = Q弹 1 = 临界 1.2 = 稍缓</param>
        /// <param name="stiffness">弹簧刚度（实际上直接作为自然频率使用）5 = 1秒 10 = 0.5秒 16.5 = 0.2秒 </param>
        /// <param name="precision">计算精度容差</param>
        /// <returns>新的位移值</returns>
        public static double SpringElastic(
            double deltaTime,
            double currentValue,
            double currentVelocity,
            double targetValue,
            out double newVelocity,
            double targetVelocity = 0.0d,
            double dampingRatio = 0.5f,
            double stiffness = 1.0d,
            double precision = 1e-5f)
        {
            // 使用有意义的变量名，提高代码可读性
            double currentPosition = currentValue;
            double currentVel = currentVelocity;
            double targetPosition = targetValue;
            double targetVel = targetVelocity;
            // 将stiffness参数重命名为naturalFreq进行内部计算
            double naturalFreq = stiffness;
            double stiffnessValue = naturalFreq * naturalFreq;  // 刚度值 = naturalFreq²
            double dampingHalf = dampingRatio * naturalFreq;
            double dampingCoeff = 2.0d * dampingHalf;  // 阻尼系数 = 2 * 阻尼比 * naturalFreq
            double adjustedTargetPosition = targetPosition + (dampingCoeff * targetVel) / (stiffnessValue + precision);

            if (Math.Abs(stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0d) < precision) // Critically Damped
            {
                double initialDisplacement = currentPosition - adjustedTargetPosition;
                double initialVelocityWithDamping = currentVel + initialDisplacement * dampingHalf;

                double exponentialDecay = FastNegExp(dampingHalf * deltaTime);

                currentPosition = initialDisplacement * exponentialDecay + deltaTime * initialVelocityWithDamping * exponentialDecay + adjustedTargetPosition;
                currentVel = -dampingHalf * initialDisplacement * exponentialDecay - dampingHalf * deltaTime * initialVelocityWithDamping * exponentialDecay + initialVelocityWithDamping * exponentialDecay;
            }
            else if (stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0d > 0.0) // Under Damped
            {
                double dampedFrequency = Math.Sqrt(stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0d);
                double displacementFromTarget = currentPosition - adjustedTargetPosition;
                double amplitude = Math.Sqrt(FastSquare(currentVel + dampingHalf * displacementFromTarget) / (dampedFrequency * dampedFrequency + precision) + FastSquare(displacementFromTarget));
                double phase = FastAtan((currentVel + displacementFromTarget * dampingHalf) / (-displacementFromTarget * dampedFrequency + precision));

                amplitude = displacementFromTarget > 0.0d ? amplitude : -amplitude;

                double exponentialDecay = FastNegExp(dampingHalf * deltaTime);

                currentPosition = amplitude * exponentialDecay * Math.Cos(dampedFrequency * deltaTime + phase) + adjustedTargetPosition;
                currentVel = -dampingHalf * amplitude * exponentialDecay * Math.Cos(dampedFrequency * deltaTime + phase) - dampedFrequency * amplitude * exponentialDecay * Math.Sin(dampedFrequency * deltaTime + phase);
            }
            else if (stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0d < 0.0) // Over Damped
            {
                double fastDecayRate = (dampingCoeff + Math.Sqrt(dampingCoeff * dampingCoeff - 4d * stiffnessValue)) / 2.0d;
                double slowDecayRate = (dampingCoeff - Math.Sqrt(dampingCoeff * dampingCoeff - 4d * stiffnessValue)) / 2.0d;
                // 计算过阻尼系数：fastDecayCoeff对应fastDecayRate，slowDecayCoeff对应slowDecayRate
                double fastDecayCoeff = (adjustedTargetPosition * fastDecayRate - currentPosition * fastDecayRate - currentVel) / (slowDecayRate - fastDecayRate);
                double slowDecayCoeff = currentPosition - fastDecayCoeff - adjustedTargetPosition;

                double fastExponentialDecay = FastNegExp(fastDecayRate * deltaTime);
                double slowExponentialDecay = FastNegExp(slowDecayRate * deltaTime);

                // 过阻尼位置更新：slowDecayCoeff用fastExponentialDecay，fastDecayCoeff用slowExponentialDecay
                currentPosition = slowDecayCoeff * fastExponentialDecay + fastDecayCoeff * slowExponentialDecay + adjustedTargetPosition;
                currentVel = -fastDecayRate * slowDecayCoeff * fastExponentialDecay - slowDecayRate * fastDecayCoeff * slowExponentialDecay;
            }

            newVelocity = (double)currentVel;
            return (double)currentPosition;
        }
        /// <summary>
        /// 精确弹簧阻尼器，有过阻尼和欠阻尼和临界阻尼，采用精确算法
        /// </summary>
        /// <param name="currentValue">上次位移</param>
        /// <param name="currentVelocity">当前速度</param>
        /// <param name="timeElapsed">经过时间（毫秒）</param>
        /// <param name="targetValue">目标位置</param>
        /// <param name="newVelocity">输出新的速度</param>
        /// <param name="dampingRatio">阻尼比</param>
        /// <param name="stiffness">弹簧刚度（实际上直接作为自然频率使用）</param>
        /// <param name="targetVelocity">目标速度（到达目标位置时的期望速度）</param>
        /// <param name="precision">计算精度容差</param>
        /// <returns>新的位移值</returns>
        public static double SpringPrecise(
            double deltaTime,
            double currentValue,
            double currentVelocity,
            double targetValue,
            out double newVelocity,
            double targetVelocity = 0.0d,
            double dampingRatio = 0.5d,
            double stiffness = 1.0d,
            double precision = 1e-5d)
        {
            // 将stiffness参数重命名为naturalFreq进行内部计算
            double naturalFreq = stiffness;

            // 根据targetVelocity调整目标位置（类似SpringElastic的逻辑）
            double adjustedTargetPosition = targetValue;
            if (Math.Abs(targetVelocity) > precision)
            {
                // 计算调整后的目标位置，使系统在到达目标时具有指定速度
                // 使用与SpringElastic相同的逻辑：c = g + (d * q) / (s + eps)
                // 其中 d = 2 * dampingRatio * naturalFreq, s = naturalFreq^2
                double dampingCoeff = 2.0d * dampingRatio * naturalFreq;
                double stiffnessValue = naturalFreq * naturalFreq;
                adjustedTargetPosition = targetValue + (dampingCoeff * targetVelocity) / (stiffnessValue + precision);
            }

            double adjustedDisplacement = currentValue - adjustedTargetPosition;
            double dampingRatioSquared = dampingRatio * dampingRatio;
            double r = -dampingRatio * naturalFreq;

            double displacement;
            double calculatedVelocity;

            if (dampingRatio > 1)
            {
                // 过阻尼
                double s = naturalFreq * Math.Sqrt(dampingRatioSquared - 1);
                double gammaPlus = r + s;
                double gammaMinus = r - s;

                // 过阻尼计算
                double coeffB = (gammaMinus * adjustedDisplacement - currentVelocity) / (gammaMinus - gammaPlus);
                double coeffA = adjustedDisplacement - coeffB;
                displacement = coeffA * Math.Exp(gammaMinus * deltaTime) + coeffB * Math.Exp(gammaPlus * deltaTime);
                calculatedVelocity = coeffA * gammaMinus * Math.Exp(gammaMinus * deltaTime) +
                                    coeffB * gammaPlus * Math.Exp(gammaPlus * deltaTime);
            }
            else if (Math.Abs(dampingRatio - 1.0d) < precision)
            {
                // 临界阻尼
                double coeffA = adjustedDisplacement;
                double coeffB = currentVelocity + naturalFreq * adjustedDisplacement;
                double nFdT = -naturalFreq * deltaTime;
                displacement = (coeffA + coeffB * deltaTime) * Math.Exp(nFdT);
                calculatedVelocity = ((coeffA + coeffB * deltaTime) * Math.Exp(nFdT) * (-naturalFreq)) +
                                    coeffB * Math.Exp(nFdT);
            }
            else
            {
                // 欠阻尼
                double dampedFreq = naturalFreq * Math.Sqrt(1 - dampingRatioSquared);
                double cosCoeff = adjustedDisplacement;
                double sinCoeff = (1.0 / dampedFreq) * ((-r * adjustedDisplacement) + currentVelocity);
                double dFdT = dampedFreq * deltaTime;
                displacement = Math.Exp(r * deltaTime) * (cosCoeff * Math.Cos(dFdT) + sinCoeff * Math.Sin(dFdT));
                calculatedVelocity = displacement * r +
                                    (Math.Exp(r * deltaTime) *
                                     ((-dampedFreq * cosCoeff * Math.Sin(dFdT) +
                                       dampedFreq * sinCoeff * Math.Cos(dFdT))));
            }

            double newValue = (double)(displacement + adjustedTargetPosition);
            newVelocity = (double)calculatedVelocity;
            return newValue;
        }
        /// <summary>
        /// 速度平滑弹簧阻尼器，支持目标速度的连续运动
        /// </summary>
        /// <param name="timeElapsed">经过时间（毫秒）</param>
        /// <param name="currentValue">当前值</param>
        /// <param name="currentVelocity">当前速度</param>
        /// <param name="targetValue">目标值</param>
        /// <param name="newVelocity">输出新的速度</param>
        /// <param name="intermediatePosition">中间位置（用于维护状态）</param>
        /// <param name="smothingVelocity">平滑速度（需要平滑的线性速度）</param>
        /// <param name="dampingRatio">阻尼比</param>
        /// <param name="stiffness">弹簧刚度（实际上直接作为自然频率使用）</param>
        /// <returns>新的位移值</returns>
        public static double SpringSimpleVelocitySmoothing(
            double deltaTime,
            double currentValue,
            double currentVelocity,
            double targetValue,
            out double newVelocity,
            ref double intermediatePosition,
            double smothingVelocity = 2d,
            double stiffness = 1.0d)
        {
            // 按照原始版本的设计，直接使用stiffness作为自然频率
            double naturalFreq = stiffness;

            // 计算目标与中间位置的差值
            double targetIntermediateDiff = targetValue - intermediatePosition;
            double absTargetIntermediateDiff = Math.Abs(targetIntermediateDiff);

            // 计算速度方向
            double velocityDirection = (targetIntermediateDiff > 0.0d ? 1.0d : -1.0d) * smothingVelocity;

            // 计算预期时间
            double anticipatedTime = 1d / naturalFreq;

            // 计算未来目标位置
            double futureTargetPosition = absTargetIntermediateDiff > anticipatedTime * smothingVelocity ?
                intermediatePosition + velocityDirection * anticipatedTime : targetValue;

            // 直接调用SpringSimple函数
            double result = SpringSimple(deltaTime, currentValue, currentVelocity, futureTargetPosition, out newVelocity, stiffness);

            // 更新中间位置
            intermediatePosition = absTargetIntermediateDiff > deltaTime * smothingVelocity ?
                intermediatePosition + velocityDirection * deltaTime : targetValue;

            return result;
        }

        /// <summary>
        /// 时间限制弹簧阻尼器，支持指定到达时间
        /// </summary>
        /// <param name="timeElapsed">经过时间（毫秒）</param>
        /// <param name="currentValue">当前值</param>
        /// <param name="currentVelocity">当前速度</param>
        /// <param name="targetValue">目标值</param>
        /// <param name="newVelocity">输出新的速度</param>
        /// <param name="intermediatePosition">中间位置（用于维护状态）</param>
        /// <param name="durationMillisecond">目标到达时间（毫秒）</param>
        /// <param name="dampingRatio">阻尼比</param>
        /// <param name="stiffness">弹簧刚度（实际上直接作为自然频率使用）</param>
        /// <param name="anticipation">预期系数，用于预测未来目标位置</param>
        /// <returns>新的位移值</returns>
        public static double SpringSimpleDurationLimit(
            double deltaTime,
            double currentValue,
            double currentVelocity,
            double targetValue,
            out double newVelocity,
            ref double intermediatePosition,
            double durationSeconds = 0.2d,
            double stiffness = 1.0d)
        {
            // 直接使用stiffness作为自然频率
            double naturalFreq = stiffness;
            double tGoal = durationSeconds;

            // 计算最小时间
            double minTime = tGoal > deltaTime ? tGoal : deltaTime;

            // 基于中间位置计算目标速度
            double targetVel = (targetValue - intermediatePosition) / minTime;

            // 计算预期时间
            double anticipatedTime = 1d / naturalFreq;

            // 计算未来目标位置
            double futureTargetPosition = anticipatedTime < tGoal ?
                intermediatePosition + targetVel * anticipatedTime : targetValue;

            // 直接调用SpringSimple函数
            double result = SpringSimple(deltaTime, currentValue, currentVelocity, futureTargetPosition, out newVelocity, stiffness);

            // 更新中间位置
            intermediatePosition += targetVel * deltaTime;

            return result;
        }

        /// <summary>
        /// 双重平滑弹簧阻尼器，使用两个串联的弹簧系统实现更平滑的运动
        /// </summary>
        /// <param name="timeElapsed">经过时间（毫秒）</param>
        /// <param name="currentValue">当前值</param>
        /// <param name="currentVelocity">当前速度</param>
        /// <param name="targetValue">目标值</param>
        /// <param name="newVelocity">输出新的速度</param>
        /// <param name="intermediatePosition">中间位置（用于维护状态）</param>
        /// <param name="intermediateVelocity">中间速度（用于维护状态）</param>
        /// <param name="dampingRatio">阻尼比</param>
        /// <param name="stiffness">弹簧刚度（实际上直接作为自然频率使用）</param>
        /// <returns>新的位移值</returns>
        public static double SpringSimpleDoubleSmoothing(
            double deltaTime,
            double currentValue,
            double currentVelocity,
            double targetValue,
            out double newVelocity,
            ref double intermediatePosition,
            ref double intermediateVelocity,
            double stiffness = 1.0d)
        {
            double doubleStiffness = 2.0d * stiffness;

            // 第一层弹簧：从中间位置到目标位置
            // 直接修改intermediatePosition和intermediateVelocity
            intermediatePosition = SpringSimple(deltaTime, intermediatePosition, intermediateVelocity, targetValue, out intermediateVelocity, doubleStiffness);

            // 第二层弹簧：从当前位置到中间位置
            // 使用修改后的intermediatePosition作为目标
            return SpringSimple(deltaTime, currentValue, currentVelocity, intermediatePosition, out newVelocity, doubleStiffness);
        }

        #region float4版本的Spring函数

        /// <summary>
        /// 简易弹簧阻尼器，只有临界阻尼，没有过阻尼和欠阻尼
        /// </summary>
        /// <param name="timeElapsed">经过时间（毫秒）</param>
        /// <param name="currentValue">当前值</param>
        /// <param name="currentVelocity">当前速度</param>
        /// <param name="targetValue">目标值</param>
        /// <param name="newVelocity">输出新的速度</param>
        /// <param name="dampingRatio">阻尼比</param>
        /// <param name="stiffness">弹簧刚度（实际上直接作为自然频率使用）</param>
        /// <returns>新的位移值</returns>
        [BurstCompile]
        public static void SpringSimple(
            float deltaTime,
            ref float4 currentValue,
            ref float4 currentVelocity,
            ref float4 targetValue,
            ref float4 newVelocity,
            ref float4 result,
            float stiffness = 1.0f)
        {
            // SpringSimple是专门为临界阻尼设计的简化版本
            // 直接使用stiffness作为自然频率，阻尼系数的一半等于自然频率
            float naturalFreq = stiffness;

            // 临界阻尼的计算逻辑
            float4 displacementFromTarget = currentValue - targetValue;  // 当前位置与目标的位移差
            float4 velocityWithDamping = currentVelocity + displacementFromTarget * naturalFreq;  // 考虑阻尼的初始速度
            float4 exponentialDecay = (float4)FastNegExp(naturalFreq * deltaTime); // 指数衰减因子

            // 临界阻尼的更新公式
            result = exponentialDecay * (displacementFromTarget + velocityWithDamping * deltaTime) + targetValue;  // 新位置
            newVelocity = exponentialDecay * (currentVelocity - velocityWithDamping * naturalFreq * deltaTime);    // 新速度
        }

        /// <summary>
        /// 近似弹性弹簧阻尼器，float4版本，有过阻尼和欠阻尼和临界阻尼，采用高性能近似算法
        /// </summary>
        /// <param name="deltaTime">时间步长（秒）</param>
        /// <param name="currentValue">当前值</param>
        /// <param name="currentVelocity">当前速度</param>
        /// <param name="targetValue">目标值</param>
        /// <param name="newVelocity">输出新的速度</param>
        /// <param name="targetVelocity">目标速度（到达目标位置时的期望速度）</param>
        /// <param name="dampingRatio">阻尼比 0.6 = Q弹 1 = 临界 1.2 = 稍缓</param>
        /// <param name="stiffness">弹簧刚度（实际上直接作为自然频率使用）5 = 1秒 10 = 0.5秒 16.5 = 0.2秒 </param>
        /// <param name="precision">计算精度容差</param>
        /// <returns>新的位移值</returns>
        [BurstCompile]
        public static void SpringElastic(
            float deltaTime,
            ref float4 currentValue,
            ref float4 currentVelocity,
            ref float4 targetValue,
            ref float4 newVelocity,
            ref float4 targetVelocity,
            ref float4 result,
            float dampingRatio = 0.5f,
            float stiffness = 1.0f,
            float precision = 1e-5f)
        {
            // 使用有意义的变量名，提高代码可读性
            float4 currentPosition = currentValue;
            float4 currentVel = currentVelocity;
            float4 targetPosition = targetValue;
            float4 targetVel = targetVelocity;
            // 将stiffness参数重命名为naturalFreq进行内部计算
            float naturalFreq = stiffness;
            float stiffnessValue = naturalFreq * naturalFreq;  // 刚度值 = naturalFreq²
            float dampingHalf = dampingRatio * naturalFreq;
            float dampingCoeff = 2.0f * dampingHalf;  // 阻尼系数 = 2 * 阻尼比 * naturalFreq
            float4 adjustedTargetPosition = targetPosition + (dampingCoeff * targetVel) / (stiffnessValue + precision);

            if (math.abs(stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0f) < precision) // Critically Damped
            {
                float4 initialDisplacement = currentPosition - adjustedTargetPosition;
                float4 initialVelocityWithDamping = currentVel + initialDisplacement * dampingHalf;

                float exponentialDecay = (float)FastNegExp((double)(dampingHalf * deltaTime));

                currentPosition = initialDisplacement * exponentialDecay + deltaTime * initialVelocityWithDamping * exponentialDecay + adjustedTargetPosition;
                currentVel = -dampingHalf * initialDisplacement * exponentialDecay - dampingHalf * deltaTime * initialVelocityWithDamping * exponentialDecay + initialVelocityWithDamping * exponentialDecay;
            }
            else if (stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0f > 0.0f) // Under Damped
            {
                float dampedFrequency = math.sqrt(stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0f);
                float4 displacementFromTarget = currentPosition - adjustedTargetPosition;
                float4 amplitude = math.sqrt(Square(currentVel + dampingHalf * displacementFromTarget) / (dampedFrequency * dampedFrequency + precision) + Square(displacementFromTarget));
                float4 phase = FastAtan((currentVel + displacementFromTarget * dampingHalf) / (-displacementFromTarget * dampedFrequency + precision));

                amplitude = math.select(-amplitude, amplitude, displacementFromTarget > 0.0f);

                float exponentialDecay = (float)FastNegExp((double)(dampingHalf * deltaTime));

                currentPosition = amplitude * exponentialDecay * math.cos(dampedFrequency * deltaTime + phase) + adjustedTargetPosition;
                currentVel = -dampingHalf * amplitude * exponentialDecay * math.cos(dampedFrequency * deltaTime + phase) - dampedFrequency * amplitude * exponentialDecay * math.sin(dampedFrequency * deltaTime + phase);
            }
            else if (stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0f < 0.0f) // Over Damped
            {
                float fastDecayRate = (dampingCoeff + math.sqrt(dampingCoeff * dampingCoeff - 4f * stiffnessValue)) / 2.0f;
                float slowDecayRate = (dampingCoeff - math.sqrt(dampingCoeff * dampingCoeff - 4f * stiffnessValue)) / 2.0f;
                // 计算过阻尼系数：fastDecayCoeff对应fastDecayRate，slowDecayCoeff对应slowDecayRate
                float4 fastDecayCoeff = (adjustedTargetPosition * fastDecayRate - currentPosition * fastDecayRate - currentVel) / (slowDecayRate - fastDecayRate);
                float4 slowDecayCoeff = currentPosition - fastDecayCoeff - adjustedTargetPosition;

                float fastExponentialDecay = (float)FastNegExp((double)(fastDecayRate * deltaTime));
                float slowExponentialDecay = (float)FastNegExp((double)(slowDecayRate * deltaTime));

                // 过阻尼位置更新：slowDecayCoeff用fastExponentialDecay，fastDecayCoeff用slowExponentialDecay
                currentPosition = slowDecayCoeff * fastExponentialDecay + fastDecayCoeff * slowExponentialDecay + adjustedTargetPosition;
                currentVel = -fastDecayRate * slowDecayCoeff * fastExponentialDecay - slowDecayRate * fastDecayCoeff * slowExponentialDecay;
            }

            newVelocity = currentVel;
            result = currentPosition;
        }

        /// <summary>
        /// 速度平滑弹簧阻尼器，float4版本，支持指定到达时间
        /// </summary>
        /// <param name="deltaTime">时间步长（秒）</param>
        /// <param name="currentValue">当前值</param>
        /// <param name="currentVelocity">当前速度</param>
        /// <param name="targetValue">目标值</param>
        /// <param name="newVelocity">输出新的速度</param>
        /// <param name="intermediatePosition">中间位置（用于维护状态）</param>
        /// <param name="smothingVelocity">平滑速度</param>
        /// <param name="stiffness">弹簧刚度（实际上直接作为自然频率使用）</param>
        /// <returns>新的位移值</returns>
        [BurstCompile]
        public static void SpringSimpleVelocitySmoothing(
            float deltaTime,
            ref float4 currentValue,
            ref float4 currentVelocity,
            ref float4 targetValue,
            ref float4 newVelocity,
            ref float4 intermediatePosition,
            ref float4 result,
            float smothingVelocity = 2f,
            float stiffness = 1.0f)
        {
            // 按照原始版本的设计，直接使用stiffness作为自然频率
            float naturalFreq = stiffness;

            // 计算目标与中间位置的差值
            float4 targetIntermediateDiff = targetValue - intermediatePosition;
            float4 absTargetIntermediateDiff = math.abs(targetIntermediateDiff);

            // 计算速度方向
            float4 velocityDirection = math.sign(targetIntermediateDiff) * smothingVelocity;

            // 计算预期时间
            float anticipatedTime = 1f / naturalFreq;

            // 计算未来目标位置
            float4 futureTargetPosition = math.select(targetValue,
                intermediatePosition + velocityDirection * anticipatedTime,
                absTargetIntermediateDiff > anticipatedTime * smothingVelocity);

            // 直接调用SpringSimple函数
            SpringSimple(deltaTime, ref currentValue, ref currentVelocity, ref futureTargetPosition, ref newVelocity, ref result, stiffness);

            // 更新中间位置
            intermediatePosition = math.select(targetValue,
                intermediatePosition + velocityDirection * deltaTime,
                absTargetIntermediateDiff > deltaTime * smothingVelocity);

            // result已经在SpringSimple调用中设置
        }

        /// <summary>
        /// 时间限制弹簧阻尼器，float4版本，支持指定到达时间
        /// </summary>
        /// <param name="deltaTime">时间步长（秒）</param>
        /// <param name="currentValue">当前值</param>
        /// <param name="currentVelocity">当前速度</param>
        /// <param name="targetValue">目标值</param>
        /// <param name="newVelocity">输出新的速度</param>
        /// <param name="intermediatePosition">中间位置（用于维护状态）</param>
        /// <param name="durationSeconds">目标到达时间（秒）</param>
        /// <param name="stiffness">弹簧刚度（实际上直接作为自然频率使用）</param>
        /// <returns>新的位移值</returns>
        [BurstCompile]
        public static void SpringSimpleDurationLimit(
            float deltaTime,
            ref float4 currentValue,
            ref float4 currentVelocity,
            ref float4 targetValue,
            ref float4 newVelocity,
            ref float4 intermediatePosition,
            ref float4 result,
            float durationSeconds = 0.2f,
            float stiffness = 1.0f)
        {
            // 直接使用stiffness作为自然频率
            float naturalFreq = stiffness;
            float tGoal = durationSeconds;

            // 计算最小时间
            float minTime = math.max(tGoal, deltaTime);

            // 基于中间位置计算目标速度
            float4 targetVel = (targetValue - intermediatePosition) / minTime;

            // 计算预期时间
            float anticipatedTime = 1f / naturalFreq;

            // 计算未来目标位置
            float4 futureTargetPosition = math.select(targetValue,
                intermediatePosition + targetVel * anticipatedTime,
                anticipatedTime < tGoal);

            // 直接调用SpringSimple函数
            SpringSimple(deltaTime, ref currentValue, ref currentVelocity, ref futureTargetPosition, ref newVelocity, ref result, stiffness);

            // 更新中间位置
            intermediatePosition += targetVel * deltaTime;

            // result已经在SpringSimple调用中设置
        }

        /// <summary>
        /// 双重平滑弹簧阻尼器，float4版本，使用两个串联的弹簧系统实现更平滑的运动
        /// </summary>
        /// <param name="deltaTime">时间步长（秒）</param>
        /// <param name="currentValue">当前值</param>
        /// <param name="currentVelocity">当前速度</param>
        /// <param name="targetValue">目标值</param>
        /// <param name="newVelocity">输出新的速度</param>
        /// <param name="intermediatePosition">中间位置（用于维护状态）</param>
        /// <param name="intermediateVelocity">中间速度（用于维护状态）</param>
        /// <param name="stiffness">弹簧刚度（实际上直接作为自然频率使用）</param>
        /// <returns>新的位移值</returns>
        [BurstCompile]
        public static void SpringSimpleDoubleSmoothing(
            float deltaTime,
            ref float4 currentValue,
            ref float4 currentVelocity,
            ref float4 targetValue,
            ref float4 newVelocity,
            ref float4 intermediatePosition,
            ref float4 intermediateVelocity,
            ref float4 result,
            float stiffness = 1.0f)
        {
            float doubleStiffness = 2.0f * stiffness;
            
            // 第一层弹簧：从中间位置到目标位置
            // 直接修改intermediatePosition和intermediateVelocity
            SpringSimple(deltaTime, ref intermediatePosition, ref intermediateVelocity, ref targetValue, ref intermediateVelocity, ref intermediatePosition, doubleStiffness);

            // 第二层弹簧：从当前位置到中间位置
            // 使用修改后的intermediatePosition作为目标
            SpringSimple(deltaTime, ref currentValue, ref currentVelocity, ref intermediatePosition, ref newVelocity, ref result, doubleStiffness);
        }

        #endregion



        private static double HalfLifeToDamping(double halfLife, double eps = 1e-5d)
        {
            return (4.0d * 0.6931471805599453d) / (halfLife + eps);
        }

        private static double DampingToHalfLife(double damping, double eps = 1e-5d)
        {
            return (4.0d * 0.6931471805599453d) / (damping + eps);
        }

        private static double DampingRatioToStiffness(double ratio, double damping)
        {
            return Square(damping / (ratio * 2.0d));
        }

        private static double DampingRatioToDamping(double ratio, double stiffness)
        {
            return ratio * 2.0d * Math.Sqrt(stiffness);
        }

        private static double FrequencyToStiffness(double frequency)
        {
            return Square(2.0d * Math.PI * frequency);
        }

        /// <summary>
        /// 将半衰期转换为自然频率（临界阻尼）
        /// </summary>
        /// <param name="halflife">半衰期（秒）</param>
        /// <returns>自然频率（rad/s）</returns>
        private static double HalflifeToNaturalFreq(double halflife)
        {
            return 0.69314718056d / halflife;  // ω₀ = ln(2) / τ₁/₂
        }

        /// <summary>
        /// 将自然频率转换为半衰期（临界阻尼）
        /// </summary>
        /// <param name="naturalFreq">自然频率（rad/s）</param>
        /// <returns>半衰期（秒）</returns>
        private static double NaturalFreqToHalflife(double naturalFreq)
        {
            return 0.69314718056d / naturalFreq;  // τ₁/₂ = ln(2) / ω₀
        }
        private static double FastNegExp(double x)
        {
            return 1.0d / (1.0d + x + 0.48d * x * x + 0.235d * x * x * x);
        }

        private static double Square(double x)
        {
            return x * x;
        }

        private static float4 Square(float4 x)
        {
            return x * x;
        }

        /// <summary>
        /// 快速反正切函数近似，比Math.Atan快2-5倍，精度损失很小
        /// </summary>
        private static double FastAtan(double x)
        {
            double z = Math.Abs(x);
            double w = z > 1.0d ? 1.0d / z : z;
            double y = (Math.PI / 4.0d) * w - w * (w - 1) * (0.2447d + 0.0663d * w);
            return Math.Sign(x) * (z > 1.0d ? Math.PI / 2.0d - y : y);
        }

        /// <summary>
        /// 快速反正切函数近似，float4版本
        /// </summary>
        private static float4 FastAtan(float4 x)
        {
            float4 z = math.abs(x);
            float4 w = math.select(1.0f / z, z, z <= 1.0f);
            float4 y = (math.PI / 4.0f) * w - w * (w - 1.0f) * (0.2447f + 0.0663f * w);
            return math.sign(x) * math.select(math.PI / 2.0f - y, y, z <= 1.0f);
        }

        /// <summary>
        /// 快速平方函数，与直接乘法性能相同
        /// </summary>
        private static double FastSquare(double x)
        {
            return x * x;
        }

        /// <summary>
        /// 根据纳秒时间计算弹簧动画值
        /// </summary>
        /// <param name="playTimeNanos">播放时间（纳秒）</param>
        /// <param name="initialValue">初始值</param>
        /// <param name="targetValue">目标值</param>
        /// <param name="initialVelocity">初始速度</param>
        /// <param name="dampingRatio">阻尼比</param>
        /// <param name="stiffness">弹簧刚度（实际上直接作为自然频率使用）</param>
        /// <param name="targetVelocity">目标速度（到达目标位置时的期望速度）</param>
        /// <param name="precision">计算精度容差</param>
        /// <returns>当前动画值</returns>
        public static double GetSpringValueAndVelocityFromNanos(
            long playTimeNanos,
            double initialValue,
            double targetValue,
            double initialVelocity,
            double dampingRatio = 0.5f,
            double stiffness = 1.0d,
            double targetVelocity = 0.0d,
            double precision = 1e-5f)
        {
            // TODO: 在弹簧实现中正确支持纳秒
            long playTimeMillis = playTimeNanos / 1_000_000L;
            return SpringPrecise(playTimeMillis, initialValue, initialVelocity, targetValue, out _, targetVelocity, dampingRatio, stiffness, precision);
        }
    }
}
