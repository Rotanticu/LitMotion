# LitMotion Spring Animation API Reference

## Namespace
```csharp
using LitMotion;
using LitMotion.Adapters;
```

## Core Classes

### SpringOptions

```csharp
public struct SpringOptions : IMotionOptions
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `CurrentValue` | `float4` | Current position of the spring |
| `CurrentVelocity` | `float4` | Current velocity of the spring |
| `TargetValue` | `float4` | Target position to converge to |
| `TargetVelocity` | `float4` | Target velocity (usually zero) |
| `Stiffness` | `float` | Spring stiffness (higher = faster) |
| `DampingRatio` | `float` | Damping ratio (1.0 = critical damping) |

#### Constructors

```csharp
// Default constructor
public SpringOptions(float stiffness = 10.0f, float dampingRatio = 1.0f, 
                     float4 startVelocity = default, float4 targetVelocity = default)

// Initialize with values
public void Init(float4 startValue, float4 targetValue, 
                 float4 startVelocity = default, float4 targetVelocity = default)
```

#### Static Properties

```csharp
// Critical damping - fastest convergence without oscillation
public static SpringOptions Critical { get; }

// Overdamped - smooth, slow convergence
public static SpringOptions Overdamped { get; }

// Underdamped - bouncy, oscillating motion
public static SpringOptions Underdamped { get; }
```

#### Methods

```csharp
// Restart animation from initial values
public void Restart()

// Check equality
public bool Equals(SpringOptions other)
```

### LMotion.Spring

```csharp
public static class Spring
```

#### Methods

##### Create Float Spring
```csharp
public static MotionBuilder<float, SpringOptions, FloatSpringMotionAdapter> Create(
    float startValue, 
    float endValue, 
    SpringOptions options = default)
```

**Parameters:**
- `startValue`: Starting value
- `endValue`: Target value
- `options`: Spring configuration

**Returns:** MotionBuilder for chaining

**Example:**
```csharp
var motion = LMotion.Spring.Create(0f, 100f, SpringOptions.Critical)
    .Bind(value => transform.position.x = value);
```

##### Create Vector2 Spring
```csharp
public static MotionBuilder<Vector2, SpringOptions, Vector2SpringMotionAdapter> Create(
    Vector2 startValue, 
    Vector2 endValue, 
    SpringOptions options = default, 
    Vector2 targetVelocity = default)
```

**Parameters:**
- `startValue`: Starting Vector2
- `endValue`: Target Vector2
- `options`: Spring configuration
- `targetVelocity`: Target velocity (optional)

**Example:**
```csharp
var motion = LMotion.Spring.Create(Vector2.zero, new Vector2(100f, 50f), SpringOptions.Critical)
    .Bind(value => transform.position = value);
```

##### Create Vector3 Spring
```csharp
public static MotionBuilder<Vector3, SpringOptions, Vector3SpringMotionAdapter> Create(
    Vector3 startValue, 
    Vector3 endValue, 
    SpringOptions options = default, 
    Vector3 targetVelocity = default)
```

**Example:**
```csharp
var motion = LMotion.Spring.Create(Vector3.zero, new Vector3(10f, 5f, 0f), SpringOptions.Critical)
    .Bind(value => transform.position = value);
```

##### Create Vector4 Spring
```csharp
public static MotionBuilder<Vector4, SpringOptions, Vector4SpringMotionAdapter> Create(
    Vector4 startValue, 
    Vector4 endValue, 
    SpringOptions options = default, 
    Vector4 targetVelocity = default)
```

**Example:**
```csharp
var motion = LMotion.Spring.Create(Vector4.zero, new Vector4(1f, 0.5f, 0.2f, 1f), SpringOptions.Critical)
    .Bind(value => material.color = value);
```

## Motion Adapters

### FloatSpringMotionAdapter
```csharp
public readonly struct FloatSpringMotionAdapter : IMotionAdapter<float, SpringOptions>
```

### Vector2SpringMotionAdapter
```csharp
public readonly struct Vector2SpringMotionAdapter : IMotionAdapter<Vector2, SpringOptions>
```

### Vector3SpringMotionAdapter
```csharp
public readonly struct Vector3SpringMotionAdapter : IMotionAdapter<Vector3, SpringOptions>
```

### Vector4SpringMotionAdapter
```csharp
public readonly struct Vector4SpringMotionAdapter : IMotionAdapter<Vector4, SpringOptions>
```

## MotionBuilder Extensions

All standard LitMotion MotionBuilder methods are supported:

### Animation Control
```csharp
// Pause animation
motion.Pause();

// Resume animation
motion.Resume();

// Cancel animation
motion.Cancel();

// Complete immediately
motion.Complete();
```

### Continuous Tracking (Manual Stop Required)
```csharp
// For continuous tracking that requires manual stopping
var continuousMotion = LMotion.Spring.Create(0f, 10f, SpringOptions.Critical)
    .WithLoops(-1, LoopType.Incremental)  // Infinite loops with incremental
    .Bind(value => transform.position.x = value);

