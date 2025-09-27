# LitMotion Spring Animation Documentation

## Overview

LitMotion Spring Animation is an extension to the LitMotion library that provides physics-based spring animations. Unlike traditional tween animations that use easing curves, spring animations simulate real-world spring physics to create more natural and responsive motion.

## Key Features

- **Physics-based Animation**: Uses spring physics simulation for natural motion
- **Multi-dimensional Support**: Works with float, Vector2, Vector3, and Vector4 types
- **High Performance**: Optimized with Burst compilation and SIMD operations
- **Velocity Preservation**: Maintains velocity information for smooth animation interruptions
- **Configurable Parameters**: Adjustable stiffness and damping ratio for different spring behaviors

## Spring Animation vs Traditional Tween

### Supported Features ✅

| Feature | Traditional Tween | Spring Animation |
|---------|-------------------|------------------|
| Duration | ✅ Fixed duration | ❌ Physics-based convergence |
| Easing | ✅ Ease curves (In, Out, InOut) | ❌ Physics-based motion |
| Loops | ✅ Loop support | ✅ Loop support |
| Delay | ✅ Delay support | ✅ Delay support |
| Callbacks | ✅ OnComplete, OnUpdate | ✅ OnComplete, OnUpdate |
| Time Scale | ✅ Time scale support | ✅ Time scale support |
| Pause/Resume | ✅ Pause/Resume | ✅ Pause/Resume |
| Cancel | ✅ Cancel support | ✅ Cancel support |

### Unsupported Features ❌

| Feature | Reason |
|---------|--------|
| **Fixed Duration** | Spring animations converge based on physics, not time |
| **Easing Curves** | Spring physics replace easing with natural motion |
| **Duration-based Completion** | Completion is based on convergence to target value |
| **Linear Interpolation** | Spring motion is inherently non-linear |
| **TotalDuration Property** | `MotionHandle.TotalDuration` is meaningless for physics-based animations |
| **Auto-completion** | Spring animations require manual stopping for continuous tracking |

## API Reference

### SpringOptions

```csharp
public struct SpringOptions : IMotionOptions
{
    public float4 CurrentValue;        // Current position
    public float4 CurrentVelocity;     // Current velocity
    public float4 TargetValue;         // Target position
    public float4 TargetVelocity;      // Target velocity
    public float Stiffness;            // Spring stiffness (higher = faster)
    public float DampingRatio;         // Damping ratio (1.0 = critical damping)
}
```

#### Predefined Spring Configurations

```csharp
// Critical damping - no oscillation, fastest convergence
SpringOptions.Critical

// Overdamped - slow convergence, no oscillation
SpringOptions.Overdamped

// Underdamped - oscillates before settling
SpringOptions.Underdamped
```

### Creating Spring Animations

#### Float Spring Animation

```csharp
// Basic spring animation
var motion = LMotion.Spring.Create(0f, 10f, SpringOptions.Critical)
    .Bind(value => transform.position.x = value);

// Custom spring parameters
var options = new SpringOptions(stiffness: 15f, dampingRatio: 0.8f);
var motion = LMotion.Spring.Create(0f, 10f, options)
    .Bind(value => transform.position.x = value);
```

#### Vector2 Spring Animation

```csharp
var motion = LMotion.Spring.Create(Vector2.zero, new Vector2(100f, 50f), SpringOptions.Critical)
    .Bind(value => transform.position = value);
```

#### Vector3 Spring Animation

```csharp
var motion = LMotion.Spring.Create(Vector3.zero, new Vector3(10f, 5f, 0f), SpringOptions.Critical)
    .Bind(value => transform.position = value);
```

#### Vector4 Spring Animation

```csharp
var motion = LMotion.Spring.Create(Vector4.zero, new Vector4(1f, 0.5f, 0.2f, 1f), SpringOptions.Critical)
    .Bind(value => material.color = value);
```

### Spring Parameters

#### Stiffness
- **Range**: 1.0f - 50.0f (recommended: 5.0f - 20.0f)
- **Effect**: Higher values make the spring "stiffer" and converge faster
- **Default**: 10.0f

#### Damping Ratio
- **< 1.0**: Underdamped (oscillates before settling)
- **= 1.0**: Critical damping (fastest convergence without oscillation)
- **> 1.0**: Overdamped (slow convergence, no oscillation)
- **Default**: 1.0f (critical damping)

### Animation Control

```csharp
// Start animation
var motion = LMotion.Spring.Create(0f, 10f, SpringOptions.Critical)
    .Bind(value => transform.position.x = value);

// Pause animation
motion.Pause();

// Resume animation
motion.Resume();

// Cancel animation
motion.Cancel();

// Complete animation immediately
motion.Complete();
```

### Continuous Tracking (Manual Stop Required)

For animations that need continuous tracking and manual stopping:

```csharp
// Continuous spring animation that requires manual stop
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

### Callbacks

```csharp
var motion = LMotion.Spring.Create(0f, 10f, SpringOptions.Critical)
    .WithOnComplete(() => Debug.Log("Spring animation completed"))
    .WithOnUpdate(value => Debug.Log($"Current value: {value}"))
    .Bind(value => transform.position.x = value);
