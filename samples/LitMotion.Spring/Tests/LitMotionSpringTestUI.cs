using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitMotion;
using System.Collections.Generic;
using Unity.Mathematics;
using LitMotion.Adapters;

namespace ASUI.Tests
{
    /// <summary>
    /// LitMotion.Spring可视化测试UI
    /// 用于测试各种Spring动画配置的行为和参数效果
    /// </summary>
    public class LitMotionSpringTestUI : MonoBehaviour
    {
        [Header("UI Controls")]
        [SerializeField] private Slider timeScaleSlider;
        [SerializeField] private TextMeshProUGUI timeScaleText;
        [SerializeField] private Slider dampingRatioSlider;
        [SerializeField] private TextMeshProUGUI dampingRatioText;
        [SerializeField] private Slider stiffnessSlider;
        [SerializeField] private TextMeshProUGUI stiffnessText;
        [SerializeField] private Slider targetValueSlider;
        [SerializeField] private TextMeshProUGUI targetValueText;
        [SerializeField] private Slider delaySlider;
        [SerializeField] private TextMeshProUGUI delayText;
        [SerializeField] private Slider loopCountSlider;
        [SerializeField] private TextMeshProUGUI loopCountText;
        [SerializeField] private Dropdown loopTypeDropdown;
        [SerializeField] private Dropdown delayTypeDropdown;
        
        [Header("Spring Test Sliders")]
        [SerializeField] private Slider spring1DSlider;
        [SerializeField] private Slider spring2DSlider;
        [SerializeField] private Slider spring3DSlider;
        [SerializeField] private Slider spring4DSlider;
        [SerializeField] private Slider performanceSlider;
        
        [Header("Control Buttons")]
        [SerializeField] private Button setRandomTargetButton;
        [SerializeField] private Button setHalfTargetButton;
        [SerializeField] private Button testAllCombinationsButton;
        
        [Header("Test Buttons")]
        [SerializeField] private Button test1DButton;
        [SerializeField] private Button stop1DButton;
        [SerializeField] private Button test2DButton;
        [SerializeField] private Button stop2DButton;
        [SerializeField] private Button test3DButton;
        [SerializeField] private Button stop3DButton;
        [SerializeField] private Button test4DButton;
        [SerializeField] private Button stop4DButton;
        [SerializeField] private Button testPerformanceButton;
        [SerializeField] private Button stopPerformanceButton;
        
        [Header("Performance Test UI")]
        [SerializeField] private Text performanceResultText;
        
        // Spring动画句柄
        private MotionHandle spring1DMotion = MotionHandle.None;
        private MotionHandle spring2DMotion = MotionHandle.None;
        private MotionHandle spring3DMotion = MotionHandle.None;
        private MotionHandle spring4DMotion = MotionHandle.None;
        private MotionHandle performanceMotion = MotionHandle.None;
        
        // 参数值
        private float dampingRatio = 0.5f;
        private float stiffness = 10.0f;
        private float targetValue = 0f;
        private float delay = 0f;
        private float loopCount = 0f;
        private int loopTypeIndex = 0;
        private int delayTypeIndex = 0;
        
        // 性能测试
        private int performanceTestCount = 0;
        private float performanceTestStartTime = 0f;
        private bool isPerformanceTestRunning = false;
        private bool isSpringTest = true; // true为Spring测试，false为Tween测试
        private float springTestTime = 0f;
        private float tweenTestTime = 0f;
        private List<MotionHandle> performanceMotions = new List<MotionHandle>();
        private float performanceTestDuration = 3f; // 运行性能测试持续时间
        private bool isComparingTest = false; // 是否正在进行比较测试
        private int currentTestPhase = 0; // 0=Spring测试, 1=Tween测试, 2=完成
        
        private float timeScale
        {
            get => UnityEngine.Time.timeScale;
            set => UnityEngine.Time.timeScale = value;
        }
        
        private void Start()
        {
            DetectSystemLanguage();
            InitializeLocalizedStrings();
            AutoAssignUIReferences();
            SetupUI();
        }
        
        private void SetupUI()
        {
            // 设置时间缩放控制
            if (timeScaleSlider != null)
            {
                timeScaleSlider.minValue = 0.1f;
                timeScaleSlider.maxValue = 3.0f;
                timeScaleSlider.value = 1.0f;
                timeScaleSlider.onValueChanged.AddListener(OnTimeScaleChanged);
                timeScale = timeScaleSlider.value;
            }
            
            // 设置Spring参数控制
            if (dampingRatioSlider != null)
            {
                dampingRatioSlider.minValue = 0.1f;
                dampingRatioSlider.maxValue = 2.0f;
                dampingRatioSlider.value = 1f;
                dampingRatioSlider.onValueChanged.AddListener(OnDampingRatioChanged);
                dampingRatio = dampingRatioSlider.value;
            }
            
            if (stiffnessSlider != null)
            {
                stiffnessSlider.minValue = 1f;
                stiffnessSlider.maxValue = 20f;
                stiffnessSlider.value = 10f;
                stiffnessSlider.onValueChanged.AddListener(OnStiffnessChanged);
                stiffness = stiffnessSlider.value;
            }
            
            if (targetValueSlider != null)
            {
                targetValueSlider.minValue = 0f;
                targetValueSlider.maxValue = 1f;
                targetValueSlider.value = 1f;
                targetValueSlider.onValueChanged.AddListener(OnTargetValueChanged);
                targetValue = targetValueSlider.value;
            }
            
            if (delaySlider != null)
            {
                delaySlider.minValue = 0f;
                delaySlider.maxValue = 3f;
                delaySlider.value = 0f;
                delaySlider.onValueChanged.AddListener(OnDelayChanged);
                delay = delaySlider.value;
            }
            
            if (loopCountSlider != null)
            {
                loopCountSlider.minValue = -1f;
                loopCountSlider.maxValue = 5f;
                loopCountSlider.value = 0f;
                loopCountSlider.onValueChanged.AddListener(OnLoopCountChanged);
                loopCount = loopCountSlider.value;
            }
            
            // 设置循环类型选择
            if (loopTypeDropdown != null)
            {
                loopTypeDropdown.onValueChanged.AddListener(OnLoopTypeChanged);
                loopTypeIndex = loopTypeDropdown.value;
            }
            
            // 设置延迟类型选择
            if (delayTypeDropdown != null)
            {
                delayTypeDropdown.onValueChanged.AddListener(OnDelayTypeChanged);
                delayTypeIndex = delayTypeDropdown.value;
            }
            
            UpdateUI();
        }
        
