using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using LitMotion;

namespace ASUI.Tests
{
    /// <summary>
    /// EaseUtility性能测试类，专注于测试缓动函数的性能表现
    /// </summary>
    [TestFixture]
    public class EaseUtilityPerformanceTest
    {
        private const int TestIterations = 1000000;
        private const int WarmupIterations = 10000;

        [SetUp]
        public void Setup()
        {
            // Burst预热阶段
            WarmupBurstCompilation();
        }

        /// <summary>
        /// Burst预热阶段，确保所有EaseUtility函数都完成Burst编译
        /// </summary>
        private void WarmupBurstCompilation()
        {
            UnityEngine.Debug.Log("开始EaseUtility Burst预热阶段...");
            var warmupStopwatch = Stopwatch.StartNew();
            
            // 预热所有主要函数，确保Burst编译完成
            var allEases = GetAllEaseTypes();
            
            for (int i = 0; i < WarmupIterations; i++)
            {
                float t = (i % 100) / 100f;
                foreach (var ease in allEases)
                {
                    EaseUtility.Evaluate(t, ease);
                }
            }
            
            warmupStopwatch.Stop();
            UnityEngine.Debug.Log($"EaseUtility Burst预热完成，耗时: {warmupStopwatch.ElapsedMilliseconds}ms ({WarmupIterations * allEases.Length}次函数调用)");
        }

        /// <summary>
        /// 获取所有Ease类型
        /// </summary>
        private Ease[] GetAllEaseTypes()
        {
            return new[]
            {
                Ease.InSine, Ease.OutSine, Ease.InOutSine,
                Ease.InQuad, Ease.OutQuad, Ease.InOutQuad,
                Ease.InCubic, Ease.OutCubic, Ease.InOutCubic,
                Ease.InQuart, Ease.OutQuart, Ease.InOutQuart,
                Ease.InQuint, Ease.OutQuint, Ease.InOutQuint,
                Ease.InExpo, Ease.OutExpo, Ease.InOutExpo,
                Ease.InCirc, Ease.OutCirc, Ease.InOutCirc,
                Ease.InElastic, Ease.OutElastic, Ease.InOutElastic,
                Ease.InBack, Ease.OutBack, Ease.InOutBack,
                Ease.InBounce, Ease.OutBounce, Ease.InOutBounce
            };
        }

        /// <summary>
        /// 综合性能测试 - 所有缓动函数
        /// </summary>
        [Test]
        public void AllEaseFunctions_Comprehensive_Performance()
        {
            UnityEngine.Debug.Log("=== EaseUtility 综合性能测试 ===");
            
            var allEases = GetAllEaseTypes();
            var results = new Dictionary<Ease, PerformanceResult>();

            foreach (var ease in allEases)
            {
                var result = TestSingleEasePerformance(ease);
                results[ease] = result;
            }

            // 输出性能结果
            UnityEngine.Debug.Log($"\n性能测试结果 (迭代次数: {TestIterations}):");
            UnityEngine.Debug.Log("函数名称\t\t\t执行时间(ms)\t\t平均每次(ns)");
            UnityEngine.Debug.Log("------------------------------------------------------------");

            foreach (var kvp in results.OrderBy(x => x.Value.ExecutionTimeMs))
            {
                var ease = kvp.Key;
                var result = kvp.Value;
                UnityEngine.Debug.Log($"{ease,-20}\t{result.ExecutionTimeMs,8}ms\t\t{result.AverageTimePerCallNs:F2}ns");
            }

            // 性能分析
            AnalyzePerformanceResults(results);
        }

        /// <summary>
        /// 测试单个缓动函数的性能
        /// </summary>
        private PerformanceResult TestSingleEasePerformance(Ease ease)
        {
            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < TestIterations; i++)
            {
                float t = (i % 100) / 100f; // 循环使用0-1的值
                EaseUtility.Evaluate(t, ease);
            }
            
            stopwatch.Stop();
            
            return new PerformanceResult
            {
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                AverageTimePerCallNs = (double)stopwatch.ElapsedTicks * 1000.0 / TestIterations / Stopwatch.Frequency * 1000000.0
            };
        }

