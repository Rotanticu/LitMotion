using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitMotion;
using System.Collections.Generic;
using Unity.Mathematics;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ASUI.Tests
{
    /// <summary>
    /// DamperUtility可视化测试UI
    /// 用于测试各种阻尼器函数的行为和参数效果
    /// </summary>
    public class DamperUtilityTestUI : MonoBehaviour
    {
        [Header("UI Controls")]
        [SerializeField] private Slider timeScaleSlider;
        [SerializeField] private TextMeshProUGUI timeScaleText;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private TextMeshProUGUI masterText;
        
        [Header("Parameter Controls")]
        [SerializeField] private Slider dampingRatioSlider;
        [SerializeField] private TextMeshProUGUI dampingRatioText;
        [SerializeField] private Slider stiffnessSlider;
        [SerializeField] private TextMeshProUGUI stiffnessText;
        
        [Header("SpringElastic Tests")]
        [SerializeField] private Slider springElasticOverdampedSlider;
        [SerializeField] private Slider springElasticCriticalSlider;
        [SerializeField] private Slider springElasticUnderdampedSlider;
        
        [Header("SpringPrecise Tests")]
        [SerializeField] private Slider springPreciseOverdampedSlider;
        [SerializeField] private Slider springPreciseCriticalSlider;
        [SerializeField] private Slider springPreciseUnderdampedSlider;
        
        [Header("SpringSimple Test")]
        [SerializeField] private Slider springSimpleSlider;
        
        [Header("Variant Tests")]
        [SerializeField] private Slider velocitySmoothingSlider;
        [SerializeField] private Slider durationLimitSlider;
        [SerializeField] private Slider doubleSmoothingSlider;
        
        [Header("Variant Parameters")]
        [SerializeField] private Slider targetVelocitySlider;
        [SerializeField] private TextMeshProUGUI targetVelocityText;
        [SerializeField] private Slider durationSlider;
        [SerializeField] private TextMeshProUGUI durationText;
        [SerializeField] private Slider smothingVelocitySlider;
        [SerializeField] private TextMeshProUGUI smothingVelocityText;
        
        [Header("Mouse Follow Test")]
        [SerializeField] private RectTransform followTarget;
        [SerializeField] private TextMeshProUGUI followTargetText;
        
        // 阻尼器状态
        private class DamperState
        {
            public float currentValue;
            public float currentVelocity;
            public float intermediatePosition;
            public float intermediateVelocity;
            
            public DamperState(float initialValue = 0f)
            {
                currentValue = initialValue;
                currentVelocity = 0f;
                intermediatePosition = 0f;
                intermediateVelocity = 0f;
            }
        }
        
        // 各种阻尼器状态
        private DamperState springElasticOverdampedState;
        private DamperState springElasticCriticalState;
        private DamperState springElasticUnderdampedState;
        private DamperState springPreciseOverdampedState;
        private DamperState springPreciseCriticalState;
        private DamperState springPreciseUnderdampedState;
        private DamperState springSimpleState;
        private DamperState velocitySmoothingState;
        private DamperState durationLimitState;
        private DamperState doubleSmoothingState;
        
        // 参数值
        private float dampingRatio = 0.5f;
        private float stiffness = 10.0f;
        private float targetVelocity = 0.0f;
        private float duration = 0.02f;
        private float smothingVelocity = 2.0f;
        
        // 鼠标跟随测试
        private MotionHandle followMotion;
        private bool isMouseFollowActive = false;
        private Vector2 lastMousePosition;
        private Vector2 mouseVelocity;
        
        private float timeScale
        {
            get
            {
                return UnityEngine.Time.timeScale;
            }
            set
            {
                UnityEngine.Time.timeScale = value;
            }
        }
        
        private void Start()
        {
            DetectSystemLanguage();
            InitializeLocalizedStrings();
            InitializeStates();
            SetupUI();

            // 尝试自动分配UI引用
            AutoAssignUIReferences();     
        }

        private void InitializeStates()
        {
            springElasticOverdampedState = new DamperState();
            springElasticCriticalState = new DamperState();
            springElasticUnderdampedState = new DamperState();
            springPreciseOverdampedState = new DamperState();
            springPreciseCriticalState = new DamperState();
            springPreciseUnderdampedState = new DamperState();
            springSimpleState = new DamperState();
            velocitySmoothingState = new DamperState();
            durationLimitState = new DamperState();
            doubleSmoothingState = new DamperState();
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
            
            // 设置主控slider
            if (masterSlider != null)
            {
                masterSlider.minValue = 0f;
                masterSlider.maxValue = 1f;
                masterSlider.value = 0f;
                masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
            }
            
            // 设置参数控制
            if (dampingRatioSlider != null)
            {
                dampingRatioSlider.minValue = 0.01f;
                dampingRatioSlider.maxValue = 2.0f;
                dampingRatioSlider.value = 0.5f;
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

            // 设置变种参数控制
            if (targetVelocitySlider != null)
            {
                targetVelocitySlider.minValue = -2.0f;
                targetVelocitySlider.maxValue = 2.0f;
                targetVelocitySlider.value = 0.0f;
                targetVelocitySlider.onValueChanged.AddListener(OnTargetVelocityChanged);
                targetVelocity = targetVelocitySlider.value;
            }

            if (durationSlider != null)
            {
                durationSlider.minValue = 0.02f;
                durationSlider.maxValue = 2f;
                durationSlider.value = 0.2f;
                durationSlider.onValueChanged.AddListener(OnDurationChanged);
                duration = durationSlider.value;
                smothingVelocity = smothingVelocitySlider.value;
            }
            
            if (smothingVelocitySlider != null)
            {
                smothingVelocitySlider.minValue = 0.5f;
                smothingVelocitySlider.maxValue = 5.0f;
                smothingVelocitySlider.value = 2.0f;
                smothingVelocitySlider.onValueChanged.AddListener(OnSmothingVelocityChanged);
                smothingVelocity = smothingVelocitySlider.value;
            }
            
            UpdateUI();
        }
        
        private void Update()
        {
            long currentTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            float deltaTime = UnityEngine.Time.deltaTime;
            
            if (deltaTime > 0)
            {
                UpdateDampers(deltaTime);
            }
            
            // 处理鼠标跟随测试
            UpdateMouseFollowTest();
        }
        
        private void UpdateDampers(float deltaTime)
        {
            float targetValue = masterSlider != null ? masterSlider.value : 0f;
            
            // 调试信息：检查UI引用是否正确分配
            if (masterSlider == null)
            {
                Debug.LogWarning("masterSlider 未分配！");
                return;
            }
            
            // SpringElastic 测试 - 三种阻尼状态
            // 修复判断逻辑后，现在与SpringPrecise使用相同的参数值
            UpdateSpringElastic(springElasticOverdampedState, springElasticOverdampedSlider, targetValue, deltaTime, dampingRatio + 1f); // 过阻尼 - 更慢的收敛
            UpdateSpringPrecise(springPreciseOverdampedState, springPreciseOverdampedSlider, targetValue, deltaTime, dampingRatio + 1f); // 过阻尼 - 更慢的收敛

            UpdateSpringElastic(springElasticCriticalState, springElasticCriticalSlider, targetValue, deltaTime, 1.0f); // 临界阻尼
            UpdateSpringPrecise(springPreciseCriticalState, springPreciseCriticalSlider, targetValue, deltaTime, 1.0f); // 临界阻尼

            UpdateSpringElastic(springElasticUnderdampedState, springElasticUnderdampedSlider, targetValue, deltaTime, dampingRatio); // 欠阻尼 - 更明显的振荡
            UpdateSpringPrecise(springPreciseUnderdampedState, springPreciseUnderdampedSlider, targetValue, deltaTime, dampingRatio); // 欠阻尼 - 更明显的振荡
            // SpringSimple 测试
            UpdateSpringSimple(springSimpleState, springSimpleSlider, targetValue, deltaTime);
            
            // 变种测试
            UpdateVelocitySmoothing(velocitySmoothingState, velocitySmoothingSlider, targetValue, deltaTime);
            UpdateDurationLimit(durationLimitState, durationLimitSlider, targetValue, deltaTime);
            UpdateDoubleSmoothing(doubleSmoothingState, doubleSmoothingSlider, targetValue, deltaTime);
        }
        
        private void UpdateSpringElastic(DamperState state, Slider slider, float targetValue, float deltaTime, float dampingRatioOverride)
        {
            if (slider == null) return;
            
            SpringUtility.SpringElastic(
                deltaTime,
                ref state.currentValue,
                ref state.currentVelocity,
                targetValue,
                targetVelocity,
                dampingRatioOverride,
                stiffness
            );
            slider.value = state.currentValue;
        }
        
        private void UpdateSpringPrecise(DamperState state, Slider slider, float targetValue, float deltaTime, float dampingRatioOverride)
        {
            if (slider == null) return;
            
            SpringUtility.SpringPrecise(
                deltaTime,
                ref state.currentValue,
                ref state.currentVelocity,
                targetValue,
                targetVelocity,
                dampingRatioOverride,
                stiffness
            );
            
            slider.value = state.currentValue;
        }
        float startTime = 0;
        private void UpdateSpringSimple(DamperState state, Slider slider, float targetValue, float deltaTime)
        {
            if (slider == null) return;

            SpringUtility.SpringSimple(
                    deltaTime,
                    ref state.currentValue,
                    ref state.currentVelocity,
                    targetValue,
                    stiffness
                );

            slider.value = state.currentValue;
        }
        
        private void UpdateVelocitySmoothing(DamperState state, Slider slider, float targetValue, float deltaTime)
        {
            if (slider == null) return;
            
            SpringUtility.SpringSimpleVelocitySmoothing(
                deltaTime,
                ref state.currentValue,
                ref state.currentVelocity,
                targetValue,
                ref state.intermediatePosition,
                smothingVelocity,
                stiffness
            );
            
            slider.value = state.currentValue;
        }
        bool isSpringSimple = false;
        public static bool Approximately(float a, float b,float precision)
        {
            return System.Math.Abs(b - a) < System.Math.Max(1E-06f * System.Math.Max(System.Math.Abs(a), System.Math.Abs(b)), precision);
        }
        private void UpdateDurationLimit(DamperState state, Slider slider, float targetValue, float deltaTime)
        {
            if (slider == null) return;
            if (!Approximately((float)state.currentValue, targetValue, 0.05f))
            {
                if (!isSpringSimple)
                {
                    startTime = UnityEngine.Time.time;
                    isSpringSimple = true;
                    Debug.Log($"Start Duration Limit {startTime}");
                }
            }
            else
            {
                if (isSpringSimple)
                {
                    Debug.Log($"Duration: {duration} Time: {UnityEngine.Time.time - startTime}");
                    isSpringSimple = false;
                }
            }
            if (isSpringSimple && (UnityEngine.Time.time - startTime) > duration)
            {
                Debug.Log($"detla = {System.Math.Abs(state.currentValue - targetValue)}");
            }
            SpringUtility.SpringSimpleDurationLimit(
                deltaTime,
                ref state.currentValue,
                ref state.currentVelocity,
                targetValue,
                duration
            );

            slider.value = state.currentValue;
        }
        
        private void UpdateDoubleSmoothing(DamperState state, Slider slider, float targetValue, float deltaTime)
        {
            if (slider == null) return;
            
                SpringUtility.SpringSimpleDoubleSmoothing(
                deltaTime,
                ref state.currentValue,
                ref state.currentVelocity,
                targetValue,
                ref state.intermediatePosition,
                ref state.intermediateVelocity,
                stiffness
            );
            
            slider.value = state.currentValue;
        }
        // 鼠标跟随测试
        private void UpdateMouseFollowTest()
        {
            Vector2 currentMousePosition;
            bool rightButtonPressed;
            
#if ENABLE_INPUT_SYSTEM
            // 使用新输入系统获取鼠标位置（性能更好）
            currentMousePosition = Mouse.current?.position.ReadValue() ?? Vector2.zero;
            rightButtonPressed = Mouse.current?.rightButton.isPressed == true;
#else
            // 使用传统输入系统
            currentMousePosition = Input.mousePosition;
            rightButtonPressed = Input.GetMouseButton(1); // 右键
#endif
            
            // 计算鼠标速度
            if (lastMousePosition != Vector2.zero)
            {
                mouseVelocity = (currentMousePosition - lastMousePosition) / Time.deltaTime;
            }
            lastMousePosition = currentMousePosition;

            // 检测鼠标右键按下
            if (rightButtonPressed)
            {
                // 获取鼠标在屏幕上的位置
                Vector2 mouseScreenPos = currentMousePosition;

                if (followTarget != null)
                {
                    if (followMotion == MotionHandle.None)
                    {
                        // 创建自定义的Spring选项，包含鼠标速度
                        LitMotion.SpringOptions springOptions = LitMotion.SpringOptions.Overdamped;
                        // 创建2D Spring动画，让followTarget跟随鼠标位置
                        followMotion = LitMotion.LMotion.Spring.Create(
                            (Vector2)followTarget.position,  // 起始位置
                            mouseScreenPos,                  // 目标位置（鼠标位置）
                            springOptions                   // 使用自定义的Spring选项，包含鼠标速度
                        ).WithLoops(-1)
                        .Bind(pos => followTarget.position = pos);

                        isMouseFollowActive = true;
                        UpdateFollowTargetText(GetLocalizedString("FOLLOWING"));
                    }
                    else
                    {
                        // 更新目标位置
                        followMotion.SetEndValue<Vector2, SpringOptions>(mouseScreenPos);
                        ref var springOptions = ref followMotion.GetOptions<Vector2, SpringOptions>(false);
                        springOptions.TargetVelocity = new float4(mouseVelocity.x, mouseVelocity.y, 0, 0);
                    }
                }
            }
        }
        
        private void UpdateFollowTargetText(string text)
        {
            if (followTargetText != null)
            {
                followTargetText.text = text;
            }
        }
        
        // UI事件处理
        private void OnTimeScaleChanged(float value)
        {
            timeScale = value;
            UpdateUI();
        }
        
        private void OnMasterSliderChanged(float value)
        {
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
        
        private void OnTargetVelocityChanged(float value)
        {
            targetVelocity = value;
            UpdateUI();
        }
        
        private void OnDurationChanged(float value)
        {
            duration = value;
            UpdateUI();
        }
        
        private void OnSmothingVelocityChanged(float value)
        {
            smothingVelocity = value;
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            if (timeScaleText != null)
                timeScaleText.text = GetLocalizedString("TIME_SCALE", timeScale);
            
            if (masterText != null)
                masterText.text = GetLocalizedString("MASTER_VALUE", masterSlider?.value ?? 0f);
            
            if (dampingRatioText != null)
                dampingRatioText.text = GetLocalizedString("DAMPING_RATIO", dampingRatio);
            
            if (stiffnessText != null)
                stiffnessText.text = GetLocalizedString("STIFFNESS", stiffness);
            
            if (targetVelocityText != null)
                targetVelocityText.text = GetLocalizedString("TARGET_VELOCITY", targetVelocity);
            
            if (durationText != null)
                durationText.text = GetLocalizedString("DURATION", duration);
            
            if (smothingVelocityText != null)
                smothingVelocityText.text = GetLocalizedString("SMOOTHING_VELOCITY", smothingVelocity);
            
            if (followTargetText != null && !isMouseFollowActive)
                followTargetText.text = GetLocalizedString("RIGHT_CLICK_FOLLOW_MOUSE");
        }
        
        // 自动分配UI引用
        private void AutoAssignUIReferences()
        {
            // 自动查找并分配UI引用
            var fields = typeof(DamperUtilityTestUI).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
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
                else if (field.FieldType == typeof(RectTransform))
                {
                    RectTransform foundRect = FindRectTransformByName(field.Name);
                    if (foundRect != null)
                    {
                        field.SetValue(this, foundRect);
                        Debug.Log($"自动分配RectTransform: {field.Name} -> {foundRect.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"未找到RectTransform: {field.Name}");
                    }
                }
            }
        }
        
        private Slider FindSliderByName(string fieldName)
        {
            Slider[] allSliders = FindObjectsByType<Slider>(FindObjectsSortMode.None);
            string searchName = fieldName.ToLower().Replace("slider", "");
            
            foreach (var slider in allSliders)
            {
                string sliderName = slider.name.ToLower();
                // 精确匹配：slider名称包含字段名
                if (sliderName.Contains(searchName) && sliderName.Contains("slider"))
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
                // 精确匹配：text名称包含字段名
                if (textName.Replace("label", "").Replace("value", "").Replace("text", "") == searchName)
                {
                    return text;
                }
            }
            return null;
        }
        
        private RectTransform FindRectTransformByName(string fieldName)
        {
            RectTransform[] allRects = FindObjectsByType<RectTransform>(FindObjectsSortMode.None);
            foreach (var rect in allRects)
            {
                // 精确匹配：rect名称包含字段名
                if (rect.name.ToLower() == fieldName.ToLower())
                {
                    return rect;
                }
            }
            return null;
        }
        
        // 检查UI引用分配状态
        [ContextMenu("Check UI References")]
        public void CheckUIReferences()
        {
            Debug.Log("=== UI引用检查 ===");
            Debug.Log($"timeScaleSlider: {(timeScaleSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"masterSlider: {(masterSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"dampingRatioSlider: {(dampingRatioSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"stiffnessSlider: {(stiffnessSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"springElasticOverdampedSlider: {(springElasticOverdampedSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"springElasticCriticalSlider: {(springElasticCriticalSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"springElasticUnderdampedSlider: {(springElasticUnderdampedSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"springPreciseOverdampedSlider: {(springPreciseOverdampedSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"springPreciseCriticalSlider: {(springPreciseCriticalSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"springPreciseUnderdampedSlider: {(springPreciseUnderdampedSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"springSimpleSlider: {(springSimpleSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"velocitySmoothingSlider: {(velocitySmoothingSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"durationLimitSlider: {(durationLimitSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"doubleSmoothingSlider: {(doubleSmoothingSlider != null ? "已分配" : "未分配")}");
            Debug.Log($"followTarget: {(followTarget != null ? "已分配" : "未分配")}");
            Debug.Log($"followTargetText: {(followTargetText != null ? "已分配" : "未分配")}");
            Debug.Log("==================");
        }
        
        // 测试阻尼器参数计算
        [ContextMenu("Test Damper Calculations")]
        public void TestDamperCalculations()
        {
            Debug.Log("=== 阻尼器参数计算测试 ===");
            
            float[] testRatios = { 0.5f, 1.0f, 1.5f, 2.0f };
            float naturalFreq = 1.0f;
            
            foreach (float dampingRatio in testRatios)
            {
                Debug.Log($"\n阻尼比: {dampingRatio}");
                
                // SpringElastic计算
                double halfLife = DampingToHalfLife(2.0 * dampingRatio * naturalFreq);
                double dampingCoeff = HalfLifeToDamping(halfLife);
                double stiffnessValue = DampingRatioToStiffness(dampingRatio, dampingCoeff);
                double discriminant = stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0f;
                
                Debug.Log($"SpringElastic - halfLife: {halfLife:F3}, dampingCoeff: {dampingCoeff:F3}, stiffnessValue: {stiffnessValue:F3}");
                Debug.Log($"判别式: {discriminant:F6}");
                
                if (System.Math.Abs(discriminant) < 1e-5f)
                    Debug.Log("SpringElastic判断: 临界阻尼");
                else if (discriminant > 0)
                    Debug.Log("SpringElastic判断: 欠阻尼");
                else
                    Debug.Log("SpringElastic判断: 过阻尼");
                
                // SpringPrecise计算
                float dampingCoeffPrecise = 2.0f * dampingRatio * naturalFreq;
                float stiffnessValuePrecise = naturalFreq * naturalFreq;
                
                Debug.Log($"SpringPrecise - dampingCoeff: {dampingCoeffPrecise:F3}, stiffnessValue: {stiffnessValuePrecise:F3}");
                
                if (dampingRatio > 1.0f)
                    Debug.Log("SpringPrecise判断: 过阻尼");
                else if (System.Math.Abs(dampingRatio - 1.0f) < 1e-5f)
                    Debug.Log("SpringPrecise判断: 临界阻尼");
                else
                    Debug.Log("SpringPrecise判断: 欠阻尼");
            }
            
            Debug.Log("========================");
        }
        
        // 辅助函数（从DamperUtility复制）
        private static double DampingToHalfLife(double damping, double eps = 1e-5f)
        {
            return (4.0d * 0.6931471805599453d) / (damping + eps);
        }

        private static double HalfLifeToDamping(double halfLife, double eps = 1e-5f)
        {
            return (4.0d * 0.6931471805599453d) / (halfLife + eps);
        }

        private static double DampingRatioToStiffness(double ratio, double damping)
        {
            return Square(damping / (ratio * 2.0f));
        }
        
        private static double Square(double x)
        {
            return x * x;
        }
        
        // 重置所有阻尼器状态
        [ContextMenu("Reset All Dampers")]
        public void ResetAllDampers()
        {
            InitializeStates();
            
            // 重置所有slider到当前主控值
            float targetValue = masterSlider != null ? masterSlider.value : 0f;
            
            if (springElasticOverdampedSlider != null) springElasticOverdampedSlider.value = targetValue;
            if (springElasticCriticalSlider != null) springElasticCriticalSlider.value = targetValue;
            if (springElasticUnderdampedSlider != null) springElasticUnderdampedSlider.value = targetValue;
            if (springPreciseOverdampedSlider != null) springPreciseOverdampedSlider.value = targetValue;
            if (springPreciseCriticalSlider != null) springPreciseCriticalSlider.value = targetValue;
            if (springPreciseUnderdampedSlider != null) springPreciseUnderdampedSlider.value = targetValue;
            if (springSimpleSlider != null) springSimpleSlider.value = targetValue;
            if (velocitySmoothingSlider != null) velocitySmoothingSlider.value = targetValue;
            if (durationLimitSlider != null) durationLimitSlider.value = targetValue;
            if (doubleSmoothingSlider != null) doubleSmoothingSlider.value = targetValue;
        }
        
        // 测试阻尼器计算逻辑
        [ContextMenu("测试阻尼器计算")]
        private void TestDamperCalculationsNew()
        {
            Debug.Log("=== 阻尼器计算测试 ===");
            
            // 测试不同阻尼比的计算
            float[] testRatios = { 0.1f, 1.0f, 2.5f };
            
            foreach (float dampingRatio in testRatios)
            {
                Debug.Log($"\n--- 阻尼比: {dampingRatio} ---");
                
                // 测试SpringElastic的计算 - 使用反射访问私有方法
                float stiffness = 1.0f;
                double naturalFreq = stiffness;
                
                var dampingToHalfLifeMethod = typeof(SpringUtility).GetMethod("DampingToHalfLife", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                var halfLifeToDampingMethod = typeof(SpringUtility).GetMethod("HalfLifeToDamping", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                var dampingRatioToStiffnessMethod = typeof(SpringUtility).GetMethod("DampingRatioToStiffness", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                
                if (dampingToHalfLifeMethod != null && halfLifeToDampingMethod != null && dampingRatioToStiffnessMethod != null)
                {
                    double halfLife = (double)dampingToHalfLifeMethod.Invoke(null, new object[] { 2.0 * dampingRatio * naturalFreq });
                    double dampingCoeff = (double)halfLifeToDampingMethod.Invoke(null, new object[] { halfLife });
                    double stiffnessValue = (double)dampingRatioToStiffnessMethod.Invoke(null, new object[] { dampingRatio, dampingCoeff });
                    
                    double discriminant = stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0;
                    
                    Debug.Log($"SpringElastic:");
                    Debug.Log($"  刚度值: {stiffnessValue:F6}, 阻尼系数: {dampingCoeff:F6}");
                    Debug.Log($"  判别式: {discriminant:F6}");
                    
                    if (System.Math.Abs(discriminant) < 1e-5)
                        Debug.Log("  判断结果: 临界阻尼");
                    else if (discriminant > 0)
                        Debug.Log("  判断结果: 欠阻尼");
                    else
                        Debug.Log("  判断结果: 过阻尼");
                }
                
                // 测试SpringPrecise的计算
                Debug.Log($"SpringPrecise:");
                if (System.Math.Abs(dampingRatio - 1.0f) < 1e-5)
                    Debug.Log("  判断结果: 临界阻尼");
                else if (dampingRatio > 1.0f)
                    Debug.Log("  判断结果: 过阻尼");
                else
                    Debug.Log("  判断结果: 欠阻尼");
            }
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
            
            // 阻尼器测试相关字符串 [英文, 中文, 日文]
            localizedStrings["DAMPER_UTILITY_TEST"] = new string[] {
                "DamperUtility Visual Test UI",
                "DamperUtility可视化测试UI",
                "DamperUtilityビジュアルテストUI"
            };
            
            localizedStrings["TEST_DAMPER_FUNCTIONS"] = new string[] {
                "Test various damper functions and parameter effects",
                "用于测试各种阻尼器函数的行为和参数效果",
                "様々なダンパー関数の動作とパラメータ効果をテスト"
            };
            
            localizedStrings["TIME_SCALE"] = new string[] {
                "Time Scale: {0:F2}x",
                "时间缩放: {0:F2}x",
                "時間スケール: {0:F2}x"
            };
            
            localizedStrings["MASTER_VALUE"] = new string[] {
                "Master Value: {0:F3}",
                "主控值: {0:F3}",
                "マスター値: {0:F3}"
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
            
            localizedStrings["TARGET_VELOCITY"] = new string[] {
                "Target Velocity: {0:F2}",
                "目标速度: {0:F2}",
                "目標速度: {0:F2}"
            };
            
            localizedStrings["DURATION"] = new string[] {
                "Duration: {0:F0}ms",
                "持续时间: {0:F0}ms",
                "持続時間: {0:F0}ms"
            };
            
            localizedStrings["SMOOTHING_VELOCITY"] = new string[] {
                "Smoothing Velocity: {0:F2}",
                "平滑速度: {0:F2}",
                "スムージング速度: {0:F2}"
            };
            
            localizedStrings["RIGHT_CLICK_FOLLOW_MOUSE"] = new string[] {
                "Right click to follow mouse",
                "右键跟随鼠标",
                "右クリックでマウスを追従"
            };
            
            localizedStrings["FOLLOWING"] = new string[] {
                "Following...",
                "跟随中..",
                "追従中..."
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
