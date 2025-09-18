using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using LitMotion.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LitMotion
{
    /// <summary>
    /// Motion dispatcher.
    /// </summary>
    public static class MotionDispatcher
    {
        static class StorageCache<TValue, VValue, TOptions, TAnimationSpec>
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
        {
            public static MotionStorage<TValue, VValue, TOptions, TAnimationSpec> initialization;
            public static MotionStorage<TValue, VValue, TOptions, TAnimationSpec> earlyUpdate;
            public static MotionStorage<TValue, VValue, TOptions, TAnimationSpec> fixedUpdate;
            public static MotionStorage<TValue, VValue, TOptions, TAnimationSpec> preUpdate;
            public static MotionStorage<TValue, VValue, TOptions, TAnimationSpec> update;
            public static MotionStorage<TValue, VValue, TOptions, TAnimationSpec> preLateUpdate;
            public static MotionStorage<TValue, VValue, TOptions, TAnimationSpec> postLateUpdate;
            public static MotionStorage<TValue, VValue, TOptions, TAnimationSpec> timeUpdate;

            public static MotionStorage<TValue, VValue, TOptions, TAnimationSpec> GetOrCreate(PlayerLoopTiming playerLoopTiming)
            {
                return playerLoopTiming switch
                {
                    PlayerLoopTiming.Initialization => CreateIfNull(ref initialization),
                    PlayerLoopTiming.EarlyUpdate => CreateIfNull(ref earlyUpdate),
                    PlayerLoopTiming.FixedUpdate => CreateIfNull(ref fixedUpdate),
                    PlayerLoopTiming.PreUpdate => CreateIfNull(ref preUpdate),
                    PlayerLoopTiming.Update => CreateIfNull(ref update),
                    PlayerLoopTiming.PreLateUpdate => CreateIfNull(ref preLateUpdate),
                    PlayerLoopTiming.PostLateUpdate => CreateIfNull(ref postLateUpdate),
                    PlayerLoopTiming.TimeUpdate => CreateIfNull(ref timeUpdate),
                    _ => null,
                };
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static MotionStorage<TValue, VValue, TOptions, TAnimationSpec> CreateIfNull(ref MotionStorage<TValue, VValue, TOptions, TAnimationSpec> storage)
            {
                if (storage == null)
                {
                    storage = new MotionStorage<TValue, VValue, TOptions, TAnimationSpec>(MotionManager.MotionTypeCount);
                    MotionManager.Register(storage);
                }
                return storage;
            }
        }

        static class RunnerCache<TValue, VValue, TOptions, TAnimationSpec>
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
        {
            public static UpdateRunner<TValue, VValue, TOptions, TAnimationSpec> initialization;
            public static UpdateRunner<TValue, VValue, TOptions, TAnimationSpec> earlyUpdate;
            public static UpdateRunner<TValue, VValue, TOptions, TAnimationSpec> fixedUpdate;
            public static UpdateRunner<TValue, VValue, TOptions, TAnimationSpec> preUpdate;
            public static UpdateRunner<TValue, VValue, TOptions, TAnimationSpec> update;
            public static UpdateRunner<TValue, VValue, TOptions, TAnimationSpec> preLateUpdate;
            public static UpdateRunner<TValue, VValue, TOptions, TAnimationSpec> postLateUpdate;
            public static UpdateRunner<TValue, VValue, TOptions, TAnimationSpec> timeUpdate;

            public static (UpdateRunner<TValue, VValue, TOptions, TAnimationSpec> runner, bool isCreated) GetOrCreate(PlayerLoopTiming playerLoopTiming, MotionStorage<TValue, VValue, TOptions, TAnimationSpec> storage)
            {
                return playerLoopTiming switch
                {
                    PlayerLoopTiming.Initialization => CreateIfNull(playerLoopTiming, ref initialization, storage),
                    PlayerLoopTiming.EarlyUpdate => CreateIfNull(playerLoopTiming, ref earlyUpdate, storage),
                    PlayerLoopTiming.FixedUpdate => CreateIfNull(playerLoopTiming, ref fixedUpdate, storage),
                    PlayerLoopTiming.PreUpdate => CreateIfNull(playerLoopTiming, ref preUpdate, storage),
                    PlayerLoopTiming.Update => CreateIfNull(playerLoopTiming, ref update, storage),
                    PlayerLoopTiming.PreLateUpdate => CreateIfNull(playerLoopTiming, ref preLateUpdate, storage),
                    PlayerLoopTiming.PostLateUpdate => CreateIfNull(playerLoopTiming, ref postLateUpdate, storage),
                    PlayerLoopTiming.TimeUpdate => CreateIfNull(playerLoopTiming, ref timeUpdate, storage),
                    _ => default,
                };
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static (UpdateRunner<TValue, VValue, TOptions, TAnimationSpec>, bool) CreateIfNull(PlayerLoopTiming playerLoopTiming, ref UpdateRunner<TValue, VValue, TOptions, TAnimationSpec> runner, MotionStorage<TValue, VValue, TOptions, TAnimationSpec> storage)
            {
                if (runner == null)
                {
                    if (playerLoopTiming == PlayerLoopTiming.FixedUpdate)
                    {
                        runner = new UpdateRunner<TValue, VValue, TOptions, TAnimationSpec>(storage, Time.fixedTimeAsDouble, Time.fixedUnscaledTimeAsDouble, Time.realtimeSinceStartupAsDouble);
                    }
                    else
                    {
                        runner = new UpdateRunner<TValue, VValue, TOptions, TAnimationSpec>(storage, Time.timeAsDouble, Time.unscaledTimeAsDouble, Time.realtimeSinceStartupAsDouble);
                    }
                    GetRunnerList(playerLoopTiming).Add(runner);
                    return (runner, true);
                }
                return (runner, false);
            }
        }

        static FastListCore<IUpdateRunner> initializationRunners;
        static FastListCore<IUpdateRunner> earlyUpdateRunners;
        static FastListCore<IUpdateRunner> fixedUpdateRunners;
        static FastListCore<IUpdateRunner> preUpdateRunners;
        static FastListCore<IUpdateRunner> updateRunners;
        static FastListCore<IUpdateRunner> preLateUpdateRunners;
        static FastListCore<IUpdateRunner> postLateUpdateRunners;
        static FastListCore<IUpdateRunner> timeUpdateRunners;

        internal static FastListCore<IUpdateRunner> EmptyList = FastListCore<IUpdateRunner>.Empty;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ref FastListCore<IUpdateRunner> GetRunnerList(PlayerLoopTiming playerLoopTiming)
        {
            // FastListCore<T> must be passed as ref
            switch (playerLoopTiming)
            {
                default:
                    return ref EmptyList;
                case PlayerLoopTiming.Initialization:
                    return ref initializationRunners;
                case PlayerLoopTiming.EarlyUpdate:
                    return ref earlyUpdateRunners;
                case PlayerLoopTiming.FixedUpdate:
                    return ref fixedUpdateRunners;
                case PlayerLoopTiming.PreUpdate:
                    return ref preUpdateRunners;
                case PlayerLoopTiming.Update:
                    return ref updateRunners;
                case PlayerLoopTiming.PreLateUpdate:
                    return ref preLateUpdateRunners;
                case PlayerLoopTiming.PostLateUpdate:
                    return ref postLateUpdateRunners;
                case PlayerLoopTiming.TimeUpdate:
                    return ref timeUpdateRunners;
            };
        }

        static Action<Exception> unhandledException = DefaultUnhandledExceptionHandler;
        static readonly PlayerLoopTiming[] playerLoopTimings = (PlayerLoopTiming[])Enum.GetValues(typeof(PlayerLoopTiming));

        static Unity.Collections.AllocatorHelper<Unity.Collections.RewindableAllocator> allocator;

        static bool isCreatedAllocator = false;

        public static Unity.Collections.AllocatorHelper<Unity.Collections.RewindableAllocator> Allocator
        {
            get
            {
                if (!isCreatedAllocator)
                {
                    allocator = RewindableAllocatorFactory.CreateAllocator();
                    isCreatedAllocator = true;
                }
                return allocator;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            Clear();
        }

        /// <summary>
        /// Set handling of unhandled exceptions.
        /// </summary>
        public static void RegisterUnhandledExceptionHandler(Action<Exception> unhandledExceptionHandler)
        {
            unhandledException = unhandledExceptionHandler;
        }

        /// <summary>
        /// Get handling of unhandled exceptions.
        /// </summary>
        public static Action<Exception> GetUnhandledExceptionHandler()
        {
            return unhandledException;
        }

        static void DefaultUnhandledExceptionHandler(Exception exception)
        {
            Debug.LogException(exception);
        }

        /// <summary>
        /// Cancel all motions.
        /// </summary>
        public static void Clear()
        {
            foreach (var playerLoopTiming in playerLoopTimings)
            {
                var span = GetRunnerList(playerLoopTiming).AsSpan();
                for (int i = 0; i < span.Length; i++)
                {
                    span[i].Reset();
                }
            }
            if (isCreatedAllocator)
            {
                allocator.Allocator.Rewind();
                isCreatedAllocator = false;
            }
        }

        /// <summary>
        /// Ensures the storage capacity until it reaches at least `capacity`.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure.</param>
        public static void EnsureStorageCapacity<TValue, VValue, TOptions, TAnimationSpec>(int capacity)
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
        {
            foreach (var playerLoopTiming in playerLoopTimings)
            {
                StorageCache<TValue, VValue, TOptions, TAnimationSpec>.GetOrCreate(playerLoopTiming).EnsureCapacity(capacity);
            }
#if UNITY_EDITOR
            EditorMotionDispatcher.EnsureStorageCapacity<TValue, VValue, TOptions, TAnimationSpec>(capacity);
#endif
        }

        internal static MotionHandle Schedule<TValue, VValue, TOptions, TAnimationSpec>(ref MotionBuilder<TValue, VValue, TOptions, TAnimationSpec> builder, PlayerLoopTiming playerLoopTiming)
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
        {
            var storage = StorageCache<TValue, VValue, TOptions, TAnimationSpec>.GetOrCreate(playerLoopTiming);
            RunnerCache<TValue, VValue, TOptions, TAnimationSpec>.GetOrCreate(playerLoopTiming, storage);
            return storage.Create(ref builder);
        }

        internal static void Update(PlayerLoopTiming playerLoopTiming)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying) return;
#endif
            var span = GetRunnerList(playerLoopTiming).AsSpan();
            if (playerLoopTiming == PlayerLoopTiming.FixedUpdate)
            {
                for (int i = 0; i < span.Length; i++) span[i].Update(Time.fixedTimeAsDouble, Time.fixedUnscaledTimeAsDouble, Time.realtimeSinceStartupAsDouble);
            }
            else
            {
                for (int i = 0; i < span.Length; i++) span[i].Update(Time.timeAsDouble, Time.unscaledTimeAsDouble, Time.realtimeSinceStartupAsDouble);
            }
        }
    }

#if UNITY_EDITOR
    internal static class EditorMotionDispatcher
    {
        static class Cache<TValue, VValue, TOptions, TAnimationSpec>
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
        {
            static MotionStorage<TValue, VValue, TOptions, TAnimationSpec> storage;
            static UpdateRunner<TValue, VValue, TOptions, TAnimationSpec> updateRunner;

            public static MotionStorage<TValue, VValue, TOptions, TAnimationSpec> GetOrCreateStorage()
            {
                if (storage == null)
                {
                    storage = new MotionStorage<TValue, VValue, TOptions, TAnimationSpec>(MotionManager.MotionTypeCount);
                    MotionManager.Register(storage);
                }
                return storage;
            }

            public static void InitUpdateRunner()
            {
                if (updateRunner == null)
                {
                    var time = EditorApplication.timeSinceStartup;
                    updateRunner = new UpdateRunner<TValue, VValue, TOptions, TAnimationSpec>(storage, time, time, time);
                    updateRunners.Add(updateRunner);
                }
            }
        }

        static FastListCore<IUpdateRunner> updateRunners;

        public static MotionHandle Schedule<TValue, VValue, TOptions, TAnimationSpec>(ref MotionBuilder<TValue, VValue, TOptions, TAnimationSpec> builder)
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
        {
            var storage = Cache<TValue, VValue, TOptions, TAnimationSpec>.GetOrCreateStorage();
            Cache<TValue, VValue, TOptions, TAnimationSpec>.InitUpdateRunner();
            return storage.Create(ref builder);
        }

        public static void EnsureStorageCapacity<TValue, VValue, TOptions, TAnimationSpec>(int capacity)
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
        {
            Cache<TValue, VValue, TOptions, TAnimationSpec>.GetOrCreateStorage().EnsureCapacity(capacity);
        }

        [InitializeOnLoadMethod]
        static void Init()
        {
            EditorApplication.update += Update;
        }

        static void Update()
        {
            var span = updateRunners.AsSpan();
            for (int i = 0; i < span.Length; i++)
            {
                span[i].Update(EditorApplication.timeSinceStartup, EditorApplication.timeSinceStartup, Time.realtimeSinceStartupAsDouble);
            }
        }
    }
#endif
}