        private void SetupButtonEvents()
        {
            // 控制按钮
            if (setRandomTargetButton != null)
            {
                setRandomTargetButton.onClick.RemoveAllListeners();
                setRandomTargetButton.onClick.AddListener(SetRandomTargetValue);
            }
            if (setHalfTargetButton != null)
            {
                setHalfTargetButton.onClick.RemoveAllListeners();
                setHalfTargetButton.onClick.AddListener(SetTargetValueToHalf);
            }
            
            // 测试按钮
            if (test1DButton != null)
            {
                test1DButton.onClick.RemoveAllListeners();
                test1DButton.onClick.AddListener(Test1DSpring);
            }
            if (stop1DButton != null)
            {
                stop1DButton.onClick.RemoveAllListeners();
                stop1DButton.onClick.AddListener(() => SafeStopMotion(ref spring1DMotion));
            }
            
            if (test2DButton != null)
            {
                test2DButton.onClick.RemoveAllListeners();
                test2DButton.onClick.AddListener(Test2DSpring);
            }
            if (stop2DButton != null)
            {
                stop2DButton.onClick.RemoveAllListeners();
                stop2DButton.onClick.AddListener(() => SafeStopMotion(ref spring2DMotion));
            }
            
            if (test3DButton != null)
            {
                test3DButton.onClick.RemoveAllListeners();
                test3DButton.onClick.AddListener(Test3DSpring);
            }
            if (stop3DButton != null)
            {
                stop3DButton.onClick.RemoveAllListeners();
                stop3DButton.onClick.AddListener(() => SafeStopMotion(ref spring3DMotion));
            }
            
            if (test4DButton != null)
            {
                test4DButton.onClick.RemoveAllListeners();
                test4DButton.onClick.AddListener(Test4DSpring);
            }
            if (stop4DButton != null)
            {
                stop4DButton.onClick.RemoveAllListeners();
                stop4DButton.onClick.AddListener(() => SafeStopMotion(ref spring4DMotion));
            }
            
            if (testPerformanceButton != null)
            {
                testPerformanceButton.onClick.RemoveAllListeners();
                testPerformanceButton.onClick.AddListener(StartPerformanceTest);
            }
            if (stopPerformanceButton != null)
            {
                stopPerformanceButton.onClick.RemoveAllListeners();
                stopPerformanceButton.onClick.AddListener(StopAllAnimations);
            }
            
            if (testAllCombinationsButton != null)
            {
                testAllCombinationsButton.onClick.RemoveAllListeners();
                testAllCombinationsButton.onClick.AddListener(TestAllCombinations);
            }
        }
        
        private void Update()
        {
            // 更新性能测试
            if (isPerformanceTestRunning)
            {
                UpdatePerformanceTest();
            }
        }
        
        private void UpdatePerformanceTest()
        {
            if (performanceSlider != null)
            {
                float elapsed = UnityEngine.Time.time - performanceTestStartTime;
                float progress = Mathf.Clamp01(elapsed / performanceTestDuration);
                performanceSlider.value = progress;
                
                // 实时更新性能测试状态到UI
                UpdatePerformanceUI();
                
                if (progress >= 1f)
                {
                    isPerformanceTestRunning = false;
                    string animationType = isSpringTest ? "Spring" : "Tween";
                    
                    // 统计运行性能
                    float frameRate = 1f / UnityEngine.Time.unscaledDeltaTime;
                    Debug.Log($"{animationType}运行性能测试完成 - 运行了 {performanceTestCount} 个{animationType}动画，平均帧率: {frameRate:F1} FPS");
                    
                    // 记录性能数据
                    if (isSpringTest)
                    {
                        springTestTime = frameRate;
                        currentTestPhase = 2; // 测试完成
                    }
                    else
                    {
                        tweenTestTime = frameRate;
                        currentTestPhase = 1; // 进入Spring测试阶段
                    }
                    
                    // 清理动画
                    ClearPerformanceMotions();
                    
                    // 如果是比较测试，自动进行下一阶段
                    if (isComparingTest)
                    {
                        if (currentTestPhase == 1)
                        {
                            // 开始Spring测试
                            StartSinglePerformanceTest(true);
                        }
                        else if (currentTestPhase == 2)
                        {
                            // 比较测试完成，显示结果
                            isComparingTest = false;
                            ShowPerformanceComparison();
                            // 更新UI显示结果
                            UpdatePerformanceUI();
                        }
                    }
                    else
                    {
                        // 单次测试完成
                        if (springTestTime > 0f && tweenTestTime > 0f)
                        {
                            ShowPerformanceComparison();
                        }
                        
                        // 更新UI显示结果
                        UpdatePerformanceUI();
                        
                        // 切换测试类型，为下次测试做准备
                        isSpringTest = !isSpringTest;
                        Debug.Log($"下次将测试{(isSpringTest ? "Spring" : "Tween")}动画性能");
                    }
                }
            }
        }
        
