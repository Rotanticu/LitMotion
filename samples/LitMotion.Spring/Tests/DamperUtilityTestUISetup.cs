using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ASUI.Tests
{
    /// <summary>
    /// DamperUtility测试UI的自动设置脚本
    /// 用于快速创建和配置测试UI界面
    /// </summary>
    [System.Serializable]
    public class DamperUtilityTestUISetup : MonoBehaviour
    {
        [Header("自动设置选项")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private bool createUIFromScratch = false;
        
        [Header("UI样式设置")]
        [SerializeField] private Color sliderColor = Color.white;
        [SerializeField] private Color textColor = Color.black;
        [SerializeField] private int fontSize = 14;
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupTestUI();
            }
        }
        
        [ContextMenu("设置测试UI")]
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
            GameObject canvasGO = new GameObject("DamperTestCanvas");
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
            
            // 创建控制面板 - 适配屏幕尺寸，放在左上角
            GameObject controlPanel = CreatePanel(mainPanel.transform, "ControlPanel", new Vector2(-700, 0), new Vector2(300, 600), new Color(1f, 1f, 1f, 0.3f));
            SetupControlPanel(controlPanel);
            
            // 创建测试面板
            GameObject testPanel = CreatePanel(mainPanel.transform, "TestPanel", new Vector2(0, 0), new Vector2(1200, 800), new Color(1f, 1f, 1f, 0.1f));
            SetupTestPanel(testPanel);
            
            // 创建鼠标跟随测试面板
            GameObject mouseFollowPanel = CreatePanel(mainPanel.transform, "MouseFollowPanel", new Vector2(700, 0), new Vector2(300, 200), new Color(1f, 1f, 1f, 0.2f));
            SetupMouseFollowPanel(mouseFollowPanel);
            
            // 添加测试脚本
            DamperUtilityTestUI testUI = mainPanel.AddComponent<DamperUtilityTestUI>();
            AssignUIReferences(testUI, controlPanel, testPanel, mouseFollowPanel);
        }
        
        private void SetupExistingUI()
        {
            // 查找现有的DamperUtilityTestUI组件
            DamperUtilityTestUI testUI = FindFirstObjectByType<DamperUtilityTestUI>();
            if (testUI == null)
            {
                Debug.LogError("未找到DamperUtilityTestUI组件！请先添加该组件到场景中。");
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
            // 时间缩放控制 - 用户控制，需要Handle，使用蓝色系
            CreateSliderWithLabel(controlPanel.transform, "timeScale", "时间缩放", new Vector2(0, 250), 0.1f, 3.0f, 1.0f, true, new Color(0.2f, 0.6f, 1.0f));
            
            // 主控slider - 用户控制，需要Handle，使用红色系
            CreateSliderWithLabel(controlPanel.transform, "master", "主控值", new Vector2(0, 200), 0f, 1f, 0f, true, new Color(1.0f, 0.3f, 0.3f));
            
            // 参数控制 - 用户控制，需要Handle，使用绿色系
            CreateSliderWithLabel(controlPanel.transform, "dampingRatio", "阻尼比", new Vector2(0, 150), 0.1f, 2.0f, 0.5f, true, new Color(0.3f, 0.8f, 0.3f));
            CreateSliderWithLabel(controlPanel.transform, "stiffness", "刚度", new Vector2(0, 100), 1f, 10f, 1.0f, true, new Color(0.3f, 0.8f, 0.3f));
            
            // 变种参数控制 - 用户控制，需要Handle，使用紫色系
            CreateSliderWithLabel(controlPanel.transform, "targetVelocity", "目标速度", new Vector2(0, 50), -2.0f, 2.0f, 0.0f, true, new Color(0.8f, 0.3f, 0.8f));
            CreateSliderWithLabel(controlPanel.transform, "duration", "持续时间", new Vector2(0, 0), 0.02f, 2f, 0.2f, true, new Color(0.8f, 0.3f, 0.8f));
            CreateSliderWithLabel(controlPanel.transform, "smothingVelocity", "平滑速度", new Vector2(0, -50), 0.5f, 5.0f, 1.0f, true, new Color(0.8f, 0.3f, 0.8f));
        }
        
        private void SetupTestPanel(GameObject testPanel)
        {
            // SpringElastic测试
            CreateTestSection(testPanel.transform, "SpringElastic", "SpringElastic测试", new Vector2(-400, 300), new Vector2(350, 200));
            
            // SpringPrecise测试
            CreateTestSection(testPanel.transform, "SpringPrecise", "SpringPrecise测试", new Vector2(0, 300), new Vector2(350, 200));
            
            // SpringSimple测试
            CreateTestSection(testPanel.transform, "SpringSimple", "SpringSimple测试", new Vector2(400, 300), new Vector2(350, 200));
            
            // 变种测试
            CreateTestSection(testPanel.transform, "Variants", "变种测试", new Vector2(0, 50), new Vector2(800, 200));
        }
        
        private void SetupMouseFollowPanel(GameObject mouseFollowPanel)
        {
            // 添加标题
            CreateText(mouseFollowPanel.transform, "MouseFollowTitle", "鼠标跟随测试", new Vector2(0, 80), new Vector2(280, 30));
            
            // 创建跟随目标
            GameObject followTarget = CreateFollowTarget(mouseFollowPanel.transform, "FollowTarget", new Vector2(0, 20));
            
            // 创建说明文本
            CreateText(mouseFollowPanel.transform, "FollowTargetText", "右键跟随鼠标", new Vector2(0, -40), new Vector2(280, 30));
        }
        
        private GameObject CreateFollowTarget(Transform parent, string name, Vector2 position)
        {
            GameObject followTarget = new GameObject(name);
            followTarget.transform.SetParent(parent, false);
            
            RectTransform rectTransform = followTarget.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(40, 40);
            
            // 添加圆形图像
            Image image = followTarget.AddComponent<Image>();
            image.color = new Color(1f, 0.2f, 0.2f, 0.8f); // 红色半透明
            
            // 创建圆形精灵
            Texture2D circleTexture = CreateCircleTexture(40, 40, Color.white);
            Sprite circleSprite = Sprite.Create(circleTexture, new Rect(0, 0, 40, 40), new Vector2(0.5f, 0.5f));
            image.sprite = circleSprite;
            
            return followTarget;
        }
        
        private Texture2D CreateCircleTexture(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];
            
            Vector2 center = new Vector2(width / 2f, height / 2f);
            float radius = Mathf.Min(width, height) / 2f - 2f; // 留2像素边距
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 pos = new Vector2(x, y);
                    float distance = Vector2.Distance(pos, center);
                    
                    if (distance <= radius)
                    {
                        pixels[y * width + x] = color;
                    }
                    else
                    {
                        pixels[y * width + x] = Color.clear;
                    }
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
        
        private void CreateTestSection(Transform parent, string sectionName, string title, Vector2 position, Vector2 size)
        {
            // 阻尼器slider的面板使用高透明度的白色
            GameObject section = CreatePanel(parent, sectionName + "Section", position, size, new Color(1f, 1f, 1f, 0.15f));
            
            // 添加标题
            CreateText(section.transform, sectionName + "Title", title, new Vector2(0, size.y/2 - 20), new Vector2(size.x - 20, 30));
            
            if (sectionName == "SpringElastic")
            {
                // SpringElastic - 橙色系，不同阻尼状态用不同深浅
                CreateSliderWithLabel(section.transform, "springElasticOverdamped", "过阻尼", new Vector2(0, 20), 0f, 1f, 0f, false, new Color(1.0f, 0.5f, 0.0f)); // 深橙色
                CreateSliderWithLabel(section.transform, "springElasticCritical", "临界阻尼", new Vector2(0, -20), 0f, 1f, 0f, false, new Color(1.0f, 0.7f, 0.2f)); // 中橙色
                CreateSliderWithLabel(section.transform, "springElasticUnderdamped", "欠阻尼", new Vector2(0, -60), 0f, 1f, 0f, false, new Color(1.0f, 0.9f, 0.4f)); // 浅橙色
            }
            else if (sectionName == "SpringPrecise")
            {
                // SpringPrecise - 青色系，不同阻尼状态用不同深浅
                CreateSliderWithLabel(section.transform, "springPreciseOverdamped", "过阻尼", new Vector2(0, 20), 0f, 1f, 0f, false, new Color(0.0f, 0.6f, 0.8f)); // 深青色
                CreateSliderWithLabel(section.transform, "springPreciseCritical", "临界阻尼", new Vector2(0, -20), 0f, 1f, 0f, false, new Color(0.2f, 0.8f, 1.0f)); // 中青色
                CreateSliderWithLabel(section.transform, "springPreciseUnderdamped", "欠阻尼", new Vector2(0, -60), 0f, 1f, 0f, false, new Color(0.4f, 1.0f, 1.0f)); // 浅青色
            }
            else if (sectionName == "SpringSimple")
            {
                // SpringSimple - 黄色系
                CreateSliderWithLabel(section.transform, "springSimple", "简单阻尼", new Vector2(0, 0), 0f, 1f, 0f, false, new Color(1.0f, 0.8f, 0.0f));
            }
            else if (sectionName == "Variants")
            {
                // 变种阻尼器 - 粉色系，不同变种用不同深浅
                CreateSliderWithLabel(section.transform, "velocitySmoothing", "速度平滑", new Vector2(-200, 20), 0f, 1f, 0f, false, new Color(1.0f, 0.4f, 0.8f)); // 深粉色
                CreateSliderWithLabel(section.transform, "durationLimit", "时间限制", new Vector2(0, 20), 0f, 1f, 0f, false, new Color(1.0f, 0.6f, 0.9f)); // 中粉色
                CreateSliderWithLabel(section.transform, "doubleSmoothing", "双重平滑", new Vector2(200, 20), 0f, 1f, 0f, false, new Color(1.0f, 0.8f, 1.0f)); // 浅粉色
            }
        }
        
        private void CreateSliderWithLabel(Transform parent, string name, string label, Vector2 position, float minValue, float maxValue, float defaultValue, bool hasHandle = true, Color? fillColor = null)
        {
            // 创建容器
            GameObject container = new GameObject(name + "Container");
            container.transform.SetParent(parent, false);
            
            RectTransform containerRect = container.AddComponent<RectTransform>();
            containerRect.anchoredPosition = position;
            containerRect.sizeDelta = new Vector2(300, 40);
            
            // 创建标签
            CreateText(container.transform, name + "Label", label, new Vector2(-120, 0), new Vector2(100, 30));
            
            // 创建Slider
            GameObject sliderGO = new GameObject(name + "Slider");
            sliderGO.transform.SetParent(container.transform, false);
            
            // 使用Unity的DefaultControls创建Slider
            var sliderData = UnityEngine.UI.DefaultControls.CreateSlider(new UnityEngine.UI.DefaultControls.Resources());
            GameObject defaultSlider = sliderData.gameObject;
            defaultSlider.transform.SetParent(sliderGO.transform, false);
            defaultSlider.name = name + "Slider"; // 设置正确的名称用于自动分配
            
            // 设置Slider的RectTransform
            RectTransform sliderRect = defaultSlider.GetComponent<RectTransform>();
            sliderRect.anchoredPosition = new Vector2(50, 0);
            sliderRect.sizeDelta = new Vector2(150, 20);
            
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
                // 移除Handle Slide Area
                Transform handleSlideArea = defaultSlider.transform.Find("Handle Slide Area");
                if (handleSlideArea != null)
                {
                    DestroyImmediate(handleSlideArea.gameObject);
                }
            }
            
            // 创建值显示文本
            CreateText(container.transform, name + "Value", defaultValue.ToString("F3"), new Vector2(120, 0), new Vector2(80, 30));
        }
        
        private void CreateText(Transform parent, string name, string text, Vector2 position, Vector2 size)
        {
            GameObject textGO = new GameObject(name);
            textGO.transform.SetParent(parent, false);
            
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchoredPosition = position;
            textRect.sizeDelta = size;
            
            TextMeshProUGUI textComponent = textGO.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.color = textColor;
            textComponent.alignment = TextAlignmentOptions.Center;
        }
        
        private void AssignUIReferences(DamperUtilityTestUI testUI, GameObject controlPanel, GameObject testPanel, GameObject mouseFollowPanel)
        {
            // 自动分配UI引用
            AutoAssignUIReferences(testUI);
            
            Debug.Log("UI引用分配完成。");
            Debug.Log("控制面板: " + controlPanel.name);
            Debug.Log("测试面板: " + testPanel.name);
            Debug.Log("鼠标跟随面板: " + mouseFollowPanel.name);
        }
        
        private void AutoAssignUIReferences(DamperUtilityTestUI testUI)
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
                        field.SetValue(testUI, foundRect);
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
    }
}
