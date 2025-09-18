using System;
namespace LitMotion
{
    //https://github.com/orangeduck/Spring-It-On
    /// <summary>
    /// 提供阻尼插值计算的实用程序类。
    /// </summary>
    public static class DamperUtility
    {
        /// <summary>
        /// 临界阻尼
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="velocity"></param>
        /// <param name="targetValue"></param>
        /// <param name="halfLife"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public static double SimpleSpringDamperImplicit(
            double startValue,
            ref double velocity,
            double targetValue,
            double halfLife,
            double deltaTime)
        {
            double y = HalfLifeToDamping(halfLife) / 2.0d;
            double j0 = startValue - targetValue;
            double j1 = velocity + j0 * y;
            double eydt = FastNegExp(y * deltaTime);

            startValue = eydt * (j0 + j1 * deltaTime) + targetValue;
            velocity = eydt * (velocity - j1 * y * deltaTime);
            return startValue;
        }
        public static double VelocitySpringDamperImplicit(
            double startValue,
            ref double velocity,
            ref double xi,
            double targetValue,
            double targetVelocity,
            double halfLife,
            double deltaTime,
            double apprehension = 2.0d)
        {
            double x_diff = ((targetValue - xi) > 0.0d ? 1.0d : -1.0d) * targetVelocity;

            double t_goal_future = deltaTime + apprehension * halfLife;
            double targetValue_future = Math.Abs(targetValue - xi) > t_goal_future * targetVelocity ?
                xi + x_diff * t_goal_future : targetValue;

            double result = SimpleSpringDamperImplicit(startValue, ref velocity, targetValue_future, halfLife, deltaTime);

            xi = Math.Abs(targetValue - xi) > deltaTime * targetVelocity ? xi + x_diff * deltaTime : targetValue;
            return result;
        }

        public static double TimedSpringDamperImplicit(
            double startValue,
            ref double Velocity,
            ref double xi,
            double targetValue,
            double targetTime,
            double halfLife,
            double deltaTime,
            double apprehension = 2.0d)
        {
            double min_time = targetTime > deltaTime ? targetTime : deltaTime;

            double v_goal = (targetValue - xi) / min_time;

            double t_goal_future = deltaTime + apprehension * halfLife;
            double targetValue_future = t_goal_future < targetTime ?
                xi + v_goal * t_goal_future : targetValue;

            double result = SimpleSpringDamperImplicit(startValue, ref Velocity, targetValue_future, halfLife, deltaTime);

            xi += v_goal * deltaTime;

            return result;
        }

        public static double DoubleSpringDamperImplicit(
            double startValue,
            ref double velocity,
            ref double xi,
            ref double vi,
            double targetValue,
            double halfLife,
            double deltaTime)
        {
            xi = SimpleSpringDamperImplicit(xi, ref vi, targetValue, 0.5d * halfLife, deltaTime);
            return SimpleSpringDamperImplicit(startValue, ref velocity, xi, 0.5d * halfLife, deltaTime);
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


        public static double SpringDamperExactRatio(
            double x,
            ref double v,
            double targetValue,
            double v_goal,
            double damping_ratio,
            double halfLife,
            double deltaTime,
            double eps = 1e-5f)
        {
            double g = targetValue;
            double q = v_goal;
            double d = HalfLifeToDamping(halfLife);
            double s = DampingRatioToStiffness(damping_ratio, d);
            double c = g + (d * q) / (s + eps);
            double y = d / 2.0f;

            if (Math.Abs(s - (d * d) / 4.0f) < eps) // 欠阻尼
            {
                double j0 = x - c;
                double j1 = v + j0 * y;

                double eydt = FastNegExp(y * deltaTime);

                x = j0 * eydt + deltaTime * j1 * eydt + c;
                v = -y * j0 * eydt - y * deltaTime * j1 * eydt + j1 * eydt;
                return x;
            }
            else if (s - (d * d) / 4.0f > 0.0) // 临界阻尼
            {
                double w = Math.Sqrt(s - (d * d) / 4.0f);
                double j = Math.Sqrt(Square(v + y * (x - c)) / (w * w + eps) + Square(x - c));
                double p = Math.Atan((v + (x - c) * y) / (-(x - c) * w + eps));

                j = (x - c) > 0.0f ? j : -j;

                double eydt = FastNegExp(y * deltaTime);

                x = j * eydt * Math.Cos(w * deltaTime + p) + c;
                v = -y * j * eydt * Math.Cos(w * deltaTime + p) - w * j * eydt * Math.Sin(w * deltaTime + p);
                return x;
            }
            else if (s - (d * d) / 4.0f < 0.0) // 过阻尼
            {
                double y0 = (d + Math.Sqrt(d * d - 4 * s)) / 2.0f;
                double y1 = (d - Math.Sqrt(d * d - 4 * s)) / 2.0f;
                double j1 = (c * y0 - x * y0 - v) / (y1 - y0);
                double j0 = x - j1 - c;

                double ey0deltaTime = FastNegExp(y0 * deltaTime);
                double ey1deltaTime = FastNegExp(y1 * deltaTime);

                x = j0 * ey0deltaTime + j1 * ey1deltaTime + c;
                v = -y0 * j0 * ey0deltaTime - y1 * j1 * ey1deltaTime;
                return x;
            }
            return 0d;
        }
    }
}