        private void UpdatePerformanceUI()
        {
            if (performanceResultText == null) 
            {
                Debug.LogWarning("performanceResultText为null，无法更新UI！");
                return;
            }
            
            if (isPerformanceTestRunning)
            {
                float elapsed = UnityEngine.Time.time - performanceTestStartTime;
                float progress = Mathf.Clamp01(elapsed / performanceTestDuration);
                float currentFrameRate = 1f / UnityEngine.Time.unscaledDeltaTime;
                string animationType = isSpringTest ? "Spring" : "Tween";
                
                if (isComparingTest)
                {
                    string phaseText = currentTestPhase == 0 ? "第一阶段" : "第二阶段";
                    string phaseDescription = currentTestPhase == 0 ? "Tween动画" : "Spring动画";
                    performanceResultText.text = $"性能比较测试进行中...\n" +
                                              $"{phaseText}: {phaseDescription}\n" +
                                              $"动画数量: {performanceTestCount:N0}\n" +
                                              $"进度: {progress:P1}\n" +
                                              $"当前帧率: {currentFrameRate:F1} FPS";
                }
                else
                {
                    performanceResultText.text = $"性能测试进行中...\n" +
                                              $"测试类型: {animationType}动画\n" +
                                              $"动画数量: {performanceTestCount:N0}\n" +
                                              $"进度: {progress:P1}\n" +
                                              $"当前帧率: {currentFrameRate:F1} FPS";
                }
            }
            else
            {
                if (springTestTime > 0f && tweenTestTime > 0f)
                {
                    // 显示性能比较结果
                    float ratio = springTestTime / tweenTestTime;
                    string betterAnimation = ratio > 1f ? "Spring" : "Tween";
                    float performanceRatio = ratio > 1f ? ratio : 1f / ratio;
                    
                    string resultText = $"=== 性能比较测试结果 ===\n" +
                                      $"Spring动画帧率: {springTestTime:F1} FPS\n" +
                                      $"Tween动画帧率: {tweenTestTime:F1} FPS\n" +
                                      $"{betterAnimation}动画性能更好\n" +
                                      $"性能差异: {performanceRatio:F2} 倍\n" +
                                      $"点击'性能测试'重新开始比较";
                    
                    performanceResultText.text = resultText;
                    Debug.Log("更新性能比较结果到UI: " + resultText);
                }
                else if (springTestTime > 0f || tweenTestTime > 0f)
                {
                    // 显示单个测试结果
                    string animationType = springTestTime > 0f ? "Spring" : "Tween";
                    float frameRate = springTestTime > 0f ? springTestTime : tweenTestTime;
                    
                    performanceResultText.text = $"{animationType}动画测试完成\n" +
                                              $"帧率: {frameRate:F1} FPS\n" +
                                              $"动画数量: {performanceTestCount:N0}\n" +
                                              $"点击'性能测试'开始完整比较";
                }
                else
                {
                    performanceResultText.text = "点击'性能测试'开始比较测试\n" +
                                              $"将自动测试Tween和Spring动画\n" +
                                              $"测试动画数量: 1,000,000\n" +
                                              $"每个阶段持续时间: {performanceTestDuration}秒";
                }
            }
        }
        
        private void ClearPerformanceMotions()
        {
            foreach (var motion in performanceMotions)
            {
                if (motion.IsActive())
                {
                    try
                    {
                        motion.Cancel();
                    }
                    catch (System.ArgumentException)
                    {
                        // 忽略无效的MotionHandle错误
                    }
                }
            }
            performanceMotions.Clear();
        }
        
        private void ShowPerformanceComparison()
        {
            Debug.Log("=== 运行性能测试比较结果 ===");
            Debug.Log($"Spring动画运行帧率: {springTestTime:F1} FPS");
            Debug.Log($"Tween动画运行帧率: {tweenTestTime:F1} FPS");
            
            if (springTestTime > 0f && tweenTestTime > 0f)
            {
                float ratio = springTestTime / tweenTestTime;
                if (ratio > 1f)
                {
                    Debug.Log($"Spring动画比Tween动画性能好 {ratio:F2} 倍 (帧率更高)");
                }
                else
                {
                    Debug.Log($"Tween动画比Spring动画性能好 {1f/ratio:F2} 倍 (帧率更高)");
                }
            }
            Debug.Log("=============================");
            
            // 确保UI更新
            if (performanceResultText != null)
            {
                Debug.Log("正在更新性能测试结果UI...");
            }
            else
            {
                Debug.LogWarning("performanceResultText为null，无法更新UI！");
            }
        }
        
        // 测试方法
        [ContextMenu("测试1D Spring")]
        public void Test1DSpring()
        {
            TestSpring1D();
        }
        
        [ContextMenu("测试2D Spring")]
        public void Test2DSpring()
        {
            TestSpring2D();
        }
        
        [ContextMenu("测试3D Spring")]
        public void Test3DSpring()
        {
            TestSpring3D();
        }
        
        [ContextMenu("测试4D Spring")]
        public void Test4DSpring()
        {
            TestSpring4D();
        }
        
        [ContextMenu("性能测试")]
        public void TestPerformance()
        {
            StartPerformanceTest();
        }
        
        [ContextMenu("切换测试类型")]
        public void ToggleTestType()
        {
            isSpringTest = !isSpringTest;
            string animationType = isSpringTest ? "Spring" : "Tween";
            Debug.Log($"测试类型已切换为: {animationType}动画");
        }
        
        [ContextMenu("重置性能测试数据")]
        public void ResetPerformanceTestData()
        {
            springTestTime = 0f;
            tweenTestTime = 0f;
            isSpringTest = true;
            isComparingTest = false;
            currentTestPhase = 0;
            ClearPerformanceMotions();
            Debug.Log("性能测试数据已重置");
        }
        
        [ContextMenu("停止所有动画")]
        public void StopAllAnimations()
        {
            SafeCompleteMotion(ref spring1DMotion);
            SafeCompleteMotion(ref spring2DMotion);
            SafeCompleteMotion(ref spring3DMotion);
            SafeCompleteMotion(ref spring4DMotion);
            SafeCompleteMotion(ref performanceMotion);
        }
        
        public void SetRandomTargetValue()
        {
            float randomTarget = UnityEngine.Random.Range(0.2f, 0.8f);
            if (targetValueSlider != null)
            {
                targetValueSlider.value = randomTarget;
                targetValue = randomTarget;
                UpdateUI();
                Debug.Log($"设置随机目标值: {targetValue}");
            }
        }
        
        public void SetTargetValueToHalf()
        {
            if (targetValueSlider != null)
            {
                targetValueSlider.value = 0.5f;
                targetValue = 0.5f;
                UpdateUI();
                Debug.Log($"设置目标值: {targetValue}");
            }
        }
        
        private void SafeCompleteMotion(ref MotionHandle motion)
        {
            if (motion.IsActive())
            {
                try
                {
                    motion.Complete();
                }
                catch (System.ArgumentException)
                {
                    // 忽略无效的MotionHandle错误
                    Debug.LogWarning("尝试完成无效的MotionHandle");
                }
                motion = MotionHandle.None;
            }
        }
        
