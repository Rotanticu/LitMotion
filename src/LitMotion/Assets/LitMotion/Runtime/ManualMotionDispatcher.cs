using System;
using System.Collections.Generic;

namespace LitMotion
{
    internal sealed class ManualMotionDispatcherScheduler : IMotionScheduler
    {
        readonly ManualMotionDispatcher dispatcher;

        public ManualMotionDispatcherScheduler(ManualMotionDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public MotionHandle Schedule<TValue, VValue, TOptions, TAnimationSpec>(ref MotionBuilder<TValue, VValue, TOptions, TAnimationSpec> builder)
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
        {
            return dispatcher.GetOrCreateRunner<TValue, VValue, TOptions, TAnimationSpec>().Storage.Create(ref builder);
        }
    }

    /// <summary>
    /// Manually updatable MotionDispatcher
    /// </summary>
    public sealed class ManualMotionDispatcher
    {
        /// <summary>
        /// Default ManualMotionDispatcher
        /// </summary>
        public static readonly ManualMotionDispatcher Default = new();

        /// <summary>
        /// MotionScheduler for scheduling to the dispatcher.
        /// </summary>
        public IMotionScheduler Scheduler => scheduler;

        readonly ManualMotionDispatcherScheduler scheduler;
        readonly Dictionary<Type, IUpdateRunner> runners = new();

        public ManualMotionDispatcher()
        {
            scheduler = new(this);
        }

        /// <summary>
        /// ManualMotionDispatcher time. It increases every time Update is called.
        /// </summary>
        public double Time
        {
            get => time;
            set
            {
                Update(value - time);
            }
        }

        double time;

        /// <summary>
        /// Ensures the storage capacity until it reaches at least capacity.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure</param>
        public void EnsureStorageCapacity<TValue, VValue, TOptions, TAnimationSpec>(int capacity)
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
        {

            GetOrCreateRunner<TValue, VValue, TOptions, TAnimationSpec>().Storage.EnsureCapacity(capacity);
        }

        /// <summary>
        /// Update all scheduled motions.
        /// </summary>
        /// <param name="deltaTime">Delta time</param>
        public void Update(double deltaTime)
        {
            time += deltaTime;

            foreach (var kv in runners)
            {
                kv.Value.Update(time, time, time);
            }
        }

        /// <summary>
        /// Cancel all motions and reset data.
        /// </summary>
        public void Reset()
        {
            foreach (var kv in runners)
            {
                kv.Value.Reset();
            }

            time = 0;
        }

        internal UpdateRunner<TValue, VValue, TOptions, TAnimationSpec> GetOrCreateRunner<TValue, VValue, TOptions, TAnimationSpec>()
            where TValue : unmanaged
            where VValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<VValue, TOptions>
        {
            var key = typeof((TValue, TOptions, TAnimationSpec));
            if (!runners.TryGetValue(key, out var runner))
            {
                var storage = new MotionStorage<TValue, VValue, TOptions, TAnimationSpec>(MotionManager.MotionTypeCount);
                MotionManager.Register(storage);

                runner = new UpdateRunner<TValue, VValue, TOptions, TAnimationSpec>(storage, 0, 0, 0);
                runners.Add(key, runner);
            }

            return (UpdateRunner<TValue, VValue, TOptions, TAnimationSpec>)runner;
        }
    }
}