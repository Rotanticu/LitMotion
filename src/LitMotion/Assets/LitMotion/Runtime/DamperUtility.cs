using System;
namespace LitMotion
{
    //https://github.com/orangeduck/Spring-It-On
    /// <summary>
    /// �ṩ�����ֵ�����ʵ�ó����ࡣ
    /// </summary>
    public static class DamperUtility
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
        public static float SpringSimple(
            long timeElapsed,
            float currentValue,
            float currentVelocity,
            float targetValue,
            out float newVelocity,
            float dampingRatio = 0.5f,
            float stiffness = 1.0f)
        {
            // 将stiffness参数重命名为naturalFreq进行内部计算
            float naturalFreq = stiffness;
            
            // 参数映射到更有意义的变量名，保持计算逻辑不变
            double currentPosition = currentValue;
            double currentVel = currentVelocity;
            double targetPosition = targetValue;
            double deltaTime = timeElapsed / 1000.0; // 转换为秒
            
            // 从stiffness和dampingRatio计算弹簧物理参数
            // 保持与原始SpringSimple相同的计算逻辑
            double halfLife = DampingToHalfLife(2.0 * dampingRatio * naturalFreq); // 从阻尼系数计算半衰期
            double dampingCoeff = HalfLifeToDamping(halfLife); // 阻尼系数
            double dampingHalf = dampingCoeff / 2.0d; // 阻尼系数的一半

            // 临界阻尼计算（SpringSimple只支持临界阻尼）
            double initialDisplacement = currentPosition - targetPosition;
            double initialVelocityWithDamping = currentVel + initialDisplacement * dampingHalf;
            double exponentialDecay = FastNegExp(dampingHalf * deltaTime);

            // 更新位置和速度
            currentPosition = exponentialDecay * (initialDisplacement + initialVelocityWithDamping * deltaTime) + targetPosition;
            currentVel = exponentialDecay * (currentVel - initialVelocityWithDamping * dampingHalf * deltaTime);

            newVelocity = (float)currentVel;
            return (float)currentPosition;
        }