// Manually stop when needed
continuousMotion.Cancel();
```

**Important Notes:**
- Use `LoopType.Incremental` with `loops = -1` for continuous tracking
- Spring animations require manual stopping - they don't auto-complete
- `MotionHandle.TotalDuration` is meaningless for spring animations (physics-based, not time-based)

### Loops
```csharp
// Loop with specific type
motion.WithLoops(3, LoopType.Yoyo);

// Available loop types:
// - LoopType.Restart
// - LoopType.Flip
// - LoopType.Incremental
// - LoopType.Yoyo
```

### Delays
```csharp
// Add delay
motion.WithDelay(1.0f, DelayType.FirstLoop);

// Available delay types:
// - DelayType.FirstLoop
// - DelayType.EveryLoop
```

### Callbacks
```csharp
// Animation callbacks
motion.WithOnComplete(() => Debug.Log("Completed"))
      .WithOnUpdate(value => Debug.Log($"Value: {value}"))
      .WithOnCancel(() => Debug.Log("Cancelled"));
```

### Time Scale
```csharp
// Respect time scale
motion.WithTimeScale(TimeScale.Unscaled);
```


## Performance Considerations

### Burst Compilation
Spring animations automatically use Burst compilation in build versions for maximum performance.

### SIMD Operations
Vector operations use Unity.Mathematics float4 for SIMD acceleration when possible.

### Memory Management
- **Zero GC Design**: LitMotion uses structs and value types throughout
- **SpringOptions are structs**: No GC allocation for spring configurations
- **MotionHandles are lightweight**: Lightweight references, not heavy objects
- **No Object Pooling Needed**: 0GC design eliminates the need for object pooling
- **High Performance**: Spring animations are actually faster than traditional tweens
- **No Concurrency Limits**: Performance tested to handle large numbers of concurrent animations

## Parameter Guidelines

### Stiffness Values
- **1-5**: Very slow, gentle motion
- **5-10**: Slow, smooth motion
- **10-15**: Balanced motion (recommended)
- **15-20**: Fast, snappy motion
- **20+**: Very fast, potentially unstable

### Damping Ratio Values
- **0.1-0.5**: Very bouncy, many oscillations
- **0.5-0.8**: Bouncy, few oscillations
- **0.8-1.0**: Slightly bouncy
- **1.0**: Critical damping (fastest convergence)
- **1.0-1.5**: Smooth, no oscillation
- **1.5+**: Very smooth, slow convergence

## Common Patterns

### UI Button Animation
```csharp
// Button press effect
var pressAnimation = LMotion.Spring.Create(1f, 0.9f, SpringOptions.Overdamped)
    .WithLoops(1, LoopType.Yoyo)
    .Bind(scale => button.transform.localScale = Vector3.one * scale);
```

### Smooth Camera Following
```csharp
// Camera follow with spring physics
var followAnimation = LMotion.Spring.Create(
    camera.transform.position, 
    targetPosition, 
    SpringOptions.Critical)
    .Bind(position => camera.transform.position = position);
```

### Color Transitions
```csharp
// Smooth color changes
var colorAnimation = LMotion.Spring.Create(
    currentColor, 
    targetColor, 
    SpringOptions.Underdamped)
    .Bind(color => material.color = color);
```

### Multi-property Animation
```csharp
// Animate multiple properties simultaneously
var positionSpring = LMotion.Spring.Create(Vector3.zero, targetPos, SpringOptions.Critical)
    .Bind(pos => transform.position = pos);

var scaleSpring = LMotion.Spring.Create(Vector3.one, targetScale, SpringOptions.Overdamped)
    .Bind(scale => transform.localScale = scale);
```

## Error Handling

### Common Issues

1. **Animation Not Completing**
   - Check if target value is reachable
   - Verify stiffness and damping ratio are reasonable
   - Ensure delta time is not zero
   - For continuous tracking, use `LoopType.Incremental` with `loops = -1`

2. **Performance Issues**
   - Use Burst compilation in build versions
   - Reuse SpringOptions instances
   - No object pooling needed (0GC design)
   - Spring animations are actually faster than traditional tweens
   - No concurrency limits - performance tested for large numbers of animations

3. **Unexpected Behavior**
   - Verify spring parameters are within recommended ranges
   - Check for conflicting animations
   - Ensure proper initialization
   - Remember that `MotionHandle.TotalDuration` is meaningless for spring animations

4. **Continuous Tracking Issues**
   - Use `LoopType.Incremental` with `loops = -1` for continuous tracking
   - Always manually stop continuous animations with `Cancel()`
   - Don't rely on auto-completion for continuous spring animations

## Integration with LitMotion

Spring animations integrate seamlessly with LitMotion's existing API:

- Same MotionBuilder interface
- Compatible with all LitMotion features (loops, delays, callbacks)
- Uses existing MotionHandle system
- Supports all LitMotion control methods

This ensures that existing LitMotion knowledge and patterns can be applied to spring animations with minimal learning curve.