        /// <summary>
        /// 分析性能结果
        /// </summary>
        private void AnalyzePerformanceResults(Dictionary<Ease, PerformanceResult> results)
        {
            var fastest = results.OrderBy(x => x.Value.ExecutionTimeMs).First();
            var slowest = results.OrderByDescending(x => x.Value.ExecutionTimeMs).First();
            
            UnityEngine.Debug.Log($"\n=== 性能分析 ===");
            UnityEngine.Debug.Log($"最快: {fastest.Key} ({fastest.Value.ExecutionTimeMs}ms, {fastest.Value.AverageTimePerCallNs:F2}ns/次)");
            UnityEngine.Debug.Log($"最慢: {slowest.Key} ({slowest.Value.ExecutionTimeMs}ms, {slowest.Value.AverageTimePerCallNs:F2}ns/次)");
            UnityEngine.Debug.Log($"性能差异: {(float)slowest.Value.ExecutionTimeMs / fastest.Value.ExecutionTimeMs:F2}x");
            
            // 按性能分类
            var fastFunctions = results.Where(x => x.Value.ExecutionTimeMs <= fastest.Value.ExecutionTimeMs * 1.2f).ToList();
            var mediumFunctions = results.Where(x => x.Value.ExecutionTimeMs > fastest.Value.ExecutionTimeMs * 1.2f && 
                                                   x.Value.ExecutionTimeMs <= fastest.Value.ExecutionTimeMs * 2.0f).ToList();
            var slowFunctions = results.Where(x => x.Value.ExecutionTimeMs > fastest.Value.ExecutionTimeMs * 2.0f).ToList();
            
            UnityEngine.Debug.Log($"\n性能分类:");
            UnityEngine.Debug.Log($"快速函数 ({fastFunctions.Count}个): {string.Join(", ", fastFunctions.Select(x => x.Key))}");
            UnityEngine.Debug.Log($"中等函数 ({mediumFunctions.Count}个): {string.Join(", ", mediumFunctions.Select(x => x.Key))}");
            UnityEngine.Debug.Log($"较慢函数 ({slowFunctions.Count}个): {string.Join(", ", slowFunctions.Select(x => x.Key))}");
        }

        /// <summary>
        /// 按函数类型分组性能测试
        /// </summary>
        [Test]
        public void EaseFunctions_Grouped_Performance()
        {
            UnityEngine.Debug.Log("=== 按函数类型分组的性能测试 ===");

            var functionGroups = new Dictionary<string, Ease[]>
            {
                ["Sine系列"] = new[] { Ease.InSine, Ease.OutSine, Ease.InOutSine },
                ["Quad系列"] = new[] { Ease.InQuad, Ease.OutQuad, Ease.InOutQuad },
                ["Cubic系列"] = new[] { Ease.InCubic, Ease.OutCubic, Ease.InOutCubic },
                ["Quart系列"] = new[] { Ease.InQuart, Ease.OutQuart, Ease.InOutQuart },
                ["Quint系列"] = new[] { Ease.InQuint, Ease.OutQuint, Ease.InOutQuint },
                ["Expo系列"] = new[] { Ease.InExpo, Ease.OutExpo, Ease.InOutExpo },
                ["Circ系列"] = new[] { Ease.InCirc, Ease.OutCirc, Ease.InOutCirc },
                ["Elastic系列"] = new[] { Ease.InElastic, Ease.OutElastic, Ease.InOutElastic },
                ["Back系列"] = new[] { Ease.InBack, Ease.OutBack, Ease.InOutBack },
                ["Bounce系列"] = new[] { Ease.InBounce, Ease.OutBounce, Ease.InOutBounce }
            };

            foreach (var group in functionGroups)
            {
                UnityEngine.Debug.Log($"\n--- {group.Key} ---");
                
                var groupResults = new List<(Ease ease, long timeMs)>();
                
                foreach (var ease in group.Value)
                {
                    var stopwatch = Stopwatch.StartNew();
                    
                    for (int i = 0; i < TestIterations; i++)
                    {
                        float t = (i % 100) / 100f;
                        EaseUtility.Evaluate(t, ease);
                    }
                    
                    stopwatch.Stop();
                    groupResults.Add((ease, stopwatch.ElapsedMilliseconds));
                }
                
                // 按性能排序
                groupResults.Sort((a, b) => a.timeMs.CompareTo(b.timeMs));
                
                foreach (var (ease, timeMs) in groupResults)
                {
                    UnityEngine.Debug.Log($"  {ease}: {timeMs}ms");
                }
                
                // 计算组内性能差异
                var fastest = groupResults[0].timeMs;
                var slowest = groupResults[groupResults.Count - 1].timeMs;
                UnityEngine.Debug.Log($"  组内性能差异: {(float)slowest / fastest:F2}x");
            }
        }