        /// <summary>
        /// 近似弹性弹簧阻尼器，有过阻尼和欠阻尼和临界阻尼，采用高性能近似算法
        /// </summary>
        /// <param name="timeElapsed">经过时间（毫秒）</param>
        /// <param name="currentValue">当前值</param>
        /// <param name="currentVelocity">当前速度</param>
        /// <param name="targetValue">目标值</param>
        /// <param name="newVelocity">输出新的速度</param>
        /// <param name="targetVelocity">目标速度（到达目标位置时的期望速度）</param>
        /// <param name="dampingRatio">阻尼比</param>
        /// <param name="stiffness">弹簧刚度（实际上直接作为自然频率使用）</param>
        /// <param name="precision">计算精度容差</param>
        /// <returns>新的位移值</returns>
        public static float SpringElastic(
            long timeElapsed,
            float currentValue,
            float currentVelocity,
            float targetValue,
            out float newVelocity,
            float targetVelocity = 0.0f,
            float dampingRatio = 0.5f,
            float stiffness = 1.0f,
            float precision = 1e-5f)
        {
            // 将stiffness参数重命名为naturalFreq进行内部计算
            float naturalFreq = stiffness;
            
            double currentPosition = currentValue;
            double currentVel = currentVelocity;
            double targetPosition = targetValue;
            double targetVel = targetVelocity;
            double deltaTime = timeElapsed / 1000.0; // 转换为秒
            
            // 从stiffness和dampingRatio计算弹簧物理参数
            // 保持与原始SpringElastic相同的计算逻辑
            double halfLife = DampingToHalfLife(2.0 * dampingRatio * naturalFreq); // 从阻尼系数计算半衰期
            double dampingCoeff = HalfLifeToDamping(halfLife); // 阻尼系数
            double stiffnessValue = DampingRatioToStiffness(dampingRatio, dampingCoeff); // 刚度值
            
            // 计算调整后的目标位置，支持连续运动
            double adjustedTargetPosition = targetPosition + (dampingCoeff * targetVel) / (stiffnessValue + precision);
            double dampingHalf = dampingCoeff / 2.0f; // 阻尼系数的一半

            if (Math.Abs(stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0f) < precision)
            {
                // 临界阻尼：计算初始条件系数
                double initialDisplacement = currentPosition - adjustedTargetPosition;
                double initialVelocityWithDamping = currentVel + initialDisplacement * dampingHalf;

                double exponentialDecay = FastNegExp(dampingHalf * deltaTime);

                // 更新位置和速度
                currentPosition = initialDisplacement * exponentialDecay + deltaTime * initialVelocityWithDamping * exponentialDecay + adjustedTargetPosition;
                currentVel = -dampingHalf * initialDisplacement * exponentialDecay - dampingHalf * deltaTime * initialVelocityWithDamping * exponentialDecay + initialVelocityWithDamping * exponentialDecay;
            }
            else if (stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0f > 0.0)
            {
                // 欠阻尼：计算振荡频率和振幅
                double dampedFrequency = Math.Sqrt(stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0f);
                double displacementFromTarget = currentPosition - adjustedTargetPosition;
                
                // 计算振幅和相位
                double amplitude = Math.Sqrt(Square(currentVel + dampingHalf * displacementFromTarget) / (dampedFrequency * dampedFrequency + precision) + Square(displacementFromTarget));
                double phase = Math.Atan((currentVel + displacementFromTarget * dampingHalf) / (-displacementFromTarget * dampedFrequency + precision));

                // 根据位移方向确定振幅符号
                amplitude = displacementFromTarget > 0.0f ? amplitude : -amplitude;

                double exponentialDecay = FastNegExp(dampingHalf * deltaTime);

                // 更新位置和速度（振荡运动）
                currentPosition = amplitude * exponentialDecay * Math.Cos(dampedFrequency * deltaTime + phase) + adjustedTargetPosition;
                currentVel = -dampingHalf * amplitude * exponentialDecay * Math.Cos(dampedFrequency * deltaTime + phase) - 
                            dampedFrequency * amplitude * exponentialDecay * Math.Sin(dampedFrequency * deltaTime + phase);
            }
            else if (stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0f < 0.0)
            {
                // 过阻尼：计算两个衰减指数
                double fastDecayRate = (dampingCoeff + Math.Sqrt(dampingCoeff * dampingCoeff - 4 * stiffnessValue)) / 2.0f;
                double slowDecayRate = (dampingCoeff - Math.Sqrt(dampingCoeff * dampingCoeff - 4 * stiffnessValue)) / 2.0f;
                // 计算系数
                double fastCoeff = (adjustedTargetPosition * fastDecayRate - currentPosition * fastDecayRate - currentVel) / (slowDecayRate - fastDecayRate);
                double slowCoeff = currentPosition - fastCoeff - adjustedTargetPosition;

                double fastExponentialDecay = FastNegExp(fastDecayRate * deltaTime);
                double slowExponentialDecay = FastNegExp(slowDecayRate * deltaTime);

                // 更新位置和速度（过阻尼运动）
                currentPosition = slowCoeff * slowExponentialDecay + fastCoeff * fastExponentialDecay + adjustedTargetPosition;
                currentVel = -slowDecayRate * slowCoeff * slowExponentialDecay - fastDecayRate * fastCoeff * fastExponentialDecay;
            }

            newVelocity = (float)currentVel;
            return (float)currentPosition;
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
        public static float SpringPrecise(
            long timeElapsed,
            float currentValue,
            float currentVelocity,
            float targetValue,
            out float newVelocity,
            float targetVelocity = 0.0f,
            float dampingRatio = 0.5f,
            float stiffness = 1.0f,
            float precision = 1e-5f)
        {
            // 将stiffness参数重命名为naturalFreq进行内部计算
            float naturalFreq = stiffness;
            
            // 根据targetVelocity调整目标位置（类似SpringElastic的逻辑）
            float adjustedTargetPosition = targetValue;
            if (Math.Abs(targetVelocity) > precision)
            {
                // 计算调整后的目标位置，使系统在到达目标时具有指定速度
                // 使用与SpringElastic相同的逻辑：c = g + (d * q) / (s + eps)
                // 其中 d = 2 * dampingRatio * naturalFreq, s = naturalFreq^2
                float dampingCoeff = 2.0f * dampingRatio * naturalFreq;
                float stiffnessValue = naturalFreq * naturalFreq;
                adjustedTargetPosition = targetValue + (dampingCoeff * targetVelocity) / (stiffnessValue + precision);
            }
            
            float adjustedDisplacement = currentValue - adjustedTargetPosition;
            double deltaT = timeElapsed / 1000.0; // 转换为秒
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
                displacement = coeffA * Math.Exp(gammaMinus * deltaT) + coeffB * Math.Exp(gammaPlus * deltaT);
                calculatedVelocity = coeffA * gammaMinus * Math.Exp(gammaMinus * deltaT) +
                                    coeffB * gammaPlus * Math.Exp(gammaPlus * deltaT);
            }
            else if (Math.Abs(dampingRatio - 1.0f) < precision)
            {
                // 临界阻尼
                double coeffA = adjustedDisplacement;
                double coeffB = currentVelocity + naturalFreq * adjustedDisplacement;
                double nFdT = -naturalFreq * deltaT;
                displacement = (coeffA + coeffB * deltaT) * Math.Exp(nFdT);
                calculatedVelocity = ((coeffA + coeffB * deltaT) * Math.Exp(nFdT) * (-naturalFreq)) + 
                                    coeffB * Math.Exp(nFdT);
            }
            else
            {
                // 欠阻尼
                double dampedFreq = naturalFreq * Math.Sqrt(1 - dampingRatioSquared);
                double cosCoeff = adjustedDisplacement;
                double sinCoeff = (1.0 / dampedFreq) * ((-r * adjustedDisplacement) + currentVelocity);
                double dFdT = dampedFreq * deltaT;
                displacement = Math.Exp(r * deltaT) * (cosCoeff * Math.Cos(dFdT) + sinCoeff * Math.Sin(dFdT));
                calculatedVelocity = displacement * r +
                                    (Math.Exp(r * deltaT) *
                                     ((-dampedFreq * cosCoeff * Math.Sin(dFdT) + 
                                       dampedFreq * sinCoeff * Math.Cos(dFdT))));
            }

            float newValue = (float)(displacement + adjustedTargetPosition);
            newVelocity = (float)calculatedVelocity;

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
        /// <param name="targetVelocity">目标速度（到达目标位置时的期望速度）</param>
        /// <param name="dampingRatio">阻尼比</param>
        /// <param name="stiffness">弹簧刚度（实际上直接作为自然频率使用）</param>
        /// <param name="anticipation">预期系数，用于预测未来目标位置</param>
        /// <returns>新的位移值</returns>
        public static float SpringSimpleVelocitySmoothing(
            long timeElapsed,
            float currentValue,
            float currentVelocity,
            float targetValue,
            out float newVelocity,
            ref float intermediatePosition,
            float targetVelocity = 0.0f,
            float dampingRatio = 0.5f,
            float stiffness = 1.0f,
            float anticipation = 2.0f)
        {
            // 将stiffness参数重命名为naturalFreq进行内部计算
            float naturalFreq = stiffness;

            // 从stiffness和dampingRatio计算弹簧物理参数
            double halfLife = DampingToHalfLife(2.0 * dampingRatio * naturalFreq);
            double dampingCoeff = HalfLifeToDamping(halfLife);
            double dampingHalf = dampingCoeff / 2.0d;

            // 计算速度方向
            float velocityDirection = (float)((targetValue - intermediatePosition) > 0.0d ? 1.0d : -1.0d) * targetVelocity;

            // 计算预期时间
            float anticipatedTime = timeElapsed / 1000.0f + anticipation * (float)halfLife;
            
            // 计算未来目标位置
            float futureTargetPosition = Math.Abs(targetValue - intermediatePosition) > anticipatedTime * targetVelocity ?
                intermediatePosition + velocityDirection * anticipatedTime : targetValue;

            // 直接调用SpringSimple函数
            float result = SpringSimple(timeElapsed, currentValue, currentVelocity, futureTargetPosition, out newVelocity, dampingRatio, stiffness);

            // 更新中间位置
            intermediatePosition = Math.Abs(targetValue - intermediatePosition) > timeElapsed / 1000.0f * targetVelocity ? 
                intermediatePosition + velocityDirection * timeElapsed / 1000.0f : targetValue;

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
        public static float SpringSimpleDurationLimit(
            long timeElapsed,
            float currentValue,
            float currentVelocity,
            float targetValue,
            out float newVelocity,
            ref float intermediatePosition,
            float durationMillisecond = 200.0f,
            float dampingRatio = 0.5f,
            float stiffness = 1.0f,
            float anticipation = 2.0f)
        {
            // 将stiffness参数重命名为naturalFreq进行内部计算
            float naturalFreq = stiffness;

            // 从stiffness和dampingRatio计算弹簧物理参数
            double halfLife = DampingToHalfLife(2.0 * dampingRatio * naturalFreq);
            double dampingCoeff = HalfLifeToDamping(halfLife);
            double dampingHalf = dampingCoeff / 2.0d;

            // 计算最小时间（转换为秒）
            float minTimeSeconds = (durationMillisecond > timeElapsed ? durationMillisecond : timeElapsed) / 1000.0f;

            // 基于中间位置计算目标速度
            float targetVel = (targetValue - intermediatePosition) / minTimeSeconds;

            // 计算预期时间
            float anticipatedTime = timeElapsed / 1000.0f + anticipation * (float)halfLife;
            
            // 计算未来目标位置
            float futureTargetPosition = anticipatedTime < durationMillisecond ?
                intermediatePosition + targetVel * anticipatedTime : targetValue;

            // 直接调用SpringSimple函数
            float result = SpringSimple(timeElapsed, currentValue, currentVelocity, futureTargetPosition, out newVelocity, dampingRatio, stiffness);

            // 更新中间位置
            intermediatePosition += targetVel * timeElapsed / 1000.0f;

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
        public static float SpringSimpleDoubleSmoothing(
            long timeElapsed,
            float currentValue,
            float currentVelocity,
            float targetValue,
            out float newVelocity,
            ref float intermediatePosition,
            ref float intermediateVelocity,
            float dampingRatio = 0.5f,
            float stiffness = 1.0f)
        {
            // 第一层弹簧：从中间位置到目标位置
            float firstLayerResult = SpringSimple(timeElapsed, intermediatePosition, intermediateVelocity, targetValue, out intermediateVelocity, dampingRatio, stiffness);

            // 第二层弹簧：从当前位置到中间位置
            return SpringSimple(timeElapsed, currentValue, currentVelocity, firstLayerResult, out newVelocity, dampingRatio, stiffness);
        }

        private static double HalfLifeToDamping(double halfLife, double eps = 1e-5f)
        {
            return (4.0d * 0.6931471805599453d) / (halfLife + eps);
        }

        private static double DampingToHalfLife(double damping, double eps = 1e-5f)
        {
            return (4.0d * 0.6931471805599453d) / (damping + eps);
        }

        private static double DampingRatioToStiffness(double ratio, double damping)
        {
            return Square(damping / (ratio * 2.0f));
        }

        private static double DampingRatioToDamping(double ratio, double stiffness)
        {
            return ratio * 2.0f * Math.Sqrt(stiffness);
        }

        private static double FrequencyToStiffness(double frequency)
        {
            return Square(2.0d * Math.PI * frequency);
        }

        private static double FastNegExp(double x)
        {
            return 1.0d / (1.0d + x + 0.48d * x * x + 0.235d * x * x * x);
        }

        private static double Square(double x)
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
        public static float GetSpringValueAndVelocityFromNanos(
            long playTimeNanos,
            float initialValue,
            float targetValue,
            float initialVelocity,
            float dampingRatio = 0.5f,
            float stiffness = 1.0f,
            float targetVelocity = 0.0f,
            float precision = 1e-5f)
        {
            // TODO: 在弹簧实现中正确支持纳秒
            long playTimeMillis = playTimeNanos / 1_000_000L;
            return SpringPrecise(playTimeMillis, initialValue, initialVelocity, targetValue, out _, targetVelocity, dampingRatio, stiffness, precision);
    }
}
}
