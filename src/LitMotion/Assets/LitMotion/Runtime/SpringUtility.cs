using System;
using Unity.Burst;
using Unity.Burst.CompilerServices;
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
        public static void SpringSimple(
            in float deltaTime,
            ref float currentValue,
            ref float currentVelocity,
            in float targetValue,
            in float stiffness = 10.0f)
        {
            // SpringSimple是专门为临界阻尼设计的简化版本
            // 直接使用stiffness作为自然频率，阻尼系数的一半等于自然频率
            float naturalFreq = stiffness;

            // 临界阻尼的计算逻辑
            float displacementFromTarget = currentValue - targetValue;  // 当前位置与目标的位移差
            float velocityWithDamping = currentVelocity + displacementFromTarget * naturalFreq;  // 考虑阻尼的初始速度
            float exponentialDecay = FastNegExp(naturalFreq * deltaTime); // 指数衰减因子

            // 临界阻尼的更新公式
            currentValue = exponentialDecay * (displacementFromTarget + velocityWithDamping * deltaTime) + targetValue;  // 新位置
            currentVelocity = exponentialDecay * (currentVelocity - velocityWithDamping * naturalFreq * deltaTime);    // 新速度
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
        /// <returns>新的位移值</returns>
        public static void SpringElastic(
            in float deltaTime,
            ref float currentValue,
            ref float currentVelocity,
            in float targetValue,
            in float targetVelocity = 0.0f,
            in float dampingRatio = 0.5f,
            in float stiffness = 10.0f)
        {
            if (Hint.Unlikely(Approximately(currentValue, targetValue)))
            {
                currentValue = targetValue;
                currentVelocity = 0.0f;
                return;
            }
            // 使用有意义的变量名，提高代码可读性
            float targetPosition = targetValue;
            float targetVel = targetVelocity;
            // 将stiffness参数重命名为naturalFreq进行内部计算
            float naturalFreq = stiffness;
            float stiffnessValue = naturalFreq * naturalFreq;  // 刚度值 = naturalFreq²
            float dampingHalf = dampingRatio * naturalFreq;
            float dampingCoeff = 2.0f * dampingHalf;  // 阻尼系数 = 2 * 阻尼比 * naturalFreq
            float adjustedTargetPosition = targetPosition + (dampingCoeff * targetVel) / stiffnessValue;

            if (Math.Abs(dampingRatio - 1.0f) < 1e-5f) // Critically Damped
            {
                float initialDisplacement = currentValue - adjustedTargetPosition;
                float initialVelocityWithDamping = currentVelocity + initialDisplacement * dampingHalf;

                float exponentialDecay = FastNegExp(dampingHalf * deltaTime);

                currentValue = initialDisplacement * exponentialDecay + deltaTime * initialVelocityWithDamping * exponentialDecay + adjustedTargetPosition;
                currentVelocity = -dampingHalf * initialDisplacement * exponentialDecay - dampingHalf * deltaTime * initialVelocityWithDamping * exponentialDecay + initialVelocityWithDamping * exponentialDecay;
            }
            else if (dampingRatio < 1.0f) // Under Damped
            {
                float dampedFrequency = math.sqrt(stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0f);
                float displacementFromTarget = currentValue - adjustedTargetPosition;
                float amplitude = math.sqrt(FastSquare(currentVelocity + dampingHalf * displacementFromTarget) / (dampedFrequency * dampedFrequency) + FastSquare(displacementFromTarget));
                float phase = FastAtan((currentVelocity + displacementFromTarget * dampingHalf) / (-displacementFromTarget * dampedFrequency));

                amplitude = displacementFromTarget > 0.0f ? amplitude : -amplitude;

                float exponentialDecay = FastNegExp(dampingHalf * deltaTime);

                currentValue = amplitude * exponentialDecay * math.cos(dampedFrequency * deltaTime + phase) + adjustedTargetPosition;
                currentVelocity = -dampingHalf * amplitude * exponentialDecay * math.cos(dampedFrequency * deltaTime + phase) - dampedFrequency * amplitude * exponentialDecay * math.sin(dampedFrequency * deltaTime + phase);
            }
            else // Over Damped (dampingRatio > 1.0f)
            {
                float fastDecayRate = (dampingCoeff + math.sqrt(dampingCoeff * dampingCoeff - 4f * stiffnessValue)) / 2.0f;
                float slowDecayRate = (dampingCoeff - math.sqrt(dampingCoeff * dampingCoeff - 4f * stiffnessValue)) / 2.0f;
                // 计算过阻尼系数：fastDecayCoeff对应fastDecayRate，slowDecayCoeff对应slowDecayRate
                float fastDecayCoeff = (adjustedTargetPosition * fastDecayRate - currentValue * fastDecayRate - currentVelocity) / (slowDecayRate - fastDecayRate);
                float slowDecayCoeff = currentValue - fastDecayCoeff - adjustedTargetPosition;

                float fastExponentialDecay = FastNegExp(fastDecayRate * deltaTime);
                float slowExponentialDecay = FastNegExp(slowDecayRate * deltaTime);

                // 过阻尼位置更新：slowDecayCoeff用fastExponentialDecay，fastDecayCoeff用slowExponentialDecay
                currentValue = slowDecayCoeff * fastExponentialDecay + fastDecayCoeff * slowExponentialDecay + adjustedTargetPosition;
                currentVelocity = -fastDecayRate * slowDecayCoeff * fastExponentialDecay - slowDecayRate * fastDecayCoeff * slowExponentialDecay;
            }
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
        /// <returns>新的位移值</returns>
        public static void SpringPrecise(
            in float deltaTime,
            ref float currentValue,
            ref float currentVelocity,
            in float targetValue,
            in float targetVelocity = 0.0f,
            in float dampingRatio = 0.5f,
            in float stiffness = 10.0f)
        {
            // 将stiffness参数重命名为naturalFreq进行内部计算
            float naturalFreq = stiffness;

            // 根据targetVelocity调整目标位置（类似SpringElastic的逻辑）
            float adjustedTargetPosition = targetValue;
            if (math.abs(targetVelocity) > 1e-5f)
            {
                // 计算调整后的目标位置，使系统在到达目标时具有指定速度
                // 使用与SpringElastic相同的逻辑：c = g + (d * q) / s
                // 其中 d = 2 * dampingRatio * naturalFreq, s = naturalFreq^2
                float dampingCoeff = 2.0f * dampingRatio * naturalFreq;
                float stiffnessValue = naturalFreq * naturalFreq;
                adjustedTargetPosition = targetValue + (dampingCoeff * targetVelocity) / stiffnessValue;
            }

            float adjustedDisplacement = currentValue - adjustedTargetPosition;
            float dampingRatioSquared = dampingRatio * dampingRatio;
            float r = -dampingRatio * naturalFreq;

            float displacement;
            float calculatedVelocity;

            if (dampingRatio > 1)
            {
                // 过阻尼
                float s = naturalFreq * math.sqrt(dampingRatioSquared - 1);
                float gammaPlus = r + s;
                float gammaMinus = r - s;

                // 过阻尼计算
                float coeffB = (gammaMinus * adjustedDisplacement - currentVelocity) / (gammaMinus - gammaPlus);
                float coeffA = adjustedDisplacement - coeffB;
                displacement = coeffA * FastExp(gammaMinus * deltaTime) + coeffB * FastExp(gammaPlus * deltaTime);
                calculatedVelocity = coeffA * gammaMinus * FastExp(gammaMinus * deltaTime) +
                                    coeffB * gammaPlus * FastExp(gammaPlus * deltaTime);
            }
            else if (math.abs(dampingRatio - 1.0f) < 1e-5f)
            {
                // 临界阻尼
                float coeffA = adjustedDisplacement;
                float coeffB = currentVelocity + naturalFreq * adjustedDisplacement;
                float nFdT = -naturalFreq * deltaTime;
                displacement = (coeffA + coeffB * deltaTime) * FastExp(nFdT);
                calculatedVelocity = ((coeffA + coeffB * deltaTime) * FastExp(nFdT) * (-naturalFreq)) +
                                    coeffB * FastExp(nFdT);
            }
            else
            {
                // 欠阻尼
                float dampedFreq = naturalFreq * math.sqrt(1 - dampingRatioSquared);
                float cosCoeff = adjustedDisplacement;
                float sinCoeff = (1.0f / dampedFreq) * ((-r * adjustedDisplacement) + currentVelocity);
                float dFdT = dampedFreq * deltaTime;
                displacement = FastExp(r * deltaTime) * (cosCoeff * math.cos(dFdT) + sinCoeff * math.sin(dFdT));
                calculatedVelocity = displacement * r +
                                    (FastExp(r * deltaTime) *
                                     ((-dampedFreq * cosCoeff * math.sin(dFdT) +
                                       dampedFreq * sinCoeff * math.cos(dFdT))));
            }

            currentValue = displacement + adjustedTargetPosition;
            currentVelocity = calculatedVelocity;
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
        public static void SpringSimpleVelocitySmoothing(
            in float deltaTime,
            ref float currentValue,
            ref float currentVelocity,
            in float targetValue,
            ref float intermediatePosition,
            in float smothingVelocity = 2f,
            in float stiffness = 10.0f)
        {
            // 按照原始版本的设计，直接使用stiffness作为自然频率
            float naturalFreq = stiffness;

            // 计算目标与中间位置的差值
            float targetIntermediateDiff = targetValue - intermediatePosition;
            float absTargetIntermediateDiff = math.abs(targetIntermediateDiff);

            // 计算速度方向
            float velocityDirection = (targetIntermediateDiff > 0.0f ? 1.0f : -1.0f) * smothingVelocity;

            // 计算预期时间
            float anticipatedTime = 1f / naturalFreq;

            // 计算未来目标位置
            float futureTargetPosition = absTargetIntermediateDiff > anticipatedTime * smothingVelocity ?
                intermediatePosition + velocityDirection * anticipatedTime : targetValue;

            // 直接调用SpringSimple函数
            SpringSimple(deltaTime, ref currentValue, ref currentVelocity, futureTargetPosition, stiffness);

            // 更新中间位置
            intermediatePosition = absTargetIntermediateDiff > deltaTime * smothingVelocity ?
                intermediatePosition + velocityDirection * deltaTime : targetValue;
        }

        /// <summary>
        /// 时间限制弹簧阻尼器，根据指定到达时间自动计算合适的刚度
        /// </summary>
        /// <param name="deltaTime">时间步长（秒）</param>
        /// <param name="currentValue">当前值</param>
        /// <param name="currentVelocity">当前速度</param>
        /// <param name="targetValue">目标值</param>
        /// <param name="newVelocity">输出新的速度</param>
        /// <param name="intermediatePosition">中间位置（用于维护状态）</param>
        /// <param name="durationSeconds">目标到达时间（秒）</param>
        /// <returns>新的位移值</returns>
        public static void SpringSimpleDurationLimit(
            in float deltaTime,
            ref float currentValue,
            ref float currentVelocity,
            in float targetValue,
            in float durationSeconds = 0.2f)
        {
            // 根据目标时间计算合适的自然频率
            // 对于临界阻尼系统，收敛时间约为 3-4 倍的时间常数
            // 时间常数 = 1 / naturalFreq，所以 naturalFreq = 4.6 / durationSeconds
            // 这样可以在durationSeconds时间内达到约99%收敛
            float naturalFreq = 4.6f / durationSeconds;

            // 直接使用目标值，不需要复杂的中间目标逻辑
            // 让弹簧直接收敛到最终目标
            SpringSimple(deltaTime, ref currentValue, ref currentVelocity, targetValue, naturalFreq);
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
        public static void SpringSimpleDoubleSmoothing(
            in float deltaTime,
            ref float currentValue,
            ref float currentVelocity,
            in float targetValue,
            ref float intermediatePosition,
            ref float intermediateVelocity,
            in float stiffness = 10.0f)
        {
            float floatStiffness = 2.0f * stiffness;

            // 第一层弹簧：从中间位置到目标位置
            // 直接修改intermediatePosition和intermediateVelocity
            SpringSimple(deltaTime, ref intermediatePosition, ref intermediateVelocity, targetValue, floatStiffness);

            // 第二层弹簧：从当前位置到中间位置
            // 使用修改后的intermediatePosition作为目标
            SpringSimple(deltaTime, ref currentValue, ref currentVelocity, intermediatePosition, floatStiffness);
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
            in float deltaTime,
            ref float4 currentValue,
            ref float4 currentVelocity,
            in float4 targetValue,
            in float stiffness = 1.0f)
        {
            // 检查是否已经收敛到目标值
            if (Hint.Unlikely(Approximately(currentValue, targetValue)))
            {
                currentValue = targetValue;
                currentVelocity = new float4(0.0f);
                return;
            }

            // SpringSimple是专门为临界阻尼设计的简化版本
            // 直接使用stiffness作为自然频率，阻尼系数的一半等于自然频率
            float naturalFreq = stiffness;

            // 临界阻尼的计算逻辑
            float4 displacementFromTarget = currentValue - targetValue;  // 当前位置与目标的位移差
            float4 velocityWithDamping = currentVelocity + displacementFromTarget * naturalFreq;  // 考虑阻尼的初始速度
            float4 exponentialDecay = (float4)FastNegExp(naturalFreq * deltaTime); // 指数衰减因子

            // 临界阻尼的更新公式
            currentValue = exponentialDecay * (displacementFromTarget + velocityWithDamping * deltaTime) + targetValue;  // 新位置
            currentVelocity = exponentialDecay * (currentVelocity - velocityWithDamping * naturalFreq * deltaTime);    // 新速度
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
        /// <returns>新的位移值</returns>
        [BurstCompile]
        public static void SpringElastic(
            in float deltaTime,
            ref float4 currentValue,
            ref float4 currentVelocity,
            in float4 targetValue,
            in float4 targetVelocity,
            in float dampingRatio = 0.5f,
            in float stiffness = 10.0f)
        {
            // 检查是否已经收敛到目标值
            if (Hint.Unlikely(Approximately(currentValue, targetValue)))
            {
                currentValue = targetValue;
                currentVelocity = new float4(0.0f);
                return;
            }

            // 将stiffness参数重命名为naturalFreq进行内部计算
            float naturalFreq = stiffness;
            float stiffnessValue = naturalFreq * naturalFreq;  // 刚度值 = naturalFreq²
            float dampingHalf = dampingRatio * naturalFreq;
            float dampingCoeff = 2.0f * dampingHalf;  // 阻尼系数 = 2 * 阻尼比 * naturalFreq
            float4 adjustedtargetValue = targetValue + (dampingCoeff * targetVelocity) / stiffnessValue;

            if (math.abs(dampingRatio - 1.0f) < 1e-5f) // Critically Damped
            {
                float4 initialDisplacement = currentValue - adjustedtargetValue;
                float4 initialVelocityWithDamping = currentVelocity + initialDisplacement * dampingHalf;

                float exponentialDecay = (float)FastNegExp((float)(dampingHalf * deltaTime));

                currentValue = initialDisplacement * exponentialDecay + deltaTime * initialVelocityWithDamping * exponentialDecay + adjustedtargetValue;
                currentVelocity = -dampingHalf * initialDisplacement * exponentialDecay - dampingHalf * deltaTime * initialVelocityWithDamping * exponentialDecay + initialVelocityWithDamping * exponentialDecay;
            }
            else if (dampingRatio < 1.0f) // Under Damped
            {
                float dampedFrequency = math.sqrt(stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0f);
                float4 displacementFromTarget = currentValue - adjustedtargetValue;
                float4 amplitude = math.sqrt(Square(currentVelocity + dampingHalf * displacementFromTarget) / (dampedFrequency * dampedFrequency) + Square(displacementFromTarget));
                float4 phase = FastAtan((currentVelocity + displacementFromTarget * dampingHalf) / (-displacementFromTarget * dampedFrequency));

                amplitude = math.select(-amplitude, amplitude, displacementFromTarget > 0.0f);

                float exponentialDecay = (float)FastNegExp((float)(dampingHalf * deltaTime));

                currentValue = amplitude * exponentialDecay * math.cos(dampedFrequency * deltaTime + phase) + adjustedtargetValue;
                currentVelocity = -dampingHalf * amplitude * exponentialDecay * math.cos(dampedFrequency * deltaTime + phase) - dampedFrequency * amplitude * exponentialDecay * math.sin(dampedFrequency * deltaTime + phase);
            }
            else // Over Damped (dampingRatio > 1.0f)
            {
                float fastDecayRate = (dampingCoeff + math.sqrt(dampingCoeff * dampingCoeff - 4f * stiffnessValue)) / 2.0f;
                float slowDecayRate = (dampingCoeff - math.sqrt(dampingCoeff * dampingCoeff - 4f * stiffnessValue)) / 2.0f;
                // 计算过阻尼系数：fastDecayCoeff对应fastDecayRate，slowDecayCoeff对应slowDecayRate
                float4 fastDecayCoeff = (adjustedtargetValue * fastDecayRate - currentValue * fastDecayRate - currentVelocity) / (slowDecayRate - fastDecayRate);
                float4 slowDecayCoeff = currentValue - fastDecayCoeff - adjustedtargetValue;

                float fastExponentialDecay = (float)FastNegExp((float)(fastDecayRate * deltaTime));
                float slowExponentialDecay = (float)FastNegExp((float)(slowDecayRate * deltaTime));

                // 过阻尼位置更新：slowDecayCoeff用fastExponentialDecay，fastDecayCoeff用slowExponentialDecay
                currentValue = slowDecayCoeff * fastExponentialDecay + fastDecayCoeff * slowExponentialDecay + adjustedtargetValue;
                currentVelocity = -fastDecayRate * slowDecayCoeff * fastExponentialDecay - slowDecayRate * fastDecayCoeff * slowExponentialDecay;
            }
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
            in float deltaTime,
            ref float4 currentValue,
            ref float4 currentVelocity,
            in float4 targetValue,
            ref float4 intermediatePosition,
            in float smothingVelocity = 2f,
            in float stiffness = 1.0f)
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
            SpringSimple(deltaTime, ref currentValue, ref currentVelocity, futureTargetPosition, stiffness);

            // 更新中间位置
            intermediatePosition = math.select(targetValue,
                intermediatePosition + velocityDirection * deltaTime,
                absTargetIntermediateDiff > deltaTime * smothingVelocity);

            // result已经在SpringSimple调用中设置
        }

        /// <summary>
        /// 时间限制弹簧阻尼器，float4版本，根据指定到达时间自动计算合适的刚度
        /// </summary>
        /// <param name="deltaTime">时间步长（秒）</param>
        /// <param name="currentValue">当前值</param>
        /// <param name="currentVelocity">当前速度</param>
        /// <param name="targetValue">目标值</param>
        /// <param name="newVelocity">输出新的速度</param>
        /// <param name="intermediatePosition">中间位置（用于维护状态）</param>
        /// <param name="durationSeconds">目标到达时间（秒）</param>
        /// <returns>新的位移值</returns>
        [BurstCompile]
        public static void SpringSimpleDurationLimit(
            in float deltaTime,
            ref float4 currentValue,
            ref float4 currentVelocity,
            ref float4 targetValue,
            in float durationSeconds = 0.2f)
        {
            // 根据目标时间计算合适的自然频率
            // 对于临界阻尼系统，收敛时间约为 3-4 倍的时间常数
            // 时间常数 = 1 / naturalFreq，所以 naturalFreq = 4.6 / durationSeconds
            // 这样可以在durationSeconds时间内达到约99%收敛
            float naturalFreq = 4.6f / durationSeconds;

            // 直接使用目标值，不需要复杂的中间目标逻辑
            // 让弹簧直接收敛到最终目标
            SpringSimple(deltaTime, ref currentValue, ref currentVelocity, targetValue, naturalFreq);
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
            in float deltaTime,
            ref float4 currentValue,
            ref float4 currentVelocity,
            in float4 targetValue,
            ref float4 intermediatePosition,
            ref float4 intermediateVelocity,
            in float stiffness = 1.0f)
        {
            float floatStiffness = 2.0f * stiffness;

            // 第一层弹簧：从中间位置到目标位置
            // 直接修改intermediatePosition和intermediateVelocity
            SpringSimple(deltaTime, ref intermediatePosition, ref intermediateVelocity, targetValue, floatStiffness);

            // 第二层弹簧：从当前位置到中间位置
            // 使用修改后的intermediatePosition作为目标
            SpringSimple(deltaTime, ref currentValue, ref currentVelocity, intermediatePosition, floatStiffness);
        }

        #endregion



        private static float HalfLifeToDamping(float halfLife, float eps = 1e-5f)
        {
            return (4.0f * 0.6931471805599453f) / (halfLife + eps);
        }

        private static float DampingToHalfLife(float damping, float eps = 1e-5f)
        {
            return (4.0f * 0.6931471805599453f) / (damping + eps);
        }

        private static float DampingRatioToStiffness(float ratio, float damping)
        {
            return Square(damping / (ratio * 2.0f));
        }

        private static float DampingRatioToDamping(float ratio, float stiffness)
        {
            return ratio * 2.0f * math.sqrt(stiffness);
        }

        private static float FrequencyToStiffness(float frequency)
        {
            return Square(2.0f * math.PI * frequency);
        }

        /// <summary>
        /// 将半衰期转换为自然频率（临界阻尼）
        /// </summary>
        /// <param name="halflife">半衰期（秒）</param>
        /// <returns>自然频率（rad/s）</returns>
        private static float HalflifeToNaturalFreq(float halflife)
        {
            return 0.69314718056f / halflife;  // ω₀ = ln(2) / τ₁/₂
        }

        /// <summary>
        /// 将自然频率转换为半衰期（临界阻尼）
        /// </summary>
        /// <param name="naturalFreq">自然频率（rad/s）</param>
        /// <returns>半衰期（秒）</returns>
        private static float NaturalFreqToHalflife(float naturalFreq)
        {
            return 0.69314718056f / naturalFreq;  // τ₁/₂ = ln(2) / ω₀
        }
        private static float FastNegExp(float x)
        {
            return 1.0f / (1.0f + x + 0.48f * x * x + 0.235f * x * x * x);
        }

        private static float Square(float x)
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
        private static float FastAtan(float x)
        {
            float z = math.abs(x);
            float w = z > 1.0f ? 1.0f / z : z;
            float y = (math.PI / 4.0f) * w - w * (w - 1) * (0.2447f + 0.0663f * w);
            return math.sign(x) * (z > 1.0f ? math.PI / 2.0f - y : y);
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
        private static float FastSquare(float x)
        {
            return x * x;
        }

        private static float FastExp(float x)
        {
            // 快速指数函数近似，适用于 Burst
            return 1.0f / (1.0f - x + 0.5f * x * x - 0.1667f * x * x * x);
        }
        /// <summary>
        /// 检查两个float值是否近似相等
        /// </summary>
        /// <param name="a">第一个值</param>
        /// <param name="b">第二个值</param>
        /// <param name="precision">精度阈值</param>
        /// <returns>如果近似相等则返回true</returns>
        [BurstCompile]
        public static bool Approximately(in float a, in float b, in float precision = 1e-5f)
        {
            return math.abs(b - a) < math.max(1E-06f * math.max(math.abs(a), math.abs(b)), precision);
        }

        /// <summary>
        /// 检查两个float4值是否近似相等
        /// </summary>
        /// <param name="a">第一个值</param>
        /// <param name="b">第二个值</param>
        /// <param name="precision">精度阈值</param>
        /// <returns>如果所有维度都近似相等则返回true</returns>
        [BurstCompile]
        public static bool Approximately(in float4 a, in float4 b, in float precision = 1e-5f)
        {
            return math.all(math.abs(b - a) < math.max(1E-06f * math.max(math.abs(a), math.abs(b)), precision));
        }
    }
}