```

### Loops and Delays

```csharp
var motion = LMotion.Spring.Create(0f, 10f, SpringOptions.Critical)
    .WithLoops(3, LoopType.Yoyo)  // Loop 3 times with yoyo effect
    .WithDelay(1.0f, DelayType.FirstLoop)  // 1 second delay before first loop
    .Bind(value => transform.position.x = value);
```

## Important Spring Animation Characteristics

### Duration and Completion
- **No Fixed Duration**: Spring animations are physics-based, not time-based
- **Convergence-based Completion**: Animations complete when they converge to the target value
- **Manual Stop Required**: For continuous tracking, use `LoopType.Incremental` with `loops = -1`
- **TotalDuration is Meaningless**: `MotionHandle.TotalDuration` has no meaning for spring animations

### Continuous Tracking Pattern
```csharp
// Correct pattern for continuous tracking
var continuousMotion = LMotion.Spring.Create(0f, 10f, SpringOptions.Critical)
    .WithLoops(-1, LoopType.Incremental)  // Infinite loops with incremental
    .Bind(value => transform.position.x = value);

// Must manually stop when done
continuousMotion.Cancel();
```

### Why These Limitations Exist
- **Physics-based**: Spring animations simulate real physics, not predetermined timing
- **Convergence-dependent**: Completion depends on reaching the target, not elapsed time
- **Continuous nature**: Some use cases require ongoing tracking that never "completes"

## Performance Considerations

### Burst Compilation
Spring animations are optimized with Burst compilation for maximum performance in build versions.

### SIMD Operations
Vector operations use Unity.Mathematics float4 for SIMD acceleration when possible.

### Zero GC Design
- **No Object Pooling Needed**: LitMotion uses 0GC design with structs and value types
- **Memory Efficient**: SpringOptions are structs, no garbage collection
- **Reusable**: MotionHandles are lightweight references, not heavy objects
- **High Performance**: Spring animations are actually faster than traditional tweens
- **No Concurrency Concerns**: Performance tested to handle large numbers of concurrent animations

### Performance Advantages
- **Faster than Tweens**: Performance testing shows Spring animations outperform traditional tweens
- **Scalable**: Can handle thousands of concurrent animations without performance degradation
- **Burst Optimized**: Automatic Burst compilation in build versions for maximum speed
- **SIMD Accelerated**: Vector operations use Unity.Mathematics for SIMD acceleration

## Migration from Traditional Tweens

### Before (Traditional Tween)
```csharp
// Traditional tween with easing
var tween = LMotion.Create(0f, 10f, 2f)
    .WithEase(Ease.OutQuad)
    .Bind(value => transform.position.x = value);
```

### After (Spring Animation)
```csharp
// Spring animation with physics
var spring = LMotion.Spring.Create(0f, 10f, SpringOptions.Critical)
    .Bind(value => transform.position.x = value);
```

### Key Differences
1. **No Duration**: Spring animations don't have fixed durations
2. **No Easing**: Physics replace easing curves
3. **Natural Motion**: More realistic animation behavior
4. **Velocity Preservation**: Smooth interruption handling

## Best Practices

### When to Use Spring Animations
- UI element positioning and scaling
- Camera movement and following
- Physics-based interactions
- Natural feeling transitions

### When to Use Traditional Tweens
- Precise timing requirements
- Linear or specific easing needs
- Simple value interpolation
- Legacy compatibility

### Parameter Tuning
- Start with `SpringOptions.Critical` for most use cases
- Use `SpringOptions.Underdamped` for bouncy effects
- Use `SpringOptions.Overdamped` for smooth, slow transitions
- Adjust stiffness for speed (higher = faster)
- Adjust damping ratio for oscillation behavior

## Examples

### UI Button Animation
```csharp
// Button press animation
var pressAnimation = LMotion.Spring.Create(1f, 0.9f, SpringOptions.Overdamped)
    .Bind(scale => button.transform.localScale = Vector3.one * scale);
```

### Camera Follow
```csharp
// Smooth camera following
var followAnimation = LMotion.Spring.Create(cameraPosition, targetPosition, SpringOptions.Critical)
    .Bind(position => camera.transform.position = position);
```

### Color Transition
```csharp
// Smooth color changes
var colorAnimation = LMotion.Spring.Create(currentColor, targetColor, SpringOptions.Underdamped)
    .Bind(color => material.color = color);
```

## Troubleshooting

### Animation Not Completing
- Check if target value is reachable
- Verify stiffness and damping ratio settings
- Ensure delta time is not zero

### Performance Issues
- Use Burst compilation in build versions
- Avoid creating too many simultaneous spring animations
- Consider using object pooling for frequent animations

### Unexpected Behavior
- Verify spring parameters are within reasonable ranges
- Check for conflicting animations on the same property
- Ensure proper initialization of spring options

## Conclusion

LitMotion Spring Animation provides a powerful alternative to traditional tween animations, offering more natural and physics-based motion. While it doesn't support all traditional tween features like fixed durations and easing curves, it provides unique benefits in terms of natural motion and velocity preservation that make it ideal for modern UI and game interactions.
