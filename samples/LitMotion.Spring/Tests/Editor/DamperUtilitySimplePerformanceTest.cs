using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using LitMotion;

namespace ASUI.Tests
{
    /// <summary>
    /// DamperUtility简单性能测试类
    /// 使用Stopwatch进行性能比较，不依赖Unity Performance Testing包
    /// </summary>
    public class DamperUtilitySimplePerformanceTest
    {
        // 测试参数
        private const int TestIterations = 100000;
        private const float DeltaTime = 0.016f; // 60 FPS的deltaTime (秒)
        
        // 测试用的状态变量
        private float currentValue;
        private float currentVelocity;
        private float targetValue;
        private float newVelocity;
        
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
            newVelocity = 0.0f;
            intermediatePosition = 0.0f;
            intermediateVelocity = 0.0f;
        }

        [Test]
        public void PerformanceComparison_AllFunctions()
        {
            var results = new PerformanceResults();
            
            // 测试SpringElastic
            results.SpringElasticTime = TestSpringElastic();
            
            // 测试SpringPrecise
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
                    newVelocity, 0.0f, 1.0f);
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
                    ref currentValue, ref currentVelocity, targetValue,1.0f);
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
            UnityEngine.Debug.Log("=== DamperUtility性能测试结果 ===");
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

        [Test]
        public void PerformanceComparison_DifferentDampingStates()
        {
            UnityEngine.Debug.Log("=== 不同阻尼状态性能测试 ===");
            
            // 测试SpringElastic的不同阻尼状态
            var elasticOverdamped = TestSpringElasticWithParams(2.5f, 1.0f);
            var elasticCritical = TestSpringElasticWithParams(1.0f, 1.0f);
            var elasticUnderdamped = TestSpringElasticWithParams(0.1f, 1.0f);
            
            UnityEngine.Debug.Log($"SpringElastic - 过阻尼: {elasticOverdamped}ms");
            UnityEngine.Debug.Log($"SpringElastic - 临界阻尼: {elasticCritical}ms");
            UnityEngine.Debug.Log($"SpringElastic - 欠阻尼: {elasticUnderdamped}ms");
            
            // 测试SpringPrecise的不同阻尼状态
            var preciseOverdamped = TestSpringPreciseWithParams(2.5f, 1.0f);
            var preciseCritical = TestSpringPreciseWithParams(1.0f, 1.0f);
            var preciseUnderdamped = TestSpringPreciseWithParams(0.1f, 1.0f);
            
            UnityEngine.Debug.Log($"SpringPrecise - 过阻尼: {preciseOverdamped}ms");
            UnityEngine.Debug.Log($"SpringPrecise - 临界阻尼: {preciseCritical}ms");
            UnityEngine.Debug.Log($"SpringPrecise - 欠阻尼: {preciseUnderdamped}ms");
        }

        private long TestSpringElasticWithParams(float dampingRatio, float stiffness)
        {
            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < TestIterations; i++)
            {
                SpringUtility.SpringElastic(
                    DeltaTime,
                    ref currentValue, ref currentVelocity, targetValue, 0.0f, dampingRatio, stiffness);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private long TestSpringPreciseWithParams(float dampingRatio, float stiffness)
        {
            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < TestIterations; i++)
            {
                SpringUtility.SpringPrecise(
                    DeltaTime,
                    ref currentValue, ref currentVelocity, targetValue, 0.0f, dampingRatio, stiffness);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

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
