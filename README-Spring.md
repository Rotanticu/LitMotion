# LitMotion Spring Animation Extension

## 🎯 Purpose

This extension adds physics-based spring animations to LitMotion, providing natural and responsive motion that simulates real-world spring physics. Perfect for modern UI interactions and smooth camera movements.

## ✨ Key Features

- **🌊 Physics-based Motion**: Natural spring physics simulation
- **📐 Multi-dimensional**: Support for float, Vector2, Vector3, and Vector4
- **⚡ High Performance**: Burst-compiled with SIMD optimizations
- **🔄 Velocity Preservation**: Smooth animation interruptions
- **🎛️ Configurable**: Adjustable stiffness and damping parameters

## 🚀 Quick Start

### Basic Usage

```csharp
// Simple spring animation
var motion = LMotion.Spring.Create(0f, 100f, SpringOptions.Critical)
    .Bind(value => transform.position.x = value);
```

### Predefined Configurations

```csharp
SpringOptions.Critical    // Fastest convergence, no oscillation
SpringOptions.Overdamped  // Smooth, slow convergence
SpringOptions.Underdamped // Bouncy, oscillating motion
```

### Custom Parameters

```csharp
var options = new SpringOptions(
    stiffness: 15f,        // Higher = faster convergence
    dampingRatio: 0.8f     // < 1.0 = bouncy, = 1.0 = critical, > 1.0 = smooth
);
```

### Continuous Tracking

```csharp
// For continuous tracking that requires manual stopping
var continuousMotion = LMotion.Spring.Create(0f, 10f, SpringOptions.Critical)
    .WithLoops(-1, LoopType.Incremental)  // Infinite loops
    .Bind(value => transform.position.x = value);

// Manually stop when needed
continuousMotion.Cancel();
```

**Important:** 
- Use `LoopType.Incremental` with `loops = -1` for continuous tracking
- `MotionHandle.TotalDuration` is meaningless for spring animations
- Spring animations require manual stopping

## 📋 Supported Features

| Feature | Status | Notes |
|---------|--------|-------|
| **Multi-dimensional** | ✅ | float, Vector2, Vector3, Vector4 |
| **Loops** | ✅ | All loop types supported |
| **Delays** | ✅ | First loop and every loop delays |
| **Callbacks** | ✅ | OnComplete, OnUpdate, OnCancel |
| **Time Scale** | ✅ | Respects Unity's time scale |
| **Pause/Resume** | ✅ | Full control support |
| **Cancel** | ✅ | Immediate cancellation |

## ❌ Unsupported Features

| Feature | Reason |
|---------|--------|
| **Fixed Duration** | Spring animations converge based on physics, not time |
| **Easing Curves** | Physics replace traditional easing |
| **Linear Interpolation** | Spring motion is inherently non-linear |
| **TotalDuration** | `MotionHandle.TotalDuration` is meaningless for physics-based animations |
| **Auto-completion** | Requires manual stopping for continuous tracking |

## 🎮 Use Cases

### UI Animations
```csharp
// Button press effect
var press = LMotion.Spring.Create(1f, 0.9f, SpringOptions.Overdamped)
    .Bind(scale => button.transform.localScale = Vector3.one * scale);
```

### Camera Movement
```csharp
// Smooth camera following
var follow = LMotion.Spring.Create(cameraPos, targetPos, SpringOptions.Critical)
    .Bind(pos => camera.transform.position = pos);
```

### Color Transitions
```csharp
// Natural color changes
var color = LMotion.Spring.Create(currentColor, targetColor, SpringOptions.Underdamped)
    .Bind(c => material.color = c);
```

## ⚙️ Parameter Guide

### Stiffness (5-20 recommended)
- **Low (5-10)**: Slow, gentle motion
- **Medium (10-15)**: Balanced motion
- **High (15-20)**: Fast, snappy motion

### Damping Ratio
- **< 1.0**: Underdamped (bouncy, oscillates)
- **= 1.0**: Critical damping (fastest, no oscillation)
- **> 1.0**: Overdamped (smooth, slow)

## 🔄 Migration from Traditional Tweens

### Before
```csharp
var tween = LMotion.Create(0f, 10f, 2f)
    .WithEase(Ease.OutQuad)
    .Bind(value => transform.position.x = value);
```

### After
```csharp
var spring = LMotion.Spring.Create(0f, 10f, SpringOptions.Critical)
    .Bind(value => transform.position.x = value);
```

## 🎯 When to Use Spring vs Traditional Tweens

### Use Spring When:
- You want natural, physics-based motion
- Animation timing is flexible
- You need smooth interruption handling
- Creating modern UI interactions

### Use Traditional Tweens When:
- You need precise timing control
- You require specific easing curves
- You need linear interpolation
- Working with legacy systems

## 🛠️ Performance Tips

1. **Use Burst Compilation**: Build versions automatically use Burst for maximum performance
2. **Reuse SpringOptions**: Create once, use multiple times
3. **High Performance**: Spring animations are actually faster than traditional tweens
4. **Zero GC Design**: LitMotion uses structs and value types - no object pooling needed
5. **No Concurrency Limits**: Performance tested to handle large numbers of concurrent animations

## 📚 Documentation

For complete API reference and advanced usage, see [Spring Animation Documentation](./Spring-Animation-Documentation.md).

## 🤝 Contributing

This extension follows LitMotion's design principles and integrates seamlessly with the existing API. Contributions that maintain compatibility and performance are welcome.

## 📄 License

Same as LitMotion - see the main LitMotion repository for license information.