        private void SafeStopMotion(ref MotionHandle motion)
        {
            if (motion.IsActive())
            {
                try
                {
                    // 如果循环次数小于0（无限循环），使用Cancel()
                    if (loopCount < 0)
                    {
                        motion.Cancel();
                        Debug.Log("取消动画（无限循环）");
                    }
                    else
                    {
                        motion.Complete();
                        Debug.Log("完成动画");
                    }
                }
                catch (System.ArgumentException)
                {
                    // 忽略无效的MotionHandle错误
                    Debug.LogWarning("尝试停止无效的MotionHandle");
                }
                motion = MotionHandle.None;
            }
        }
        
        private void TestSpring1D()
        {
            if (spring1DSlider == null) return;
            
            // 停止之前的动画
            SafeCompleteMotion(ref spring1DMotion);
            
            // 获取当前值作为起始值
            float currentValue = spring1DSlider.value;
            
            // 创建Spring选项
            var springOptions = new SpringOptions
            {
                DampingRatio = dampingRatio,
                Stiffness = stiffness
            };
            
            // 创建1D Spring动画 - 从当前值到目标值
            var motionBuilder = LMotion.Spring.Create(currentValue, targetValue, springOptions);
            
            // 应用循环设置
            ApplyLoopSettings(motionBuilder);
            
            // 应用延迟设置
            ApplyDelaySettings(motionBuilder);
            
            // 添加回调
            motionBuilder.WithCancelOnError(true);
            motionBuilder.WithOnCancel(() => Debug.Log("1D Spring动画被取消"));
            motionBuilder.WithOnComplete(() => Debug.Log("1D Spring动画完成"));
            motionBuilder.WithOnLoopComplete(loopCount => Debug.Log($"1D Spring动画循环完成 - 循环次数: {loopCount}"));
            
            // 绑定到Slider
            spring1DMotion = motionBuilder.Bind(value => spring1DSlider.value = value);
            
            Debug.Log($"开始1D Spring测试 - 目标值: {targetValue}, 阻尼比: {dampingRatio}, 刚度: {stiffness}");
        }
        
        private void TestSpring2D()
        {
            if (spring2DSlider == null) return;
            
            // 停止之前的动画
            SafeCompleteMotion(ref spring2DMotion);
            
            // 获取当前值作为起始值
            float currentValue = spring2DSlider.value;
            
            // 创建Spring选项
            var springOptions = new SpringOptions
            {
                DampingRatio = dampingRatio,
                Stiffness = stiffness
            };
            
            // 创建2D Spring动画 - 从当前值到目标值
            var motionBuilder = LMotion.Spring.Create(new Vector2(currentValue, currentValue), new Vector2(targetValue, targetValue), springOptions);
            
            // 应用循环设置
            ApplyLoopSettings(motionBuilder);
            
            // 应用延迟设置
            ApplyDelaySettings(motionBuilder);
            
            // 添加回调
            motionBuilder.WithCancelOnError(true);
            motionBuilder.WithOnCancel(() => Debug.Log("2D Spring动画被取消"));
            motionBuilder.WithOnComplete(() => Debug.Log("2D Spring动画完成"));
            motionBuilder.WithOnLoopComplete(loopCount => Debug.Log($"2D Spring动画循环完成 - 循环次数: {loopCount}"));
            
            // 绑定到Slider（使用X分量）
            spring2DMotion = motionBuilder.Bind(value => spring2DSlider.value = value.x);
            
            Debug.Log($"开始2D Spring测试 - 目标值: ({targetValue}, {targetValue}), 阻尼比: {dampingRatio}, 刚度: {stiffness}");
        }
        
        private void TestSpring3D()
        {
            if (spring3DSlider == null) return;
            
            // 停止之前的动画
            SafeCompleteMotion(ref spring3DMotion);
            
            // 获取当前值作为起始值
            float currentValue = spring3DSlider.value;
            
            // 创建Spring选项
            var springOptions = new SpringOptions
            {
                DampingRatio = dampingRatio,
                Stiffness = stiffness
            };
            
            // 创建3D Spring动画 - 从当前值到目标值
            var motionBuilder = LMotion.Spring.Create(new Vector3(currentValue, currentValue, currentValue), new Vector3(targetValue, targetValue, targetValue), springOptions);
            
            // 应用循环设置
            ApplyLoopSettings(motionBuilder);
            
            // 应用延迟设置
            ApplyDelaySettings(motionBuilder);
            
            // 添加回调
            motionBuilder.WithCancelOnError(true);
            motionBuilder.WithOnCancel(() => Debug.Log("3D Spring动画被取消"));
            motionBuilder.WithOnComplete(() => Debug.Log("3D Spring动画完成"));
            motionBuilder.WithOnLoopComplete(loopCount => Debug.Log($"3D Spring动画循环完成 - 循环次数: {loopCount}"));
            
            // 绑定到Slider（使用X分量）
            spring3DMotion = motionBuilder.Bind(value => spring3DSlider.value = value.x);
            
            Debug.Log($"开始3D Spring测试 - 目标值: ({targetValue}, {targetValue}, {targetValue}), 阻尼比: {dampingRatio}, 刚度: {stiffness}");
        }
        
        private void TestSpring4D()
        {
            if (spring4DSlider == null) return;
            
            // 停止之前的动画
            SafeCompleteMotion(ref spring4DMotion);
            
            // 获取当前值作为起始值
            float currentValue = spring4DSlider.value;
            
            // 创建Spring选项
            var springOptions = new SpringOptions
            {
                DampingRatio = dampingRatio,
                Stiffness = stiffness
            };
            
            // 创建4D Spring动画 - 从当前值到目标值
            var motionBuilder = LMotion.Spring.Create(new Vector4(currentValue, currentValue, currentValue, currentValue), new Vector4(targetValue, targetValue, targetValue, targetValue), springOptions);
            
            // 应用循环设置
            ApplyLoopSettings(motionBuilder);
            
            // 应用延迟设置
            ApplyDelaySettings(motionBuilder);
            
            // 添加回调
            motionBuilder.WithCancelOnError(true);
            motionBuilder.WithOnCancel(() => Debug.Log("4D Spring动画被取消"));
            motionBuilder.WithOnComplete(() => Debug.Log("4D Spring动画完成"));
            motionBuilder.WithOnLoopComplete(loopCount => Debug.Log($"4D Spring动画循环完成 - 循环次数: {loopCount}"));
            
            // 绑定到Slider（使用X分量）
            spring4DMotion = motionBuilder.Bind(value => spring4DSlider.value = value.x);
            
            Debug.Log($"开始4D Spring测试 - 目标值: ({targetValue}, {targetValue}, {targetValue}, {targetValue}), 阻尼比: {dampingRatio}, 刚度: {stiffness}");
        }
        
