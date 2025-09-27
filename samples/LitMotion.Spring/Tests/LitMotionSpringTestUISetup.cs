using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ASUI.Tests
{
    /// <summary>
    /// LitMotion.Spring测试UI的自动设置脚本
    /// 用于快速创建和配置Spring动画测试UI界面
    /// </summary>
    [System.Serializable]
    public class LitMotionSpringTestUISetup : MonoBehaviour
    {
        [Header("自动设置选项")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private bool createUIFromScratch = false;
        
        [Header("UI样式设置")]
        [SerializeField] private Color sliderColor = Color.white;
        [SerializeField] private Color textColor = Color.white;
        [SerializeField] private int fontSize = 14;
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupTestUI();
            }
        }
        
        [ContextMenu("设置Spring测试UI")]
        public void SetupTestUI()
        {
            if (createUIFromScratch)
            {
                CreateUIFromScratch();
            }
            else
            {
                SetupExistingUI();
            }
        }
        
        private void CreateUIFromScratch()
        {
            // 创建Canvas
            GameObject canvasGO = new GameObject("LitMotionSpringTestCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // 创建主容器
            GameObject mainPanel = CreatePanel(canvasGO.transform, "MainPanel", new Vector2(0, 0), new Vector2(1920, 1080));
            
            // 创建控制面板
            GameObject controlPanel = CreatePanel(mainPanel.transform, "ControlPanel", new Vector2(-800, 0), new Vector2(350, 700), new Color(1f, 1f, 1f, 0.3f));
            SetupControlPanel(controlPanel);
            
            // 创建测试面板
            GameObject testPanel = CreatePanel(mainPanel.transform, "TestPanel", new Vector2(0, 0), new Vector2(1400, 800), new Color(1f, 1f, 1f, 0.1f));
            SetupTestPanel(testPanel);
            
            // 添加测试脚本
            LitMotionSpringTestUI testUI = mainPanel.AddComponent<LitMotionSpringTestUI>();
            AssignUIReferences(testUI, controlPanel, testPanel);
        }
        
        private void SetupExistingUI()
        {
            // 查找现有的LitMotionSpringTestUI组件
            LitMotionSpringTestUI testUI = FindFirstObjectByType<LitMotionSpringTestUI>();
            if (testUI == null)
            {
                Debug.LogError("未找到LitMotionSpringTestUI组件！请先添加该组件到场景中。");
                return;
            }
            
            // 自动查找和分配UI引用
            AutoAssignUIReferences(testUI);
        }
        
        private GameObject CreatePanel(Transform parent, string name, Vector2 position, Vector2 size, Color? backgroundColor = null)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            
            RectTransform rectTransform = panel.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = size;
            
            Image image = panel.AddComponent<Image>();
            image.color = backgroundColor ?? new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            return panel;
        }
        
        private void SetupControlPanel(GameObject controlPanel)
        {
            // 时间缩放控制
            CreateSliderWithLabel(controlPanel.transform, "timeScale", "时间缩放", new Vector2(0, 300), 0.1f, 3.0f, 1.0f, true, new Color(0.2f, 0.6f, 1.0f));
            
            // Spring参数控制
            CreateSliderWithLabel(controlPanel.transform, "dampingRatio", "阻尼比", new Vector2(0, 250), 0.1f, 2.0f, 0.5f, true, new Color(0.3f, 0.8f, 0.3f));
            CreateSliderWithLabel(controlPanel.transform, "stiffness", "刚度", new Vector2(0, 200), 1f, 20f, 10.0f, true, new Color(0.3f, 0.8f, 0.3f));
            
            // 目标值控制
            CreateSliderWithLabel(controlPanel.transform, "targetValue", "目标值", new Vector2(0, 150), 0f, 1f, 0f, true, new Color(1.0f, 0.3f, 0.3f));
            CreateButton(controlPanel.transform, "setRandomTarget", "Random", new Vector2(0, 120), new Vector2(140, 30), new Color(1.0f, 0.5f, 0.0f));
            CreateButton(controlPanel.transform, "setHalfTarget", "Half", new Vector2(0, 85), new Vector2(140, 25), new Color(1.0f, 0.7f, 0.0f));
            
            // 延迟控制
            CreateSliderWithLabel(controlPanel.transform, "delay", "延迟时间", new Vector2(0, 50), 0f, 3f, 0f, true, new Color(0.8f, 0.3f, 0.8f));
            
            // 循环次数控制
            CreateSliderWithLabel(controlPanel.transform, "loopCount", "循环次数", new Vector2(0, 0), 0f, 5f, 0f, true, new Color(0.3f, 0.8f, 0.8f));
            
            // 循环类型选择
            CreateDropdownWithLabel(controlPanel.transform, "loopType", "循环类型", new Vector2(0, -50), new string[] { "Restart", "Flip", "Incremental", "Yoyo" });
            
            // 延迟类型选择
            CreateDropdownWithLabel(controlPanel.transform, "delayType", "延迟类型", new Vector2(0, -100), new string[] { "FirstLoop", "EveryLoop" });
            
            // 测试所有组合按钮
            CreateButton(controlPanel.transform, "testAllCombinations", "Test All", new Vector2(0, -150), new Vector2(140, 30), new Color(0.5f, 0.3f, 0.8f));
        }
        
        private void SetupTestPanel(GameObject testPanel)
        {
            // 1D Spring测试
            CreateTestSection(testPanel.transform, "Spring1D", "1D Spring测试", new Vector2(-600, 300), new Vector2(280, 200));
            
            // 2D Spring测试
            CreateTestSection(testPanel.transform, "Spring2D", "2D Spring测试", new Vector2(-300, 300), new Vector2(280, 200));
            
            // 3D Spring测试
            CreateTestSection(testPanel.transform, "Spring3D", "3D Spring测试", new Vector2(0, 300), new Vector2(280, 200));
            
            // 4D Spring测试
            CreateTestSection(testPanel.transform, "Spring4D", "4D Spring测试", new Vector2(300, 300), new Vector2(280, 200));
            
            // 性能测试
            CreateTestSection(testPanel.transform, "Performance", "性能测试", new Vector2(0, 50), new Vector2(800, 200));
            
            // 性能测试结果显示区域
            CreatePerformanceResultSection(testPanel.transform, "PerformanceResult", "性能测试结果", new Vector2(0, -200), new Vector2(800, 300));
        }
        
        private void CreateTestSection(Transform parent, string sectionName, string title, Vector2 position, Vector2 size)
        {
            // 创建测试区域面板
            GameObject section = CreatePanel(parent, sectionName + "Section", position, size, new Color(1f, 1f, 1f, 0.15f));
            
            // 添加标题
            CreateText(section.transform, sectionName + "Title", title, new Vector2(0, size.y/2 - 20), new Vector2(size.x - 20, 30));
            
            if (sectionName == "Spring1D")
            {
                // 1D Spring测试 - 蓝色系
                CreateSliderWithLabel(section.transform, "spring1D", "1D Spring动画", new Vector2(0, 20), 0f, 1f, 0f, false, new Color(0.2f, 0.6f, 1.0f));
                CreateButton(section.transform, "test1D", "Start", new Vector2(-60, -20), new Vector2(80, 30), new Color(0.2f, 0.6f, 1.0f));
                CreateButton(section.transform, "stop1D", "Stop", new Vector2(60, -20), new Vector2(80, 30), new Color(0.8f, 0.3f, 0.3f));
            }
            else if (sectionName == "Spring2D")
            {
                // 2D Spring测试 - 绿色系
                CreateSliderWithLabel(section.transform, "spring2D", "2D Spring动画", new Vector2(0, 20), 0f, 1f, 0f, false, new Color(0.3f, 0.8f, 0.3f));
                CreateButton(section.transform, "test2D", "Start", new Vector2(-60, -20), new Vector2(80, 30), new Color(0.3f, 0.8f, 0.3f));
                CreateButton(section.transform, "stop2D", "Stop", new Vector2(60, -20), new Vector2(80, 30), new Color(0.8f, 0.3f, 0.3f));
            }
            else if (sectionName == "Spring3D")
            {
                // 3D Spring测试 - 橙色系
                CreateSliderWithLabel(section.transform, "spring3D", "3D Spring动画", new Vector2(0, 20), 0f, 1f, 0f, false, new Color(1.0f, 0.6f, 0.2f));
                CreateButton(section.transform, "test3D", "Start", new Vector2(-60, -20), new Vector2(80, 30), new Color(1.0f, 0.6f, 0.2f));
                CreateButton(section.transform, "stop3D", "Stop", new Vector2(60, -20), new Vector2(80, 30), new Color(0.8f, 0.3f, 0.3f));
            }
            else if (sectionName == "Spring4D")
            {
                // 4D Spring测试 - 紫色系
                CreateSliderWithLabel(section.transform, "spring4D", "4D Spring动画", new Vector2(0, 20), 0f, 1f, 0f, false, new Color(0.8f, 0.3f, 0.8f));
                CreateButton(section.transform, "test4D", "Start", new Vector2(-60, -20), new Vector2(80, 30), new Color(0.8f, 0.3f, 0.8f));
                CreateButton(section.transform, "stop4D", "Stop", new Vector2(60, -20), new Vector2(80, 30), new Color(0.8f, 0.3f, 0.3f));
            }
            else if (sectionName == "Performance")
            {
                // 性能测试 - 红色系
                CreateSliderWithLabel(section.transform, "performance", "性能测试进度", new Vector2(0, 20), 0f, 1f, 0f, false, new Color(1.0f, 0.3f, 0.3f));
                CreateButton(section.transform, "testPerformance", "Start", new Vector2(-60, -20), new Vector2(80, 30), new Color(1.0f, 0.3f, 0.3f));
                CreateButton(section.transform, "stopPerformance", "Stop", new Vector2(60, -20), new Vector2(80, 30), new Color(0.8f, 0.3f, 0.3f));
            }
        }
        
        private void CreatePerformanceResultSection(Transform parent, string sectionName, string title, Vector2 position, Vector2 size)
        {
            // 创建性能测试结果区域面板
            GameObject section = CreatePanel(parent, sectionName + "Section", position, size, new Color(1f, 1f, 1f, 0.1f));
            
            // 添加标题
            CreateText(section.transform, sectionName + "Title", title, new Vector2(0, size.y/2 - 20), new Vector2(size.x - 20, 30));
            
            // 创建性能测试结果显示文本
            CreateText(section.transform, "performanceResult", "点击'性能测试'开始测试\n当前测试类型: Spring动画\n测试动画数量: 1,000,000\n测试持续时间: 3秒", 
                      new Vector2(0, 0), new Vector2(size.x - 40, size.y - 60));
        }
        
        private void CreateSliderWithLabel(Transform parent, string name, string label, Vector2 position, float minValue, float maxValue, float defaultValue, bool hasHandle = true, Color? fillColor = null)
        {
            // 创建容器
            GameObject container = new GameObject(name + "Container");
            container.transform.SetParent(parent, false);
            
            RectTransform containerRect = container.AddComponent<RectTransform>();
            containerRect.anchoredPosition = position;
            containerRect.sizeDelta = new Vector2(250, 40);
            
            // 创建标签
            CreateText(container.transform, name + "Label", label, new Vector2(-100, 0), new Vector2(80, 30));
            
            // 创建Slider
            GameObject sliderGO = new GameObject(name + "Slider");
            sliderGO.transform.SetParent(container.transform, false);
            
            // 使用Unity的DefaultControls创建Slider
            var sliderData = UnityEngine.UI.DefaultControls.CreateSlider(new UnityEngine.UI.DefaultControls.Resources());
            GameObject defaultSlider = sliderData.gameObject;
            defaultSlider.transform.SetParent(sliderGO.transform, false);
            defaultSlider.name = name + "Slider";
            
            // 设置Slider的RectTransform
            RectTransform sliderRect = defaultSlider.GetComponent<RectTransform>();
            sliderRect.anchoredPosition = new Vector2(50, 0);
            sliderRect.sizeDelta = new Vector2(120, 20);
            
            // 获取Slider组件并设置参数
            Slider slider = defaultSlider.GetComponent<Slider>();
            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.value = defaultValue;
            
            // 设置填充颜色
            if (fillColor.HasValue)
            {
                Image fillImage = slider.fillRect.GetComponent<Image>();
                if (fillImage != null)
                {
                    fillImage.color = fillColor.Value;
                }
            }
            
            // 如果不需要Handle，移除Handle相关组件
            if (!hasHandle)
            {
                slider.handleRect = null;
                Transform handleSlideArea = defaultSlider.transform.Find("Handle Slide Area");
                if (handleSlideArea != null)
                {
                    DestroyImmediate(handleSlideArea.gameObject);
                }
            }
            
            // 创建值显示文本
            CreateText(container.transform, name + "Value", defaultValue.ToString("F3"), new Vector2(100, 0), new Vector2(60, 30));
        }
        
        private void CreateDropdownWithLabel(Transform parent, string name, string label, Vector2 position, string[] options)
        {
            // 创建容器
            GameObject container = new GameObject(name + "Container");
            container.transform.SetParent(parent, false);
            
            RectTransform containerRect = container.AddComponent<RectTransform>();
            containerRect.anchoredPosition = position;
            containerRect.sizeDelta = new Vector2(250, 40);
            
            // 创建标签
            CreateText(container.transform, name + "Label", label, new Vector2(-100, 0), new Vector2(80, 30));
            
            // 创建Dropdown
            GameObject dropdownGO = new GameObject(name + "Dropdown");
            dropdownGO.transform.SetParent(container.transform, false);
            
            // 使用Unity的DefaultControls创建Dropdown
            var dropdownData = UnityEngine.UI.DefaultControls.CreateDropdown(new UnityEngine.UI.DefaultControls.Resources());
            GameObject defaultDropdown = dropdownData.gameObject;
            defaultDropdown.transform.SetParent(dropdownGO.transform, false);
            defaultDropdown.name = name + "Dropdown";
            
            // 设置Dropdown的RectTransform
            RectTransform dropdownRect = defaultDropdown.GetComponent<RectTransform>();
            dropdownRect.anchoredPosition = new Vector2(50, 0);
            dropdownRect.sizeDelta = new Vector2(120, 30);
            
            // 获取Dropdown组件并设置选项
            Dropdown dropdown = defaultDropdown.GetComponent<Dropdown>();
            dropdown.ClearOptions();
            dropdown.AddOptions(new System.Collections.Generic.List<string>(options));
        }
        
        private void CreateButton(Transform parent, string name, string text, Vector2 position, Vector2 size, Color? buttonColor = null)
        {
            // 创建Button
            GameObject buttonGO = new GameObject(name + "Button");
            buttonGO.transform.SetParent(parent, false);
            
            // 使用Unity的DefaultControls创建Button
            var buttonData = UnityEngine.UI.DefaultControls.CreateButton(new UnityEngine.UI.DefaultControls.Resources());
            GameObject defaultButton = buttonData.gameObject;
            defaultButton.transform.SetParent(buttonGO.transform, false);
            defaultButton.name = name + "Button";
            
            // 设置Button的RectTransform
            RectTransform buttonRect = defaultButton.GetComponent<RectTransform>();
            buttonRect.anchoredPosition = position;
            buttonRect.sizeDelta = size;
            
            // 获取Button组件并设置文本
            Button button = defaultButton.GetComponent<Button>();
            Text buttonText = defaultButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = text;
                buttonText.fontSize = fontSize;
            }
            
            // 设置按钮颜色
            if (buttonColor.HasValue)
            {
                ColorBlock colors = button.colors;
                colors.normalColor = buttonColor.Value;
                colors.highlightedColor = buttonColor.Value * 1.2f;
                colors.pressedColor = buttonColor.Value * 0.8f;
                button.colors = colors;
            }
        }
        
        private void CreateText(Transform parent, string name, string text, Vector2 position, Vector2 size)
        {
            GameObject textGO = new GameObject(name);
            textGO.transform.SetParent(parent, false);
            
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchoredPosition = position;
            textRect.sizeDelta = size;
            
            Text textComponent = textGO.AddComponent<Text>();
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.color = textColor;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        
        private void CreateUnityText(Transform parent, string name, string text, Vector2 position, Vector2 size)
        {
            GameObject textGO = new GameObject(name);
            textGO.transform.SetParent(parent, false);
            
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchoredPosition = position;
            textRect.sizeDelta = size;
            
            Text textComponent = textGO.AddComponent<Text>();
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.color = textColor;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        
        private void AssignUIReferences(LitMotionSpringTestUI testUI, GameObject controlPanel, GameObject testPanel)
        {
            // 自动分配UI引用
            AutoAssignUIReferences(testUI);
            
            Debug.Log("UI引用分配完成。");
            Debug.Log("控制面板: " + controlPanel.name);
            Debug.Log("测试面板: " + testPanel.name);
        }
        
        private void AutoAssignUIReferences(LitMotionSpringTestUI testUI)
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
                        field.SetValue(testUI, foundSlider);
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
                        field.SetValue(testUI, foundText);
                        Debug.Log($"自动分配TextMeshPro: {field.Name} -> {foundText.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"未找到TextMeshPro: {field.Name}");
                    }
                }
                else if (field.FieldType == typeof(Text))
                {
                    Text foundText = FindUnityTextByName(field.Name);
                    if (foundText != null)
                    {
                        field.SetValue(testUI, foundText);
                        Debug.Log($"自动分配UnityText: {field.Name} -> {foundText.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"未找到UnityText: {field.Name}");
                    }
                }
                else if (field.FieldType == typeof(Button))
                {
                    Button foundButton = FindButtonByName(field.Name);
                    if (foundButton != null)
                    {
                        field.SetValue(testUI, foundButton);
                        Debug.Log($"自动分配Button: {field.Name} -> {foundButton.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"未找到Button: {field.Name}");
                    }
                }
                else if (field.FieldType == typeof(Dropdown))
                {
                    Dropdown foundDropdown = FindDropdownByName(field.Name);
                    if (foundDropdown != null)
                    {
                        field.SetValue(testUI, foundDropdown);
                        Debug.Log($"自动分配Dropdown: {field.Name} -> {foundDropdown.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"未找到Dropdown: {field.Name}");
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
    }
}
