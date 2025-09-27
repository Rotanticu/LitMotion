# LitMotion.Spring 测试工具故障排除指南

## 常见错误及解决方案

### 1. ArgumentException: Invalid type id

**错误信息**:
```
ArgumentException: Invalid type id.
LitMotion.MotionManager.CheckTypeId (LitMotion.MotionHandle& handle)
```

**原因**:
- MotionHandle没有正确初始化
- MotionHandle已经被销毁或无效
- 尝试操作未激活的MotionHandle

**解决方案**:
1. **初始化MotionHandle**: 确保所有MotionHandle都初始化为`MotionHandle.None`
2. **检查有效性**: 在调用Complete()之前检查`motion.IsActive()`
3. **使用安全方法**: 使用`SafeCompleteMotion()`方法

**修复后的代码**:
```csharp
// 初始化
private MotionHandle spring1DMotion = MotionHandle.None;

// 安全完成动画
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
            Debug.LogWarning("尝试完成无效的MotionHandle");
        }
        motion = MotionHandle.None;
    }
}
```

### 2. UI引用未分配

**错误信息**:
```
未找到Slider: spring1DSlider
未找到Text: timeScaleText
```

**原因**:
- UI元素没有正确创建
- 自动分配逻辑失败
- UI元素名称不匹配

**解决方案**:
1. **检查UI创建**: 确保`LitMotionSpringTestUISetup`正确创建了UI
2. **手动分配**: 在Inspector中手动分配UI引用
3. **检查命名**: 确保UI元素名称符合预期

**调试方法**:
```csharp
[ContextMenu("检查UI引用")]
public void CheckUIReferences()
{
    // 检查所有UI引用状态
    Debug.Log($"spring1DSlider: {(spring1DSlider != null ? "已分配" : "未分配")}");
}
```

### 3. Spring动画不播放

**可能原因**:
- 目标值与当前值相同
- Spring参数设置不当
- 动画被立即停止

**解决方案**:
1. **检查目标值**: 确保目标值与当前值不同
2. **调整参数**: 检查阻尼比和刚度设置
3. **检查动画状态**: 确保动画没有被意外停止

### 4. 性能问题

**表现**:
- 帧率下降
- 动画卡顿
- 内存使用过高

**解决方案**:
1. **减少动画数量**: 避免同时运行过多动画
2. **调整时间缩放**: 降低时间缩放减少计算负担
3. **优化参数**: 使用合适的Spring参数

## 调试技巧

### 1. 启用详细日志
```csharp
// 在测试方法中添加详细日志
Debug.Log($"开始1D Spring测试 - 目标值: {targetValue}, 阻尼比: {dampingRatio}, 刚度: {stiffness}");
```

### 2. 检查MotionHandle状态
```csharp
// 检查MotionHandle是否有效
if (spring1DMotion.IsActive())
{
    Debug.Log("Spring1D动画正在运行");
}
else
{
    Debug.Log("Spring1D动画未运行");
}
```

### 3. 监控性能
```csharp
// 在Update中监控性能
private void Update()
{
    if (isPerformanceTestRunning)
    {
        float fps = 1.0f / Time.deltaTime;
        if (fps < 30)
        {
            Debug.LogWarning($"帧率过低: {fps:F1} FPS");
        }
    }
}
```

## 最佳实践

### 1. MotionHandle管理
- 始终初始化为`MotionHandle.None`
- 使用`IsActive()`检查状态
- 使用`SafeCompleteMotion()`安全完成动画

### 2. UI引用管理
- 使用自动分配减少手动工作
- 定期检查UI引用状态
- 提供手动分配选项

### 3. 错误处理
- 使用try-catch处理异常
- 提供有意义的错误信息
- 记录调试信息

### 4. 性能优化
- 避免同时运行过多动画
- 使用合适的Spring参数
- 监控性能指标

## 测试流程

### 1. 基础功能测试
1. 创建测试UI
2. 检查UI引用分配
3. 测试1D Spring动画
4. 验证动画收敛

### 2. 参数测试
1. 测试不同阻尼比
2. 测试不同刚度
3. 测试不同目标值
4. 验证参数影响

### 3. 循环测试
1. 测试无循环
2. 测试无限循环
3. 测试有限循环
4. 验证循环行为

### 4. 延迟测试
1. 测试无延迟
2. 测试固定延迟
3. 测试自定义延迟
4. 验证延迟行为

### 5. 性能测试
1. 创建大量动画
2. 监控性能指标
3. 测试内存使用
4. 验证稳定性

## 联系支持

如果遇到无法解决的问题，请提供以下信息：
1. 完整的错误信息
2. 复现步骤
3. Unity版本信息
4. LitMotion版本信息
5. 测试环境配置

这将有助于快速定位和解决问题。