        private void StartPerformanceTest()
        {
            if (performanceSlider == null) return;
            
            // 停止之前的动画
            SafeCompleteMotion(ref performanceMotion);
            ClearPerformanceMotions();
            
            // 重置比较测试状态
            isComparingTest = true;
            currentTestPhase = 0;
            springTestTime = 0f;
            tweenTestTime = 0f;
            
            // 开始Tween测试（调换顺序）
            StartSinglePerformanceTest(false);
        }
        
        private void StartSinglePerformanceTest(bool isSpring)
        {
            performanceTestCount = 0;
            performanceTestStartTime = UnityEngine.Time.time;
            isPerformanceTestRunning = true;
            isSpringTest = isSpring;
            
            // 创建大量动画来测试运行性能
            int animationCount = 1000000;
            
            if (isSpring)
            {
                // 测试Spring动画运行性能
                for (int i = 0; i < animationCount; i++)
                {
                    var motionBuilder = LMotion.Spring.Create(0f, targetValue, SpringOptions.Critical);
                    var motion = motionBuilder.Bind(value => { /* 空操作，仅测试运行性能 */ });
                    performanceMotions.Add(motion);
                    performanceTestCount++;
                }
                
                Debug.Log($"开始Spring运行性能测试 - 创建了 {performanceTestCount} 个Spring动画，将运行 {performanceTestDuration} 秒");
            }
            else
            {
                // 测试Tween动画运行性能
                for (int i = 0; i < animationCount; i++)
                {
                    var motionBuilder = LMotion.Create(0f, targetValue, performanceTestDuration)
                        .WithEase(Ease.OutQuad);
                    
                    var motion = motionBuilder.Bind(value => { /* 空操作，仅测试运行性能 */ });
                    performanceMotions.Add(motion);
                    performanceTestCount++;
                }
                
                Debug.Log($"开始Tween运行性能测试 - 创建了 {performanceTestCount} 个Tween动画，将运行 {performanceTestDuration} 秒");
            }
        }
        
        private void ApplyLoopSettings(MotionBuilder<float, SpringOptions, FloatSpringMotionAdapter> motionBuilder)
        {
            //if (loopCount > 0)
            {
                // 根据循环类型下拉选择器设置循环类型
                LitMotion.LoopType loopType = loopTypeIndex switch
                {
                    0 => LitMotion.LoopType.Restart,
                    1 => LitMotion.LoopType.Flip,
                    2 => LitMotion.LoopType.Incremental,
                    3 => LitMotion.LoopType.Yoyo,
                    _ => LitMotion.LoopType.Restart
                };
                
                motionBuilder.WithLoops((int)loopCount, loopType);
            }
        }
        
        private void ApplyLoopSettings(MotionBuilder<Vector2, SpringOptions, Vector2SpringMotionAdapter> motionBuilder)
        {
            //if (loopCount > 0)
            {
                // 根据循环类型下拉选择器设置循环类型
                LitMotion.LoopType loopType = loopTypeIndex switch
                {
                    0 => LitMotion.LoopType.Restart,
                    1 => LitMotion.LoopType.Flip,
                    2 => LitMotion.LoopType.Incremental,
                    3 => LitMotion.LoopType.Yoyo,
                    _ => LitMotion.LoopType.Restart
                };
                
                motionBuilder.WithLoops((int)loopCount, loopType);
            }
        }
        
        private void ApplyLoopSettings(MotionBuilder<Vector3, SpringOptions, Vector3SpringMotionAdapter> motionBuilder)
        {
            //if (loopCount > 0)
            {
                // 根据循环类型下拉选择器设置循环类型
                LitMotion.LoopType loopType = loopTypeIndex switch
                {
                    0 => LitMotion.LoopType.Restart,
                    1 => LitMotion.LoopType.Flip,
                    2 => LitMotion.LoopType.Incremental,
                    3 => LitMotion.LoopType.Yoyo,
                    _ => LitMotion.LoopType.Restart
                };
                
                motionBuilder.WithLoops((int)loopCount, loopType);
            }
        }
        
        private void ApplyLoopSettings(MotionBuilder<Vector4, SpringOptions, Vector4SpringMotionAdapter> motionBuilder)
        {
            //if (loopCount > 0)
            {
                // 根据循环类型下拉选择器设置循环类型
                LitMotion.LoopType loopType = loopTypeIndex switch
                {
                    0 => LitMotion.LoopType.Restart,
                    1 => LitMotion.LoopType.Flip,
                    2 => LitMotion.LoopType.Incremental,
                    3 => LitMotion.LoopType.Yoyo,
                    _ => LitMotion.LoopType.Restart
                };
                
                motionBuilder.WithLoops((int)loopCount, loopType);
            }
        }
        
        private void ApplyDelaySettings(MotionBuilder<float, SpringOptions, FloatSpringMotionAdapter> motionBuilder)
        {
            if (delay > 0f)
            {
                // 根据延迟类型下拉选择器设置延迟类型
                LitMotion.DelayType delayType = delayTypeIndex switch
                {
                    0 => LitMotion.DelayType.FirstLoop,
                    1 => LitMotion.DelayType.EveryLoop,
                    _ => LitMotion.DelayType.FirstLoop
                };
                
                motionBuilder.WithDelay(delay, delayType);
            }
        }
        
        private void ApplyDelaySettings(MotionBuilder<Vector2, SpringOptions, Vector2SpringMotionAdapter> motionBuilder)
        {
            if (delay > 0f)
            {
                // 根据延迟类型下拉选择器设置延迟类型
                LitMotion.DelayType delayType = delayTypeIndex switch
                {
                    0 => LitMotion.DelayType.FirstLoop,
                    1 => LitMotion.DelayType.EveryLoop,
                    _ => LitMotion.DelayType.FirstLoop
                };
                
                motionBuilder.WithDelay(delay, delayType);
            }
        }
        
