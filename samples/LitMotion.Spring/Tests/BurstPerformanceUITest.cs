using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using Unity.Mathematics;
namespace ASUI.Test
{
    /// <summary>
    /// Burst性能测试UI脚本
    /// 可以在构建项目中直接显示性能测试结果
    /// </summary>
    public class BurstPerformanceUITest : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] public Text resultText;
        [SerializeField] public Button runTestButton;
        [SerializeField] public Button clearButton;
        [SerializeField] public Slider iterationSlider;
        [SerializeField] public Text iterationLabel;
        
        
        [Header("测试配置")]
        [SerializeField] public int minIterations = 10000;
        [SerializeField] public int maxIterations = 1000000;
        [SerializeField] public float deltaTime = 0.016f; // 60 FPS
        [SerializeField] public float stiffness = 5.0f;
        [SerializeField] public float dampingRatio = 0.5f;
        
        public List<TestResult> testResults = new List<TestResult>();
        
        [System.Serializable]
        public class TestResult
        {
            public string testName;
            public long milliseconds;
            public int iterations;
            public double operationsPerSecond;
            public double microsecondsPerOperation;
            public bool isBurstCompiled;
            
            public TestResult(string name, long ms, int iter, bool burst)
            {
                testName = name;
                milliseconds = ms;
                iterations = iter;
                isBurstCompiled = burst;
                operationsPerSecond = (double)iter / (ms / 1000.0);
                microsecondsPerOperation = (ms * 1000.0) / iter;
            }
        }
        
        public void Start()
        {
            DetectSystemLanguage();
            InitializeLocalizedStrings();
            InitializeUI();
            UpdateIterationLabel();
        }
        
        public void InitializeUI()
        {
            if (resultText == null)
            {
                resultText = GetComponentInChildren<Text>();
            }
            
            if (runTestButton != null)
            {
                runTestButton.onClick.AddListener(RunAllTests);
            }
            
            if (clearButton != null)
            {
                clearButton.onClick.AddListener(ClearResults);
            }
            
            if (iterationSlider != null)
            {
                iterationSlider.minValue = 0f;
                iterationSlider.maxValue = 1f;
                iterationSlider.value = 0.5f; // 默认中等迭代次数
                iterationSlider.onValueChanged.AddListener(OnIterationSliderChanged);
            }
            
            
            // 显示初始信息
            ShowInitialInfo();
        }
        
        public void ShowInitialInfo()
        {
            string info = $"=== {GetLocalizedString("BURST_PERFORMANCE_TEST")} ===\n";
            
            string environment = Application.isEditor ? 
                GetLocalizedString("EDITOR_JIT") : 
                GetLocalizedString("BUILD_BURST");
            info += $"{GetLocalizedString("CURRENT_ENVIRONMENT", environment)}\n";
            
            info += $"{GetLocalizedString("UNITY_VERSION", Application.unityVersion)}\n";
            info += $"{GetLocalizedString("PLATFORM", Application.platform.ToString())}\n";
            info += $"{GetLocalizedString("CLICK_RUN_TEST")}\n\n";
            
            info += $"{GetLocalizedString("TEST_DESCRIPTION")}\n";
            info += $"{GetLocalizedString("EDITOR_ENVIRONMENT_JIT")}\n";
            info += $"{GetLocalizedString("BUILD_ENVIRONMENT_BURST")}\n";
            info += $"{GetLocalizedString("COMPARE_SPRING_FUNCTIONS")}\n";
            info += $"{GetLocalizedString("COMPARE_FLOAT_FLOAT4")}\n";
            
            UpdateResultText(info);
        }
        
        public void OnIterationSliderChanged(float value)
        {
            UpdateIterationLabel();
        }
        
        public void UpdateIterationLabel()
        {
            if (iterationLabel != null && iterationSlider != null)
            {
                int iterations = GetCurrentIterations();
                iterationLabel.text = GetLocalizedString("ITERATION_COUNT", iterations.ToString("N0"));
            }
        }
        
        public int GetCurrentIterations()
        {
            if (iterationSlider == null) return minIterations;
            float t = iterationSlider.value;
            return Mathf.RoundToInt(Mathf.Lerp(minIterations, maxIterations, t));
        }
        
        public void RunAllTests()
        {
            testResults.Clear();
            int iterations = GetCurrentIterations();
            
            string progress = $"=== {GetLocalizedString("START_PERFORMANCE_TEST")} ===\n";
            progress += $"{GetLocalizedString("ITERATION_COUNT").Replace("{0}", iterations.ToString("N0"))}\n";
            
            string environment = Application.isEditor ? 
                GetLocalizedString("EDITOR_JIT") : 
                GetLocalizedString("BUILD_BURST");
            progress += $"{GetLocalizedString("ENVIRONMENT").Replace("{0}", environment)}\n\n";
            UpdateResultText(progress);
            
            // 直接测试各个Spring函数，无协程等待
            TestSpringFunction("SpringSimple", TestSpringSimple, iterations);
            TestSpringFunction("SpringElastic", TestSpringElastic, iterations);
            TestSpringFunction("SpringSimpleVelocitySmoothing", TestSpringSimpleVelocitySmoothing, iterations);
            TestSpringFunction("SpringSimpleDurationLimit", TestSpringSimpleDurationLimit, iterations);
            TestSpringFunction("SpringSimpleDoubleSmoothing", TestSpringSimpleDoubleSmoothing, iterations);
            
            // 添加float版本性能对比测试
            TestSpringFunction("SpringSimple_float", TestSpringSimpleFloat, iterations);
            TestSpringFunction("SpringElastic_float", TestSpringElasticFloat, iterations);
            TestSpringFunction("SpringSimpleVelocitySmoothing_float", TestSpringSimpleVelocitySmoothingFloat, iterations);
            TestSpringFunction("SpringSimpleDurationLimit_float", TestSpringSimpleDurationLimitFloat, iterations);
            TestSpringFunction("SpringSimpleDoubleSmoothing_float", TestSpringSimpleDoubleSmoothingFloat, iterations);
            
            // 显示最终结果
            ShowFinalResults();
        }
        
        public void TestSpringFunction(string functionName, System.Func<int, long> testFunc, int iterations)
        {
            string progress = $"{GetLocalizedString("TESTING_FUNCTION").Replace("{0}", functionName)}\n";
            AppendResultText(progress);
            
            // 直接执行测试，无协程等待
            long milliseconds = testFunc(iterations);
            bool isBurstCompiled = !Application.isEditor; // 构建版本使用Burst编译
            
            TestResult result = new TestResult(functionName, milliseconds, iterations, isBurstCompiled);
            testResults.Add(result);
            
            string resultText = $"✓ {functionName}: {milliseconds}ms | {result.operationsPerSecond:N0} ops/sec | {result.microsecondsPerOperation:F2} μs/op\n";
            AppendResultText(resultText);
        }
        
        public long TestSpringSimple(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            Vector4 currentValue = new Vector4(1, 2, 3, 4);
            Vector4 currentVelocity = new Vector4(0.1f, 0.2f, 0.3f, 0.4f);
            Vector4 targetValue = new Vector4(10, 20, 30, 40);
            
            for (int i = 0; i < iterations; i++)
            {
                float4 currentVal = (float4)currentValue;
                float4 currentVel = (float4)currentVelocity;
                float4 targetVal = (float4)targetValue;
                SpringUtility.SpringSimple(deltaTime, ref currentVal, ref currentVel, targetVal, stiffness);
                // 稍微改变输入值以避免编译器优化
                currentValue = new Vector4(currentValue.x + 0.0001f, currentValue.y + 0.0001f, currentValue.z + 0.0001f, currentValue.w + 0.0001f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        public long TestSpringElastic(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            Vector4 currentValue = new Vector4(1, 2, 3, 4);
            Vector4 currentVelocity = new Vector4(0.1f, 0.2f, 0.3f, 0.4f);
            Vector4 targetValue = new Vector4(10, 20, 30, 40);
            Vector4 targetVelocity = new Vector4(0, 0, 0, 0);
            
            for (int i = 0; i < iterations; i++)
            {
                float4 currentVal = (float4)currentValue;
                float4 currentVel = (float4)currentVelocity;
                float4 targetVal = (float4)targetValue;
                float4 targetVel = (float4)targetVelocity;
                SpringUtility.SpringElastic(deltaTime, ref currentVal, ref currentVel, targetVal, targetVel, dampingRatio, stiffness);
                currentValue = new Vector4(currentValue.x + 0.0001f, currentValue.y + 0.0001f, currentValue.z + 0.0001f, currentValue.w + 0.0001f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        public long TestSpringSimpleVelocitySmoothing(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            Vector4 currentValue = new Vector4(1, 2, 3, 4);
            Vector4 currentVelocity = new Vector4(0.1f, 0.2f, 0.3f, 0.4f);
            Vector4 targetValue = new Vector4(10, 20, 30, 40);
            float4 intermediatePosition = new float4(5, 10, 15, 20);
            
            for (int i = 0; i < iterations; i++)
            {
                float4 currentVal = (float4)currentValue;
                float4 currentVel = (float4)currentVelocity;
                float4 targetVal = (float4)targetValue;
                SpringUtility.SpringSimpleVelocitySmoothing(deltaTime, ref currentVal, ref currentVel, targetVal, ref intermediatePosition, 2.0f, stiffness);
                currentValue = new Vector4(currentValue.x + 0.0001f, currentValue.y + 0.0001f, currentValue.z + 0.0001f, currentValue.w + 0.0001f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        public long TestSpringSimpleDurationLimit(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            Vector4 currentValue = new Vector4(1, 2, 3, 4);
            Vector4 currentVelocity = new Vector4(0.1f, 0.2f, 0.3f, 0.4f);
            Vector4 targetValue = new Vector4(10, 20, 30, 40);
            float4 intermediatePosition = new float4(5, 10, 15, 20);
            
            for (int i = 0; i < iterations; i++)
            {
                float4 currentVal = (float4)currentValue;
                float4 currentVel = (float4)currentVelocity;
                float4 targetVal = (float4)targetValue;
                SpringUtility.SpringSimpleDurationLimit(deltaTime, ref currentVal, ref currentVel, ref targetVal, 0.2f);
                currentValue = new Vector4(currentValue.x + 0.0001f, currentValue.y + 0.0001f, currentValue.z + 0.0001f, currentValue.w + 0.0001f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        public long TestSpringSimpleDoubleSmoothing(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            Vector4 currentValue = new Vector4(1, 2, 3, 4);
            Vector4 currentVelocity = default;
            Vector4 targetValue = new Vector4(10, 20, 30, 40);
            Vector4 intermediatePosition = default;
            Vector4 intermediateVelocity = default;
            
            for (int i = 0; i < iterations; i++)
            {
                float4 currentVal = currentValue;
                float4 currentVel = currentVelocity;
                float4 targetVal = targetValue;
                float4 intermediatePos = intermediatePosition;
                float4 intermediateVel = intermediateVelocity;
                SpringUtility.SpringSimpleDoubleSmoothing(deltaTime, ref currentVal, ref currentVel, targetVal, ref intermediatePos, ref intermediateVel, stiffness);
                currentValue = new Vector4(currentValue.x + 0.0001f, currentValue.y + 0.0001f, currentValue.z + 0.0001f, currentValue.w + 0.0001f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        /// <summary>
        /// 测试float版本的SpringSimple函数
        /// </summary>
        public long TestSpringSimpleFloat(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            float currentValue = 1.0f;
            float currentVelocity = 0.1f;
            float targetValue = 10.0f;
            
            for (int i = 0; i < iterations; i++)
            {
                SpringUtility.SpringSimple(deltaTime, ref currentValue, ref currentVelocity, targetValue, stiffness);
                // 稍微改变输入值以避免编译器优化
                currentValue += 0.0001f;
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        /// <summary>
        /// 测试float版本的SpringElastic函数
        /// </summary>
        public long TestSpringElasticFloat(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            float currentValue = 1.0f;
            float currentVelocity = 0.1f;
            float targetValue = 10.0f;
            float targetVelocity = 0.0f;
            
            for (int i = 0; i < iterations; i++)
            {
                SpringUtility.SpringElastic(deltaTime, ref currentValue, ref currentVelocity, targetValue, targetVelocity, dampingRatio, stiffness);
                // 稍微改变输入值以避免编译器优化
                currentValue += 0.0001f;
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        /// <summary>
        /// 测试float版本的SpringSimpleVelocitySmoothing函数
        /// </summary>
        public long TestSpringSimpleVelocitySmoothingFloat(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            float currentValue = 1.0f;
            float currentVelocity = 0.1f;
            float targetValue = 10.0f;
            float intermediatePosition = 5.0f;
            
            for (int i = 0; i < iterations; i++)
            {
                SpringUtility.SpringSimpleVelocitySmoothing(deltaTime, ref currentValue, ref currentVelocity, targetValue, ref intermediatePosition, 2.0f, stiffness);
                // 稍微改变输入值以避免编译器优化
                currentValue += 0.0001f;
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        /// <summary>
        /// 测试float版本的SpringSimpleDurationLimit函数
        /// </summary>
        public long TestSpringSimpleDurationLimitFloat(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            float currentValue = 1.0f;
            float currentVelocity = 0.1f;
            float targetValue = 10.0f;
            
            for (int i = 0; i < iterations; i++)
            {
                SpringUtility.SpringSimpleDurationLimit(deltaTime, ref currentValue, ref currentVelocity, targetValue, 0.2f);
                // 稍微改变输入值以避免编译器优化
                currentValue += 0.0001f;
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        /// <summary>
        /// 测试float版本的SpringSimpleDoubleSmoothing函数
        /// </summary>
        public long TestSpringSimpleDoubleSmoothingFloat(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            float currentValue = 1.0f;
            float currentVelocity = 0.1f;
            float targetValue = 10.0f;
            float intermediatePosition = 5.0f;
            float intermediateVelocity = 0.05f;
            
            for (int i = 0; i < iterations; i++)
            {
                SpringUtility.SpringSimpleDoubleSmoothing(deltaTime, ref currentValue, ref currentVelocity, targetValue, ref intermediatePosition, ref intermediateVelocity, stiffness);
                // 稍微改变输入值以避免编译器优化
                currentValue += 0.0001f;
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        public void ShowFinalResults()
        {
            string results = $"\n=== {GetLocalizedString("TEST_RESULTS_SUMMARY")} ===\n";
            
            string environment = Application.isEditor ? 
                GetLocalizedString("EDITOR_JIT") : 
                GetLocalizedString("BUILD_BURST");
            results += $"{GetLocalizedString("ENVIRONMENT").Replace("{0}", environment)}\n";
            results += $"{GetLocalizedString("TEST_TIME").Replace("{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))}\n\n";
            
            // 按性能排序
            testResults.Sort((a, b) => a.milliseconds.CompareTo(b.milliseconds));
            
            results += $"{GetLocalizedString("PERFORMANCE_RANKING")}\n";
            for (int i = 0; i < testResults.Count; i++)
            {
                var result = testResults[i];
                string burstStatus = result.isBurstCompiled ? 
                    GetLocalizedString("BURST_COMPILED") : 
                    GetLocalizedString("JIT_COMPILED");
                results += $"{i + 1}. {result.testName} {burstStatus}: {result.milliseconds}ms ({result.operationsPerSecond:N0} ops/sec)\n";
            }
            
            results += $"\n{GetLocalizedString("DETAILED_RESULTS")}\n";
            foreach (var result in testResults)
            {
                string burstStatus = result.isBurstCompiled ? 
                    GetLocalizedString("BURST_COMPILED") : 
                    GetLocalizedString("JIT_COMPILED");
                results += $"\n{result.testName} {burstStatus}:\n";
                results += $"  {GetLocalizedString("TOTAL_TIME").Replace("{0}", result.milliseconds.ToString())}\n";
                results += $"  {GetLocalizedString("OPERATIONS").Replace("{0}", result.iterations.ToString("N0"))}\n";
                results += $"  {GetLocalizedString("OPERATIONS_PER_SECOND").Replace("{0}", result.operationsPerSecond.ToString("N0"))}\n";
                results += $"  {GetLocalizedString("MICROSECONDS_PER_OPERATION").Replace("{0}", result.microsecondsPerOperation.ToString("F2"))}\n";
            }
            
            // 添加SIMD性能分析
            results += $"\n=== {GetLocalizedString("SIMD_PERFORMANCE_ANALYSIS")} ===\n";
            var vector4Result = testResults.Find(r => r.testName == "SpringSimple_Vector4");
            var float4Result = testResults.Find(r => r.testName == "SpringSimple_float4");
            
            if (vector4Result != null && float4Result != null)
            {
                double simdSpeedup = (double)vector4Result.milliseconds / float4Result.milliseconds;
                results += $"{GetLocalizedString("VECTOR4_NO_SIMD").Replace("{0}", vector4Result.milliseconds.ToString())}\n";
                results += $"{GetLocalizedString("FLOAT4_WITH_SIMD").Replace("{0}", float4Result.milliseconds.ToString())}\n";
                results += $"{GetLocalizedString("SIMD_SPEEDUP").Replace("{0}", simdSpeedup.ToString("F2"))}\n";
                
                if (simdSpeedup > 2.0)
                {
                    results += $"{GetLocalizedString("SIMD_SIGNIFICANT_EFFECT")}\n";
                }
                else if (simdSpeedup > 1.2)
                {
                    results += $"{GetLocalizedString("SIMD_MODERATE_EFFECT")}\n";
                }
                else
                {
                    results += $"{GetLocalizedString("SIMD_NO_EFFECT")}\n";
                }
                
                results += $"\n{GetLocalizedString("EXPLANATION")}\n";
                results += $"{GetLocalizedString("VECTOR4_TRADITIONAL")}\n";
                results += $"{GetLocalizedString("FLOAT4_SIMD")}\n";
                results += $"{GetLocalizedString("SIMD_PLATFORM_SUPPORT")}\n";
            }
            
            // 添加float vs float4性能对比分析
            results += $"\n=== {GetLocalizedString("FLOAT_VS_FLOAT4_PERFORMANCE")} ===\n";
            
            // SpringSimple对比
            var springSimpleFloat4Result = testResults.Find(r => r.testName == "SpringSimple");
            var springSimpleFloatResult = testResults.Find(r => r.testName == "SpringSimple_float");
            
            if (springSimpleFloat4Result != null && springSimpleFloatResult != null)
            {
                double speedupRatio = (double)springSimpleFloatResult.milliseconds / springSimpleFloat4Result.milliseconds;
                results += $"{GetLocalizedString("SPRING_SIMPLE_COMPARISON")}\n";
                results += $"  {GetLocalizedString("FLOAT4_BURST").Replace("{0}", springSimpleFloat4Result.milliseconds.ToString())}\n";
                results += $"  {GetLocalizedString("FLOAT_JIT").Replace("{0}", springSimpleFloatResult.milliseconds.ToString())}\n";
                results += $"  {GetLocalizedString("FLOAT4_SPEEDUP").Replace("{0}", speedupRatio.ToString("F2"))}\n";
            }
            
            // SpringElastic对比
            var springElasticFloat4Result = testResults.Find(r => r.testName == "SpringElastic");
            var springElasticFloatResult = testResults.Find(r => r.testName == "SpringElastic_float");
            
            if (springElasticFloat4Result != null && springElasticFloatResult != null)
            {
                double speedupRatio = (double)springElasticFloatResult.milliseconds / springElasticFloat4Result.milliseconds;
                results += $"{GetLocalizedString("SPRING_ELASTIC_COMPARISON")}\n";
                results += $"  {GetLocalizedString("FLOAT4_BURST").Replace("{0}", springElasticFloat4Result.milliseconds.ToString())}\n";
                results += $"  {GetLocalizedString("FLOAT_JIT").Replace("{0}", springElasticFloatResult.milliseconds.ToString())}\n";
                results += $"  {GetLocalizedString("FLOAT4_SPEEDUP").Replace("{0}", speedupRatio.ToString("F2"))}\n";
            }
            
            // 计算平均加速比
            var float4Results = testResults.Where(r => !r.testName.Contains("_float")).ToList();
            var floatResults = testResults.Where(r => r.testName.Contains("_float")).ToList();
            
            if (float4Results.Count > 0 && floatResults.Count > 0)
            {
                double totalFloat4Time = float4Results.Sum(r => r.milliseconds);
                double totalFloatTime = floatResults.Sum(r => r.milliseconds);
                double averageSpeedup = totalFloatTime / totalFloat4Time;
                
                results += $"\n{GetLocalizedString("OVERALL_PERFORMANCE_COMPARISON")}\n";
                results += $"  {GetLocalizedString("FLOAT4_TOTAL_TIME").Replace("{0}", totalFloat4Time.ToString())}\n";
                results += $"  {GetLocalizedString("FLOAT_TOTAL_TIME").Replace("{0}", totalFloatTime.ToString())}\n";
                results += $"  {GetLocalizedString("AVERAGE_SPEEDUP").Replace("{0}", averageSpeedup.ToString("F2"))}\n";
                
                if (averageSpeedup > 2.0)
                {
                    results += $"{GetLocalizedString("FLOAT4_SIGNIFICANTLY_BETTER")}\n";
                }
                else if (averageSpeedup > 1.2)
                {
                    results += $"{GetLocalizedString("FLOAT4_MODERATE_ADVANTAGE")}\n";
                }
                else
                {
                    results += $"{GetLocalizedString("NO_SIGNIFICANT_DIFFERENCE")}\n";
                }
                
                results += $"\n{GetLocalizedString("EXPLANATION")}\n";
                results += $"{GetLocalizedString("DOUBLE_VERSION_EXPLANATION")}\n";
                results += $"{GetLocalizedString("FLOAT4_VERSION_EXPLANATION")}\n";
                results += $"{GetLocalizedString("SIMD_PLATFORM_PERFORMANCE")}\n";
                results += $"{GetLocalizedString("BUILD_VERSION_BURST_EFFECT")}\n";
            }
            
            results += $"\n=== {GetLocalizedString("TEST_COMPLETED")} ===\n";
            results += $"{GetLocalizedString("BUILD_VERSION_BURST_PERFORMANCE")}\n";
            
            UpdateResultText(results);
        }
        
        public void UpdateResultText(string text)
        {
            if (resultText != null)
            {
                resultText.text = text;
            }
        }
        
        public void AppendResultText(string text)
        {
            if (resultText != null)
            {
                resultText.text += text;
            }
        }
        
        public void ClearResults()
        {
            testResults.Clear();
            ShowInitialInfo();
        }
        
        // 用于测试非Burst编译版本的对比函数
        [System.Obsolete("仅用于性能对比测试")]
        public long TestSpringSimpleNonBurst(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            Vector4 currentValue = new Vector4(1, 2, 3, 4);
            Vector4 currentVelocity = new Vector4(0.1f, 0.2f, 0.3f, 0.4f);
            Vector4 targetValue = new Vector4(10, 20, 30, 40);
            
            for (int i = 0; i < iterations; i++)
            {
                // 使用Unity的Vector4进行非Burst计算
                Vector4 displacement = currentValue - targetValue;
                Vector4 velocityWithDamping = currentVelocity + displacement * stiffness;
                float exponentialDecay = Mathf.Exp(-stiffness * deltaTime);
                
                Vector4 newPosition = exponentialDecay * (displacement + velocityWithDamping * deltaTime) + targetValue;
                Vector4 newVelocity = exponentialDecay * (currentVelocity - velocityWithDamping * stiffness * deltaTime);
                
                currentValue = new Vector4(currentValue.x + 0.0001f, currentValue.y + 0.0001f, currentValue.z + 0.0001f, currentValue.w + 0.0001f);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        // ==================== 多语言支持 ====================
        
        [Header("多语言支持")]
        [SerializeField] public bool useMultiLanguage = true;
        
        // 多语言字典
        private Dictionary<string, string[]> localizedStrings;
        private int currentLanguageIndex = 0; // 0=英文, 1=中文, 2=日文
        
        /// <summary>
        /// 初始化多语言字符串
        /// </summary>
        private void InitializeLocalizedStrings()
        {
            localizedStrings = new Dictionary<string, string[]>();
            
            // 性能测试相关字符串 [英文, 中文, 日文]
            localizedStrings["BURST_PERFORMANCE_TEST"] = new string[] {
                "Burst Performance Test Tool",
                "Burst性能测试工具", 
                "Burstパフォーマンステストツール"
            };
            
            localizedStrings["CURRENT_ENVIRONMENT"] = new string[] {
                "Current Environment: {0}",
                "当前环境: {0}",
                "現在の環境: {0}"
            };
            
            localizedStrings["EDITOR_JIT"] = new string[] {
                "Editor (JIT)",
                "编辑器 (JIT)",
                "エディター (JIT)"
            };
            
            localizedStrings["BUILD_BURST"] = new string[] {
                "Build Version (Burst)",
                "构建版本 (Burst)",
                "ビルド版 (Burst)"
            };
            
            localizedStrings["UNITY_VERSION"] = new string[] {
                "Unity Version: {0}",
                "Unity版本: {0}",
                "Unityバージョン: {0}"
            };
            
            localizedStrings["PLATFORM"] = new string[] {
                "Platform: {0}",
                "平台: {0}",
                "プラットフォーム: {0}"
            };
            
            localizedStrings["CLICK_RUN_TEST"] = new string[] {
                "Click 'Run Test' to start performance testing",
                "点击'运行测试'开始性能测试",
                "'テスト実行'をクリックしてパフォーマンステストを開始"
            };
            
            localizedStrings["TEST_DESCRIPTION"] = new string[] {
                "Test Description:",
                "测试说明:",
                "テスト説明:"
            };
            
            localizedStrings["EDITOR_ENVIRONMENT_JIT"] = new string[] {
                "• Editor environment uses JIT compilation",
                "• 编辑器环境使用JIT编译",
                "• エディター環境はJITコンパイルを使用"
            };
            
            localizedStrings["BUILD_ENVIRONMENT_BURST"] = new string[] {
                "• Build environment uses Burst compilation",
                "• 构建环境使用Burst编译",
                "• ビルド環境はBurstコンパイルを使用"
            };
            
            localizedStrings["COMPARE_SPRING_FUNCTIONS"] = new string[] {
                "• Compare performance of different Spring functions",
                "• 对比不同Spring函数的性能",
                "• 異なるSpring関数のパフォーマンスを比較"
            };
            
            localizedStrings["COMPARE_FLOAT_FLOAT4"] = new string[] {
                "• Compare float vs float4 performance differences",
                "• 对比float vs float4版本的性能差异",
                "• float vs float4のパフォーマンス差異を比較"
            };
            
            localizedStrings["ITERATION_COUNT"] = new string[] {
                "Iteration Count: {0}",
                "迭代次数: {0}",
                "反復回数: {0}"
            };
            
            localizedStrings["START_PERFORMANCE_TEST"] = new string[] {
                "Start Performance Test",
                "开始性能测试",
                "パフォーマンステスト開始"
            };
            
            localizedStrings["ENVIRONMENT"] = new string[] {
                "Environment: {0}",
                "环境: {0}",
                "環境: {0}"
            };
            
            localizedStrings["TESTING_FUNCTION"] = new string[] {
                "Testing {0}...",
                "正在测试 {0}...",
                "{0}をテスト中..."
            };
            
            localizedStrings["TEST_RESULTS_SUMMARY"] = new string[] {
                "Test Results Summary",
                "测试结果汇总",
                "テスト結果サマリー"
            };
            
            localizedStrings["TEST_TIME"] = new string[] {
                "Test Time: {0}",
                "测试时间: {0}",
                "テスト時間: {0}"
            };
            
            localizedStrings["PERFORMANCE_RANKING"] = new string[] {
                "Performance Ranking (Fast to Slow):",
                "性能排名 (从快到慢):",
                "パフォーマンスランキング (速い順):"
            };
            
            localizedStrings["DETAILED_RESULTS"] = new string[] {
                "Detailed Results:",
                "详细结果:",
                "詳細結果:"
            };
            
            localizedStrings["TOTAL_TIME"] = new string[] {
                "Total Time: {0}ms",
                "总时间: {0}ms",
                "総時間: {0}ms"
            };
            
            localizedStrings["OPERATIONS"] = new string[] {
                "Operations: {0:N0}",
                "操作数: {0:N0}",
                "操作数: {0:N0}"
            };
            
            localizedStrings["OPERATIONS_PER_SECOND"] = new string[] {
                "Operations per Second: {0:N0} ops/sec",
                "每秒操作: {0:N0} ops/sec",
                "秒間操作数: {0:N0} ops/sec"
            };
            
            localizedStrings["MICROSECONDS_PER_OPERATION"] = new string[] {
                "Microseconds per Operation: {0:F2} μs/op",
                "每操作耗时: {0:F2} μs/op",
                "操作あたりの時間: {0:F2} μs/op"
            };
            
            localizedStrings["BURST_COMPILED"] = new string[] {
                "[Burst Compiled]",
                "[Burst编译]",
                "[Burstコンパイル]"
            };
            
            localizedStrings["JIT_COMPILED"] = new string[] {
                "[JIT Compiled]",
                "[JIT编译]",
                "[JITコンパイル]"
            };
            
            localizedStrings["TEST_COMPLETED"] = new string[] {
                "Test Completed",
                "测试完成",
                "テスト完了"
            };
            
            localizedStrings["BUILD_VERSION_BURST_PERFORMANCE"] = new string[] {
                "Tip: Run this test in build version to see real Burst compilation performance improvement",
                "提示: 在构建版本中运行此测试可以看到Burst编译的真实性能提升",
                "ヒント: ビルド版でこのテストを実行すると、Burstコンパイルの実際のパフォーマンス向上を確認できます"
            };
            
            // 新增float vs float4性能对比相关字符串
            localizedStrings["FLOAT_VS_FLOAT4_PERFORMANCE"] = new string[] {
                "Float vs Float4 Performance Analysis",
                "Float vs Float4性能分析",
                "Float vs Float4パフォーマンス分析"
            };
            
            localizedStrings["FLOAT_JIT"] = new string[] {
                "Float (JIT): {0}ms",
                "Float (JIT): {0}ms",
                "Float (JIT): {0}ms"
            };
            
            localizedStrings["FLOAT_TOTAL_TIME"] = new string[] {
                "Float Total Time: {0}ms",
                "Float总时间: {0}ms",
                "Float総時間: {0}ms"
            };
        }
        
        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        private string GetLocalizedString(string key, params object[] args)
        {
            if (!useMultiLanguage || !localizedStrings.ContainsKey(key))
            {
                return key; // 回退到键名
            }
            
            string[] languages = localizedStrings[key];
            if (currentLanguageIndex >= 0 && currentLanguageIndex < languages.Length)
            {
                string text = languages[currentLanguageIndex];
                if (args != null && args.Length > 0)
                {
                    return string.Format(text, args);
                }
                return text;
            }
            
            return key; // 回退到键名
        }
        
        /// <summary>
        /// 自动检测系统语言
        /// </summary>
        private void DetectSystemLanguage()
        {
            // 使用C#的CultureInfo来检测当前系统文化
            var currentCulture = System.Globalization.CultureInfo.CurrentCulture;
            string cultureName = currentCulture.Name.ToLower();
            
            // 检测语言
            if (cultureName.StartsWith("ja") || cultureName.StartsWith("jp"))
            {
                currentLanguageIndex = 2; // 日文
                UnityEngine.Debug.Log("Detected Japanese system language");
            }
            else if (cultureName.StartsWith("zh"))
            {
                currentLanguageIndex = 1; // 中文
                UnityEngine.Debug.Log("Detected Chinese system language");
            }
            else
            {
                currentLanguageIndex = 0; // 英文（默认）
                UnityEngine.Debug.Log("Using English as default language");
            }
            
            UnityEngine.Debug.Log($"System Culture: {currentCulture.Name}, Language Index: {currentLanguageIndex}");
        }
    }
}
