using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using LitMotion;

namespace ASUI.Tests
{
    /// <summary>
    /// DamperUtility性能测试类
    /// 比较SpringElastic、SpringPrecise和SpringSimple的性能差异
    /// 使用Stopwatch进行性能测试，不依赖Unity Performance Testing包
    /// </summary>
    public class DamperUtilityPerformanceTest
    {
        // 测试参数
        private const int TestIterations = 1000000; // 增加到100万次迭代
        private const float DeltaTime = 0.016f; // 60 FPS的deltaTime (秒)
        
        // 测试用的状态变量
        private float currentValue;
        private float currentVelocity;
        private float targetValue;
        
        // SpringSimple变种测试用的中间状态
        private float intermediatePosition;
        private float intermediateVelocity;
        
        [SetUp]
        public void Setup()
        {
            // 初始化测试状态
            currentValue = 0.0f;
            currentVelocity = 0.0f;
            targetValue = 100.0f;
            intermediatePosition = 0.0f;
            intermediateVelocity = 0.0f;
            
            // Burst预热阶段 - 确保所有函数都完成Burst编译
            WarmupBurstCompilation();
        }
        
        /// <summary>
        /// Burst预热阶段，确保所有SpringUtility函数都完成Burst编译
        /// </summary>
        private void WarmupBurstCompilation()
        {
            UnityEngine.Debug.Log("开始Burst预热阶段...");
            var warmupStopwatch = Stopwatch.StartNew();
            
            // 预热所有主要函数，确保Burst编译完成
            const int warmupIterations = 1000;
            
            for (int i = 0; i < warmupIterations; i++)
            {
                // 预热SpringElastic - 三种阻尼状态
                SpringUtility.SpringElastic(DeltaTime, ref currentValue, ref currentVelocity, targetValue, 0.0f, 2.5f, 1.0f); // 过阻尼
                SpringUtility.SpringElastic(DeltaTime, ref currentValue, ref currentVelocity, targetValue, 0.0f, 1.0f, 1.0f);  // 临界阻尼
                SpringUtility.SpringElastic(DeltaTime, ref currentValue, ref currentVelocity, targetValue, 0.0f, 0.1f, 1.0f);  // 欠阻尼
                
                // 预热SpringPrecise - 三种阻尼状态
                SpringUtility.SpringPrecise(DeltaTime, ref currentValue, ref currentVelocity, targetValue, 0.0f, 2.5f, 1.0f); // 过阻尼
                SpringUtility.SpringPrecise(DeltaTime, ref currentValue, ref currentVelocity, targetValue, 0.0f, 1.0f, 1.0f);  // 临界阻尼
                SpringUtility.SpringPrecise(DeltaTime, ref currentValue, ref currentVelocity, targetValue, 0.0f, 0.1f, 1.0f);  // 欠阻尼
                
                // 预热SpringSimple
                SpringUtility.SpringSimple(DeltaTime, ref currentValue, ref currentVelocity, targetValue, 1.0f);
                
                // 预热SpringSimple变种函数
                SpringUtility.SpringSimpleVelocitySmoothing(DeltaTime, ref currentValue, ref currentVelocity, targetValue, ref intermediatePosition, 2.0f, 1.0f);
                SpringUtility.SpringSimpleDurationLimit(DeltaTime, ref currentValue, ref currentVelocity, targetValue, 0.5f);
                SpringUtility.SpringSimpleDoubleSmoothing(DeltaTime, ref currentValue, ref currentVelocity, targetValue, ref intermediatePosition, ref intermediateVelocity, 1.0f);
            }
            
            warmupStopwatch.Stop();
            UnityEngine.Debug.Log($"Burst预热完成，耗时: {warmupStopwatch.ElapsedMilliseconds}ms ({warmupIterations * 9}次函数调用)");
        }