        private void ApplyDelaySettings(MotionBuilder<Vector3, SpringOptions, Vector3SpringMotionAdapter> motionBuilder)
        {
            if (delay > 0f)
            {
                // 根据延迟类型下拉选择器设置延迟类型
                LitMotion.DelayType delayType = delayTypeIndex switch
                {
                    0 => LitMotion.DelayType.FirstLoop,
                    1 => LitMotion.DelayType.EveryLoop,
                    _ => LitMotion.DelayType.FirstLoop
                };
                
                motionBuilder.WithDelay(delay, delayType);
            }
        }
        
        private void ApplyDelaySettings(MotionBuilder<Vector4, SpringOptions, Vector4SpringMotionAdapter> motionBuilder)
        {
            if (delay > 0f)
            {
                // 根据延迟类型下拉选择器设置延迟类型
                LitMotion.DelayType delayType = delayTypeIndex switch
                {
                    0 => LitMotion.DelayType.FirstLoop,
                    1 => LitMotion.DelayType.EveryLoop,
                    _ => LitMotion.DelayType.FirstLoop
                };
                
                motionBuilder.WithDelay(delay, delayType);
            }
        }
        
        // UI事件处理
        private void OnTimeScaleChanged(float value)
        {
            timeScale = value;
            UpdateUI();
        }
        
        private void OnDampingRatioChanged(float value)
        {
            dampingRatio = value;
            UpdateUI();
        }
        
        private void OnStiffnessChanged(float value)
        {
            stiffness = value;
            UpdateUI();
        }
        
        private void OnTargetValueChanged(float value)
        {
            targetValue = value;
            UpdateUI();
            
            // 如果动画正在播放，动态更新目标值
            UpdateActiveAnimationsTargetValue(value);
        }
        
