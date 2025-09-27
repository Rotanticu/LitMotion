using UnityEngine;

namespace ASUI.Tests
{
    /// <summary>
    /// LitMotion.Spring测试启动器
    /// 用于快速启动Spring动画测试
    /// </summary>
    public class LitMotionSpringTestLauncher : MonoBehaviour
    {
        [Header("测试设置")]
        [SerializeField] private bool autoStartTest = true;
        [SerializeField] private bool createTestUI = true;
        
        [SerializeField] private bool runAllTests = true;
        
        private void Start()
        {
            if (autoStartTest)
            {
                StartCoroutine(RunTests());
            }
        }
        
        private System.Collections.IEnumerator RunTests()
        {
            Debug.Log("=== LitMotion.Spring测试开始 ===");
            
            if (createTestUI)
            {
                // 创建测试UI
                CreateTestUI();
                yield return new WaitForSeconds(1f);
            }
            
            if (runAllTests)
            {
                // 运行所有测试
                yield return StartCoroutine(RunAllTests());
            }
            
            Debug.Log("=== LitMotion.Spring测试完成 ===");
        }
        
        private void CreateTestUI()
        {
            // 查找或创建测试UI设置组件
            LitMotionSpringTestUISetup setup = FindFirstObjectByType<LitMotionSpringTestUISetup>();
            if (setup == null)
            {
                GameObject setupGO = new GameObject("LitMotionSpringTestSetup");
                setup = setupGO.AddComponent<LitMotionSpringTestUISetup>();
            }
            
            // 设置测试UI
            setup.SetupTestUI();
            Debug.Log("测试UI创建完成");
        }
        
        private System.Collections.IEnumerator RunAllTests()
        {
            // 查找测试UI组件
            LitMotionSpringTestUI testUI = FindFirstObjectByType<LitMotionSpringTestUI>();
            if (testUI == null)
            {
                Debug.LogError("未找到LitMotionSpringTestUI组件！");
                yield break;
            }
            
            Debug.Log("开始运行所有测试...");
            
            // 测试1D Spring
            Debug.Log("测试1D Spring...");
            testUI.Test1DSpring();
            yield return new WaitForSeconds(2f);
            
            // 测试2D Spring
            Debug.Log("测试2D Spring...");
            testUI.Test2DSpring();
            yield return new WaitForSeconds(2f);
            
            // 测试3D Spring
            Debug.Log("测试3D Spring...");
            testUI.Test3DSpring();
            yield return new WaitForSeconds(2f);
            
            // 测试4D Spring
            Debug.Log("测试4D Spring...");
            testUI.Test4DSpring();
            yield return new WaitForSeconds(2f);
            
            // 性能测试
            Debug.Log("开始性能测试...");
            testUI.TestPerformance();
            yield return new WaitForSeconds(5f);
            
            // 停止所有动画
            testUI.StopAllAnimations();
            Debug.Log("所有测试完成");
        }
        
        [ContextMenu("运行所有测试")]
        public void RunAllTestsManually()
        {
            StartCoroutine(RunAllTests());
        }
        
        [ContextMenu("创建测试UI")]
        public void CreateTestUIManually()
        {
            CreateTestUI();
        }
        
        [ContextMenu("检查测试状态")]
        public void CheckTestStatus()
        {
            LitMotionSpringTestUI testUI = FindFirstObjectByType<LitMotionSpringTestUI>();
            if (testUI != null)
            {
                testUI.CheckUIReferences();
                Debug.Log("测试状态检查完成");
            }
            else
            {
                Debug.LogError("未找到LitMotionSpringTestUI组件！");
            }
        }
    }
}