        #region SpringElastic性能测试
        [Test]
        public void SpringElastic_Overdamped_Performance()
        {
            var stopwatch = Stopwatch.StartNew();
            
                for (int i = 0; i < TestIterations; i++)
                {
                    SpringUtility.SpringElastic(
                        DeltaTime,
                        ref currentValue, ref currentVelocity, targetValue,
                        0.0f, 2.5f, 1.0f);
            }
            
            stopwatch.Stop();
            UnityEngine.Debug.Log($"SpringElastic 过阻尼性能测试: {stopwatch.ElapsedMilliseconds}ms ({TestIterations}次迭代)");
        }

        [Test]
        public void SpringElastic_CriticalDamped_Performance()
        {
            var stopwatch = Stopwatch.StartNew();
            
                for (int i = 0; i < TestIterations; i++)
                {
                    SpringUtility.SpringElastic(
                        DeltaTime,
                        ref currentValue, ref currentVelocity, targetValue,
                        0.0f, 1.0f, 1.0f);
            }
            
            stopwatch.Stop();
            UnityEngine.Debug.Log($"SpringElastic 临界阻尼性能测试: {stopwatch.ElapsedMilliseconds}ms ({TestIterations}次迭代)");
        }

        [Test]
        public void SpringElastic_Underdamped_Performance()
        {
            var stopwatch = Stopwatch.StartNew();
            
                for (int i = 0; i < TestIterations; i++)
                {
                    SpringUtility.SpringElastic(
                        DeltaTime,
                        ref currentValue, ref currentVelocity, targetValue,
                        0.0f, 0.1f, 1.0f);
            }
            
            stopwatch.Stop();
            UnityEngine.Debug.Log($"SpringElastic 欠阻尼性能测试: {stopwatch.ElapsedMilliseconds}ms ({TestIterations}次迭代)");
        }

        #endregion

        #region SpringPrecise性能测试

        [Test]
        public void SpringPrecise_Overdamped_Performance()
        {
            var stopwatch = Stopwatch.StartNew();
            
                for (int i = 0; i < TestIterations; i++)
                {
                    SpringUtility.SpringPrecise(
                        DeltaTime,
                        ref currentValue, ref currentVelocity, targetValue,
                        0.0f, 2.5f, 1.0f);
            }
            
            stopwatch.Stop();
            UnityEngine.Debug.Log($"SpringPrecise 过阻尼性能测试: {stopwatch.ElapsedMilliseconds}ms ({TestIterations}次迭代)");
        }

        [Test]
        public void SpringPrecise_CriticalDamped_Performance()
        {
            var stopwatch = Stopwatch.StartNew();
            
                for (int i = 0; i < TestIterations; i++)
                {
                    SpringUtility.SpringPrecise(
                        DeltaTime,
                        ref currentValue, ref currentVelocity, targetValue,
                        0.0f, 1.0f, 1.0f);
            }
            
            stopwatch.Stop();
            UnityEngine.Debug.Log($"SpringPrecise 临界阻尼性能测试: {stopwatch.ElapsedMilliseconds}ms ({TestIterations}次迭代)");
        }

        [Test]
        public void SpringPrecise_Underdamped_Performance()
        {
            var stopwatch = Stopwatch.StartNew();
            
                for (int i = 0; i < TestIterations; i++)
                {
                    SpringUtility.SpringPrecise(
                        DeltaTime,
                        ref currentValue, ref currentVelocity, targetValue,
                        0.0f, 0.1f, 1.0f);
            }
            
            stopwatch.Stop();
            UnityEngine.Debug.Log($"SpringPrecise 欠阻尼性能测试: {stopwatch.ElapsedMilliseconds}ms ({TestIterations}次迭代)");
        }

        #endregion

        #region SpringSimple性能测试

        [Test]
        public void SpringSimple_Performance()
        {
            var stopwatch = Stopwatch.StartNew();
            
                for (int i = 0; i < TestIterations; i++)
                {
                    SpringUtility.SpringSimple(
                        DeltaTime,
                        ref currentValue, ref currentVelocity, targetValue,
                        1.0f);
                }
            
            stopwatch.Stop();
            UnityEngine.Debug.Log($"SpringSimple 性能测试: {stopwatch.ElapsedMilliseconds}ms ({TestIterations}次迭代)");
        }

        #endregion

        #region SpringSimple变种性能测试