        /// <summary>
        /// 压力测试 - 高迭代次数
        /// </summary>
        [Test]
        public void StressTest_HighIterations()
        {
            UnityEngine.Debug.Log("=== 压力测试 - 高迭代次数 ===");
            
            const int stressIterations = 10000000; // 1000万次
            var testEases = new[] { Ease.Linear, Ease.InQuad, Ease.OutQuad, Ease.InOutQuad, Ease.InElastic, Ease.OutBounce };
            
            foreach (var ease in testEases)
            {
                var stopwatch = Stopwatch.StartNew();
                
                for (int i = 0; i < stressIterations; i++)
                {
                    float t = (i % 100) / 100f;
                    EaseUtility.Evaluate(t, ease);
                }
                
                stopwatch.Stop();
                
                double averageTimePerCallNs = (double)stopwatch.ElapsedTicks * 1000.0 / stressIterations / Stopwatch.Frequency * 1000000.0;
                
                UnityEngine.Debug.Log($"{ease}: {stopwatch.ElapsedMilliseconds}ms ({stressIterations}次迭代, 平均{averageTimePerCallNs:F2}ns/次)");
            }
        }

        /// <summary>
        /// 内存分配测试
        /// </summary>
        [Test]
        public void MemoryAllocation_Test()
        {
            UnityEngine.Debug.Log("=== 内存分配测试 ===");
            
            // 记录初始GC计数
            long initialGen0 = GC.CollectionCount(0);
            long initialGen1 = GC.CollectionCount(1);
            long initialGen2 = GC.CollectionCount(2);
            
            var allEases = GetAllEaseTypes();
            
            // 执行大量函数调用
            for (int i = 0; i < TestIterations; i++)
            {
                float t = (i % 100) / 100f;
                foreach (var ease in allEases)
                {
                    EaseUtility.Evaluate(t, ease);
                }
            }
            
            // 记录最终GC计数
            long finalGen0 = GC.CollectionCount(0);
            long finalGen1 = GC.CollectionCount(1);
            long finalGen2 = GC.CollectionCount(2);
            
            UnityEngine.Debug.Log($"GC统计 (迭代次数: {TestIterations * allEases.Length}):");
            UnityEngine.Debug.Log($"Gen0: {finalGen0 - initialGen0} 次");
            UnityEngine.Debug.Log($"Gen1: {finalGen1 - initialGen1} 次");
            UnityEngine.Debug.Log($"Gen2: {finalGen2 - initialGen2} 次");
            
            // 如果没有GC，说明没有内存分配
            if (finalGen0 == initialGen0 && finalGen1 == initialGen1 && finalGen2 == initialGen2)
            {
                UnityEngine.Debug.Log("✓ 无内存分配 - 所有函数调用都是零分配的");
            }
            else
            {
                UnityEngine.Debug.Log("⚠ 检测到内存分配 - 可能存在性能问题");
            }
        }

        /// <summary>
        /// 不同输入值的性能测试
        /// </summary>
        [Test]
        public void DifferentInputValues_Performance()
        {
            UnityEngine.Debug.Log("=== 不同输入值的性能测试 ===");
            
            var testEase = Ease.InOutCubic; // 选择一个中等复杂度的函数
            var inputValues = new[]
            {
                ("边界值0", 0f),
                ("边界值1", 1f),
                ("中间值", 0.5f),
                ("小值", 0.001f),
                ("大值", 0.999f),
                ("随机值1", 0.123f),
                ("随机值2", 0.789f)
            };
            
            foreach (var (name, value) in inputValues)
            {
                var stopwatch = Stopwatch.StartNew();
                
                for (int i = 0; i < TestIterations; i++)
                {
                    EaseUtility.Evaluate(value, testEase);
                }
                
                stopwatch.Stop();
                
                double averageTimePerCallNs = (double)stopwatch.ElapsedTicks * 1000.0 / TestIterations / Stopwatch.Frequency * 1000000.0;
                
                UnityEngine.Debug.Log($"{name} ({value}): {stopwatch.ElapsedMilliseconds}ms (平均{averageTimePerCallNs:F2}ns/次)");
            }
        }

        /// <summary>
        /// 性能结果数据结构
        /// </summary>
        private struct PerformanceResult
        {
            public long ExecutionTimeMs;
            public double AverageTimePerCallNs;
        }
    }
}