        private void UpdateActiveAnimationsTargetValue(float newTargetValue)
        {
            // 更新1D Spring动画的目标值
            if (spring1DMotion.IsActive())
            {
                try
                {
                    ref float endValue = ref spring1DMotion.GetEndValueRef<float, SpringOptions>();
                    endValue = newTargetValue;
                    Debug.Log($"动态更新1D Spring目标值: {newTargetValue}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"更新1D Spring目标值失败: {ex.Message}");
                }
            }
            
            // 更新2D Spring动画的目标值
            if (spring2DMotion.IsActive())
            {
                try
                {
                    ref Vector2 endValue = ref spring2DMotion.GetEndValueRef<Vector2, SpringOptions>();
                    endValue = new Vector2(newTargetValue, newTargetValue);
                    Debug.Log($"动态更新2D Spring目标值: {newTargetValue}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"更新2D Spring目标值失败: {ex.Message}");
                }
            }
            
            // 更新3D Spring动画的目标值
            if (spring3DMotion.IsActive())
            {
                try
                {
                    ref Vector3 endValue = ref spring3DMotion.GetEndValueRef<Vector3, SpringOptions>();
                    endValue = new Vector3(newTargetValue, newTargetValue, newTargetValue);
                    Debug.Log($"动态更新3D Spring目标值: {newTargetValue}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"更新3D Spring目标值失败: {ex.Message}");
                }
            }
            
            // 更新4D Spring动画的目标值
            if (spring4DMotion.IsActive())
            {
                try
                {
                    ref Vector4 endValue = ref spring4DMotion.GetEndValueRef<Vector4, SpringOptions>();
                    endValue = new Vector4(newTargetValue, newTargetValue, newTargetValue, newTargetValue);
                    Debug.Log($"动态更新4D Spring目标值: {newTargetValue}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"更新4D Spring目标值失败: {ex.Message}");
                }
            }
        }
        
        private void OnDelayChanged(float value)
        {
            delay = value;
            UpdateUI();
        }
        
        private void OnLoopCountChanged(float value)
        {
            loopCount = value;
            UpdateUI();
        }
        
        private void OnLoopTypeChanged(int value)
        {
            loopTypeIndex = value;
            UpdateUI();
        }
        
        private void OnDelayTypeChanged(int value)
        {
            delayTypeIndex = value;
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            if (timeScaleText != null)
                timeScaleText.text = $"时间缩放: {timeScale:F2}x";
            
            if (dampingRatioText != null)
                dampingRatioText.text = $"阻尼比: {dampingRatio:F2}";
            
            if (stiffnessText != null)
                stiffnessText.text = $"刚度: {stiffness:F2}";
            
            if (targetValueText != null)
                targetValueText.text = $"目标值: {targetValue:F3}";
            
            if (delayText != null)
                delayText.text = $"延迟: {delay:F1}s";
            
            if (loopCountText != null)
                loopCountText.text = $"循环次数: {loopCount:F0}";
        }
        
        // 自动分配UI引用
        private void AutoAssignUIReferences()
        {
            // 自动查找并分配UI引用
            var fields = typeof(LitMotionSpringTestUI).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(Slider))
                {
                    Slider foundSlider = FindSliderByName(field.Name);
                    if (foundSlider != null)
                    {
                        field.SetValue(this, foundSlider);
                        Debug.Log($"自动分配Slider: {field.Name} -> {foundSlider.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"未找到Slider: {field.Name}");
                    }
                }
                else if (field.FieldType == typeof(TextMeshProUGUI))
                {
                    TextMeshProUGUI foundText = FindTextByName(field.Name);
                    if (foundText != null)
                    {
                        field.SetValue(this, foundText);
                        Debug.Log($"自动分配Text: {field.Name} -> {foundText.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"未找到Text: {field.Name}");
                    }
                }
                else if (field.FieldType == typeof(Dropdown))
                {
                    Dropdown foundDropdown = FindDropdownByName(field.Name);
                    if (foundDropdown != null)
                    {
                        field.SetValue(this, foundDropdown);
                        Debug.Log($"自动分配Dropdown: {field.Name} -> {foundDropdown.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"未找到Dropdown: {field.Name}");
                    }
                }
                else if (field.FieldType == typeof(Button))
                {
                    Button foundButton = FindButtonByName(field.Name);
                    if (foundButton != null)
                    {
                        field.SetValue(this, foundButton);
                        Debug.Log($"自动分配Button: {field.Name} -> {foundButton.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"未找到Button: {field.Name}");
                    }
                }
                else if (field.FieldType == typeof(Text) && field.Name.Contains("performanceResult"))
                {
                    Text foundText = FindUnityTextByName(field.Name);
                    if (foundText != null)
                    {
                        field.SetValue(this, foundText);
                        Debug.Log($"自动分配PerformanceResultText: {field.Name} -> {foundText.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"未找到PerformanceResultText: {field.Name}");
                    }
                }
            }
            
            // 在UI引用分配完成后设置按钮事件
            SetupButtonEvents();
        }
        
        private Slider FindSliderByName(string fieldName)
        {
            Slider[] allSliders = FindObjectsByType<Slider>(FindObjectsSortMode.None);
            string searchName = fieldName.ToLower().Replace("slider", "");
            
            foreach (var slider in allSliders)
            {
                string sliderName = slider.name.ToLower();
                if (sliderName.Replace("slider", "") == searchName)
                {
                    return slider;
                }
            }
            return null;
        }
        
        private TextMeshProUGUI FindTextByName(string fieldName)
        {
            TextMeshProUGUI[] allTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
            string searchName = fieldName.ToLower().Replace("text", "");
            
            foreach (var text in allTexts)
            {
                string textName = text.name.ToLower();
                if (textName.Replace("label", "").Replace("value", "").Replace("text", "") == searchName)
                {
                    return text;
                }
            }
            return null;
        }
        
        private Dropdown FindDropdownByName(string fieldName)
        {
            Dropdown[] allDropdowns = FindObjectsByType<Dropdown>(FindObjectsSortMode.None);
            string searchName = fieldName.ToLower().Replace("dropdown", "");
            
            foreach (var dropdown in allDropdowns)
            {
                string dropdownName = dropdown.name.ToLower();
                if (dropdownName.Replace("dropdown", "") == searchName)
                {
                    return dropdown;
                }
            }
            return null;
        }
        
        private Button FindButtonByName(string fieldName)
        {
            Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
            string searchName = fieldName.ToLower().Replace("button", "");
            
            foreach (var button in allButtons)
            {
                string buttonName = button.name.ToLower();
                if (buttonName.Replace("button", "") == searchName)
                {
                    return button;
                }
            }
            return null;
        }
        
        private Text FindUnityTextByName(string fieldName)
        {
            Text[] allTexts = FindObjectsByType<Text>(FindObjectsSortMode.None);
            string searchName = fieldName.ToLower().Replace("text", "");
            
            foreach (var text in allTexts)
            {
                string textName = text.name.ToLower();
                if (textName.Replace("label", "").Replace("value", "").Replace("text", "") == searchName)
                {
                    return text;
                }
            }
            return null;
        }
        
        // 检查UI引用分配状态
        [ContextMenu("检查UI引用")]
        public void CheckUIReferences()
        {
            Debug.Log("=== UI引用检查 ===");
            Debug.Log($"timeScaleSlider: {(timeScaleSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"dampingRatioSlider: {(dampingRatioSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"stiffnessSlider: {(stiffnessSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"targetValueSlider: {(targetValueSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"delaySlider: {(delaySlider != null ? "已分配" : "未分配")}");
            Debug.Log($"loopTypeDropdown: {(loopTypeDropdown != null ? "已分配" : "未分配")}");
            Debug.Log($"delayTypeDropdown: {(delayTypeDropdown != null ? "已分配" : "未分配")}");
            Debug.Log($"spring1DSlider: {(spring1DSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"spring2DSlider: {(spring2DSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"spring3DSlider: {(spring3DSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"spring4DSlider: {(spring4DSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"performanceSlider: {(performanceSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"setRandomTargetButton: {(setRandomTargetButton != null ? "已分配" : "未分配")}");
            Debug.Log($"setHalfTargetButton: {(setHalfTargetButton != null ? "已分配" : "未分配")}");
            Debug.Log($"test1DButton: {(test1DButton != null ? "已分配" : "未分配")}");
            Debug.Log($"stop1DButton: {(stop1DButton != null ? "已分配" : "未分配")}");
            Debug.Log($"test2DButton: {(test2DButton != null ? "已分配" : "未分配")}");
            Debug.Log($"stop2DButton: {(stop2DButton != null ? "已分配" : "未分配")}");
            Debug.Log($"test3DButton: {(test3DButton != null ? "已分配" : "未分配")}");
            Debug.Log($"stop3DButton: {(stop3DButton != null ? "已分配" : "未分配")}");
            Debug.Log($"test4DButton: {(test4DButton != null ? "已分配" : "未分配")}");
            Debug.Log($"stop4DButton: {(stop4DButton != null ? "已分配" : "未分配")}");
            Debug.Log($"testPerformanceButton: {(testPerformanceButton != null ? "已分配" : "未分配")}");
            Debug.Log($"stopPerformanceButton: {(stopPerformanceButton != null ? "已分配" : "未分配")}");
            Debug.Log($"testAllCombinationsButton: {(testAllCombinationsButton != null ? "已分配" : "未分配")}");
            Debug.Log($"performanceResultText: {(performanceResultText != null ? "已分配" : "未分配")}");
            Debug.Log("==================");
        }
        
        private void TestAllCombinations()
        {
            Debug.Log(GetLocalizedString("STARTING_ALL_DIMENSIONS"));
            
            // 停止所有当前动画
            StopAllAnimations();
            
            // 同时启动所有维数的Spring动画
            TestSpring1D();
            TestSpring2D();
            TestSpring3D();
            TestSpring4D();
            
            Debug.Log(GetLocalizedString("ALL_DIMENSIONS_STARTED"));
        }
        
        private System.Collections.IEnumerator WaitForMotionComplete(MotionHandle motion, int combinationNumber)
        {
            // 等待动画完成
            while (motion.IsActive())
            {
                yield return null;
            }
            
            Debug.Log($"组合 {combinationNumber} 测试完成");
            
            // 短暂等待后开始下一个组合
            yield return new UnityEngine.WaitForSeconds(0.5f);
        }
        
        // ==================== 多语言支持 ====================
        
        [Header("多语言支持")]
        [SerializeField] public bool useMultiLanguage = true;
        
        // 多语言字典
        private Dictionary<string, string[]> localizedStrings;
        private int currentLanguageIndex = 0; // 0=英文, 1=中文, 2=日文
        
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
        
        /// <summary>
        /// 初始化多语言字符串
        /// </summary>
        private void InitializeLocalizedStrings()
        {
            localizedStrings = new Dictionary<string, string[]>();
            
            // Spring测试相关字符串 [英文, 中文, 日文]
            localizedStrings["LITMOTION_SPRING_TEST"] = new string[] {
                "LitMotion.Spring Visual Test UI",
                "LitMotion.Spring可视化测试UI",
                "LitMotion.SpringビジュアルテストUI"
            };
            
            localizedStrings["TEST_SPRING_ANIMATIONS"] = new string[] {
                "Test various Spring animation configurations and parameter effects",
                "用于测试各种Spring动画配置的行为和参数效果",
                "様々なSpringアニメーション設定の動作とパラメータ効果をテスト"
            };
            
            localizedStrings["TIME_SCALE"] = new string[] {
                "Time Scale: {0:F2}x",
                "时间缩放: {0:F2}x",
                "時間スケール: {0:F2}x"
            };
            
            localizedStrings["DAMPING_RATIO"] = new string[] {
                "Damping Ratio: {0:F2}",
                "阻尼比: {0:F2}",
                "減衰比: {0:F2}"
            };
            
            localizedStrings["STIFFNESS"] = new string[] {
                "Stiffness: {0:F2}",
                "刚度: {0:F2}",
                "剛性: {0:F2}"
            };
            
            localizedStrings["TARGET_VALUE"] = new string[] {
                "Target Value: {0:F3}",
                "目标值: {0:F3}",
                "目標値: {0:F3}"
            };
            
            localizedStrings["DELAY"] = new string[] {
                "Delay: {0:F1}s",
                "延迟: {0:F1}s",
                "遅延: {0:F1}s"
            };
            
            localizedStrings["LOOP_COUNT"] = new string[] {
                "Loop Count: {0:F0}",
                "循环次数: {0:F0}",
                "ループ回数: {0:F0}"
            };
            
            localizedStrings["PERFORMANCE_TEST_RUNNING"] = new string[] {
                "Performance test in progress...",
                "性能测试进行中...",
                "パフォーマンステスト実行中..."
            };
            
            localizedStrings["ANIMATION_TYPE"] = new string[] {
                "Animation Type: {0}",
                "测试类型: {0}",
                "アニメーションタイプ: {0}"
            };
            
            localizedStrings["ANIMATION_COUNT"] = new string[] {
                "Animation Count: {0:N0}",
                "动画数量: {0:N0}",
                "アニメーション数: {0:N0}"
            };
            
            localizedStrings["PROGRESS"] = new string[] {
                "Progress: {0:P1}",
                "进度: {0:P1}",
                "進捗: {0:P1}"
            };
            
            localizedStrings["CURRENT_FRAME_RATE"] = new string[] {
                "Current Frame Rate: {0:F1} FPS",
                "当前帧率: {0:F1} FPS",
                "現在のフレームレート: {0:F1} FPS"
            };
            
            localizedStrings["SPRING_ANIMATION"] = new string[] {
                "Spring Animation",
                "Spring动画",
                "Springアニメーション"
            };
            
            localizedStrings["TWEEN_ANIMATION"] = new string[] {
                "Tween Animation",
                "Tween动画",
                "Tweenアニメーション"
            };
            
            localizedStrings["PERFORMANCE_COMPARISON_RESULT"] = new string[] {
                "Performance Comparison Test Results",
                "性能比较测试结果",
                "パフォーマンス比較テスト結果"
            };
            
            localizedStrings["SPRING_FRAME_RATE"] = new string[] {
                "Spring Animation Frame Rate: {0:F1} FPS",
                "Spring动画帧率: {0:F1} FPS",
                "Springアニメーションフレームレート: {0:F1} FPS"
            };
            
            localizedStrings["TWEEN_FRAME_RATE"] = new string[] {
                "Tween Animation Frame Rate: {0:F1} FPS",
                "Tween动画帧率: {0:F1} FPS",
                "Tweenアニメーションフレームレート: {0:F1} FPS"
            };
            
            localizedStrings["BETTER_PERFORMANCE"] = new string[] {
                "{0} animation performs better",
                "{0}动画性能更好",
                "{0}アニメーションのパフォーマンスが良い"
            };
            
            localizedStrings["PERFORMANCE_DIFFERENCE"] = new string[] {
                "Performance Difference: {0:F2}x",
                "性能差异: {0:F2} 倍",
                "パフォーマンス差異: {0:F2}倍"
            };
            
            localizedStrings["CLICK_PERFORMANCE_TEST"] = new string[] {
                "Click 'Performance Test' to restart comparison",
                "点击'性能测试'重新开始比较",
                "'パフォーマンステスト'をクリックして比較を再開"
            };
            
            localizedStrings["AUTO_TEST_TWEEN_SPRING"] = new string[] {
                "Will automatically test Tween and Spring animations",
                "将自动测试Tween和Spring动画",
                "TweenとSpringアニメーションを自動テストします"
            };
            
            localizedStrings["TEST_ANIMATION_COUNT"] = new string[] {
                "Test Animation Count: 1,000,000",
                "测试动画数量: 1,000,000",
                "テストアニメーション数: 1,000,000"
            };
            
            localizedStrings["EACH_PHASE_DURATION"] = new string[] {
                "Each Phase Duration: {0} seconds",
                "每个阶段持续时间: {0}秒",
                "各フェーズの持続時間: {0}秒"
            };
            
            localizedStrings["TEST_ALL_DIMENSIONS"] = new string[] {
                "Test All Dimensions",
                "测试所有维数",
                "全次元テスト"
            };
            
            localizedStrings["STARTING_ALL_DIMENSIONS"] = new string[] {
                "Starting all dimension Spring animations...",
                "开始所有维数的Spring动画...",
                "全次元Springアニメーション開始中..."
            };
            
            localizedStrings["ALL_DIMENSIONS_STARTED"] = new string[] {
                "All dimension Spring animations started",
                "所有维数的Spring动画已启动",
                "全次元Springアニメーション開始完了"
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
    }
}