        [Test]
        public void SpringSimpleVelocitySmoothing_Performance()
        {
            var stopwatch = Stopwatch.StartNew();
            
                for (int i = 0; i < TestIterations; i++)
                {
                    SpringUtility.SpringSimpleVelocitySmoothing(
                        DeltaTime,
                        ref currentValue, ref currentVelocity, targetValue,
                        ref intermediatePosition, 2.0f, 1.0f);
            }
            
            stopwatch.Stop();
            UnityEngine.Debug.Log($"SpringSimpleVelocitySmoothing 性能测试: {stopwatch.ElapsedMilliseconds}ms ({TestIterations}次迭代)");
        }

        [Test]
        public void SpringSimpleDurationLimit_Performance()
        {
            var stopwatch = Stopwatch.StartNew();
            
                for (int i = 0; i < TestIterations; i++)
                {
                    SpringUtility.SpringSimpleDurationLimit(
                        DeltaTime,
                        ref currentValue, ref currentVelocity, targetValue,
                        0.5f);
            }
            
            stopwatch.Stop();
            UnityEngine.Debug.Log($"SpringSimpleDurationLimit 性能测试: {stopwatch.ElapsedMilliseconds}ms ({TestIterations}次迭代)");
        }

        [Test]
        public void SpringSimpleDoubleSmoothing_Performance()
        {
            var stopwatch = Stopwatch.StartNew();
            
                for (int i = 0; i < TestIterations; i++)
                {
                    SpringUtility.SpringSimpleDoubleSmoothing(
                        DeltaTime,
                        ref currentValue, ref currentVelocity, targetValue,
                        ref intermediatePosition, ref intermediateVelocity, 1.0f);
                }
            
            stopwatch.Stop();
            UnityEngine.Debug.Log($"SpringSimpleDoubleSmoothing 性能测试: {stopwatch.ElapsedMilliseconds}ms ({TestIterations}次迭代)");
        }

        #endregion

        #region 综合性能比较测试

        [Test]
        public void AllDamperFunctions_Comprehensive_Performance()
        {
            // 为综合测试进行额外的Burst预热，确保最准确的性能测量
            UnityEngine.Debug.Log("综合性能测试 - 额外Burst预热...");
            var extraWarmupStopwatch = Stopwatch.StartNew();
            
            // 额外预热，确保所有函数路径都被编译
            for (int i = 0; i < 500; i++)
            {
                SpringUtility.SpringElastic(DeltaTime, ref currentValue, ref currentVelocity, targetValue, 0.0f, 1.0f, 1.0f);
                SpringUtility.SpringPrecise(DeltaTime, ref currentValue, ref currentVelocity, targetValue, 0.0f, 1.0f, 1.0f);
                SpringUtility.SpringSimple(DeltaTime, ref currentValue, ref currentVelocity, targetValue, 1.0f);
            }
            
            extraWarmupStopwatch.Stop();
            UnityEngine.Debug.Log($"额外Burst预热完成，耗时: {extraWarmupStopwatch.ElapsedMilliseconds}ms");
            
            var results = new PerformanceResults();
            
            // 测试SpringElastic - 临界阻尼
            results.SpringElasticTime = TestSpringElastic();
            
            // 测试SpringPrecise - 临界阻尼
            results.SpringPreciseTime = TestSpringPrecise();
            
            // 测试SpringSimple
            results.SpringSimpleTime = TestSpringSimple();
            
            // 测试SpringSimpleVelocitySmoothing
            results.SpringSimpleVelocitySmoothingTime = TestSpringSimpleVelocitySmoothing();
            
            // 测试SpringSimpleDurationLimit
            results.SpringSimpleDurationLimitTime = TestSpringSimpleDurationLimit();
            
            // 测试SpringSimpleDoubleSmoothing
            results.SpringSimpleDoubleSmoothingTime = TestSpringSimpleDoubleSmoothing();
            
            // 输出结果
            PrintResults(results);
            
            // 验证性能差异
            AssertPerformance(results);
        }

        private long TestSpringElastic()
        {
            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < TestIterations; i++)
            {
                    SpringUtility.SpringElastic(
                        DeltaTime,
                        ref currentValue, ref currentVelocity, targetValue,
                        0.0f, 1.0f, 1.0f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private long TestSpringPrecise()
        {
            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < TestIterations; i++)
            {
                    SpringUtility.SpringPrecise(
                        DeltaTime,
                        ref currentValue, ref currentVelocity, targetValue,
                        0.0f, 1.0f, 1.0f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private long TestSpringSimple()
        {
            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < TestIterations; i++)
            {
                    SpringUtility.SpringSimple(
                        DeltaTime,
                        ref currentValue, ref currentVelocity, targetValue,
                        1.0f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private long TestSpringSimpleVelocitySmoothing()
        {
            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < TestIterations; i++)
            {
                    SpringUtility.SpringSimpleVelocitySmoothing(
                        DeltaTime,
                        ref currentValue, ref currentVelocity, targetValue,
                        ref intermediatePosition, 2.0f, 1.0f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private long TestSpringSimpleDurationLimit()
        {
            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < TestIterations; i++)
            {
                    SpringUtility.SpringSimpleDurationLimit(
                        DeltaTime,
                        ref currentValue, ref currentVelocity, targetValue,
                        0.5f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private long TestSpringSimpleDoubleSmoothing()
        {
            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < TestIterations; i++)
            {
                    SpringUtility.SpringSimpleDoubleSmoothing(
                        DeltaTime,
                        ref currentValue, ref currentVelocity, targetValue,
                        ref intermediatePosition, ref intermediateVelocity, 1.0f);
                }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private void PrintResults(PerformanceResults results)
        {
            UnityEngine.Debug.Log("=== DamperUtility综合性能测试结果 ===");
            UnityEngine.Debug.Log($"测试迭代次数: {TestIterations:N0}");
            UnityEngine.Debug.Log($"SpringElastic: {results.SpringElasticTime}ms");
            UnityEngine.Debug.Log($"SpringPrecise: {results.SpringPreciseTime}ms");
            UnityEngine.Debug.Log($"SpringSimple: {results.SpringSimpleTime}ms");
            UnityEngine.Debug.Log($"SpringSimpleVelocitySmoothing: {results.SpringSimpleVelocitySmoothingTime}ms");
            UnityEngine.Debug.Log($"SpringSimpleDurationLimit: {results.SpringSimpleDurationLimitTime}ms");
            UnityEngine.Debug.Log($"SpringSimpleDoubleSmoothing: {results.SpringSimpleDoubleSmoothingTime}ms");
            
            // 计算相对性能
            var fastest = Math.Min(Math.Min(results.SpringElasticTime, results.SpringPreciseTime), results.SpringSimpleTime);
            UnityEngine.Debug.Log("\n=== 相对性能比较 (以最快为基准) ===");
            
            if (fastest > 0)
            {
                UnityEngine.Debug.Log($"SpringElastic: {results.SpringElasticTime / (double)fastest:F2}x");
                UnityEngine.Debug.Log($"SpringPrecise: {results.SpringPreciseTime / (double)fastest:F2}x");
                UnityEngine.Debug.Log($"SpringSimple: {results.SpringSimpleTime / (double)fastest:F2}x");
                UnityEngine.Debug.Log($"SpringSimpleVelocitySmoothing: {results.SpringSimpleVelocitySmoothingTime / (double)fastest:F2}x");
                UnityEngine.Debug.Log($"SpringSimpleDurationLimit: {results.SpringSimpleDurationLimitTime / (double)fastest:F2}x");
                UnityEngine.Debug.Log($"SpringSimpleDoubleSmoothing: {results.SpringSimpleDoubleSmoothingTime / (double)fastest:F2}x");
            }
            else
            {
                UnityEngine.Debug.Log("所有函数执行时间都小于1ms，无法计算相对性能");
                UnityEngine.Debug.Log("建议增加测试迭代次数以获得更精确的测量结果");
            }
        }

        private void AssertPerformance(PerformanceResults results)
        {
            // 如果所有时间都是0，跳过断言验证
            if (results.SpringElasticTime == 0 && results.SpringPreciseTime == 0 && results.SpringSimpleTime == 0)
            {
                UnityEngine.Debug.Log("警告: 所有函数执行时间都是0ms，跳过性能断言验证");
                return;
            }
            
            // 验证SpringSimple应该是最快的（因为它是最简单的临界阻尼实现）
            if (results.SpringSimpleTime > 0)
            {
                Assert.LessOrEqual(results.SpringSimpleTime, results.SpringElasticTime, 
                    "SpringSimple应该比SpringElastic更快");
                Assert.LessOrEqual(results.SpringSimpleTime, results.SpringPreciseTime, 
                    "SpringSimple应该比SpringPrecise更快");
            }
            
            // 验证SpringElastic和SpringPrecise性能差异在合理范围内
            if (results.SpringElasticTime > 0 && results.SpringPreciseTime > 0)
            {
                var elasticPreciseRatio = Math.Max(results.SpringElasticTime, results.SpringPreciseTime) / 
                                        (double)Math.Min(results.SpringElasticTime, results.SpringPreciseTime);
                Assert.LessOrEqual(elasticPreciseRatio, 5.0, 
                    "SpringElastic和SpringPrecise性能差异不应超过5倍");
            }
            
            // 验证变种函数应该比基础函数慢
            if (results.SpringSimpleTime > 0)
            {
                Assert.LessOrEqual(results.SpringSimpleTime, results.SpringSimpleVelocitySmoothingTime, 
                    "SpringSimple应该比SpringSimpleVelocitySmoothing更快");
                Assert.LessOrEqual(results.SpringSimpleTime, results.SpringSimpleDurationLimitTime, 
                    "SpringSimple应该比SpringSimpleDurationLimit更快");
                Assert.LessOrEqual(results.SpringSimpleTime, results.SpringSimpleDoubleSmoothingTime, 
                    "SpringSimple应该比SpringSimpleDoubleSmoothing更快");
            }
        }

        #endregion

        #region 压力测试

        [Test]
        public void StressTest_HighIterations()
        {
            const int stressIterations = 100000;
            
            // 压力测试前的Burst预热
            UnityEngine.Debug.Log("压力测试 - Burst预热...");
            var stressWarmupStopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < 200; i++)
            {
                SpringUtility.SpringElastic(DeltaTime, ref currentValue, ref currentVelocity, targetValue, 0.0f, 1.0f, 1.0f);
                SpringUtility.SpringPrecise(DeltaTime, ref currentValue, ref currentVelocity, targetValue, 0.0f, 1.0f, 1.0f);
                SpringUtility.SpringSimple(DeltaTime, ref currentValue, ref currentVelocity, targetValue, 1.0f);
            }
            
            stressWarmupStopwatch.Stop();
            UnityEngine.Debug.Log($"压力测试Burst预热完成，耗时: {stressWarmupStopwatch.ElapsedMilliseconds}ms");
            
            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < stressIterations; i++)
            {
                // 交替测试三种主要函数
                switch (i % 3)
                {
                    case 0:
                        SpringUtility.SpringElastic(
                    DeltaTime,
                    ref currentValue, ref currentVelocity, targetValue,
                    0.0f, 1.0f, 1.0f);
                        break;
                    case 1:
                        SpringUtility.SpringPrecise(
                    DeltaTime,
                    ref currentValue, ref currentVelocity, targetValue,
                    0.0f, 1.0f, 1.0f);
                        break;
                    case 2:
                        SpringUtility.SpringSimple(
                            DeltaTime,
                            ref currentValue, ref currentVelocity, targetValue,
                            1.0f);
                        break;
                }
            }
            
            stopwatch.Stop();
            UnityEngine.Debug.Log($"压力测试完成: {stopwatch.ElapsedMilliseconds}ms ({stressIterations}次迭代)");
        }

        #endregion

        private struct PerformanceResults
        {
            public long SpringElasticTime;
            public long SpringPreciseTime;
            public long SpringSimpleTime;
            public long SpringSimpleVelocitySmoothingTime;
            public long SpringSimpleDurationLimitTime;
            public long SpringSimpleDoubleSmoothingTime;
        }
    }
}