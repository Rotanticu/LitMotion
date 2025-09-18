#if LITMOTION_SUPPORT_PHYSICS_2D

using UnityEngine;

namespace LitMotion.Extensions
{
    public static class LitMotionRigidbody2DExtensions
    {
        /// <summary>
        /// Create a motion data and bind it to Rigidbody2D.position
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="rigidbody2d">Target component</param>
        /// <param name="useMovePosition">Whether to use rigidbody2d.MovePosition()</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToPosition<TOptions, TAnimationSpec>(this MotionBuilder<Vector2, Vector2, TOptions, TAnimationSpec> builder, Rigidbody2D rigidbody2d, bool useMovePosition = true)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<Vector2, TOptions>
        {
            Error.IsNull(rigidbody2d);

            if (useMovePosition)
            {
                return builder.Bind(rigidbody2d, static (x, rigidbody2d) =>
                {
                    rigidbody2d.MovePosition(x);
                });
            }
            else
            {
                return builder.Bind(rigidbody2d, static (x, rigidbody2d) =>
                {
                    rigidbody2d.position = x;
                });
            }
        }

        /// <summary>
        /// Create a motion data and bind it to Rigidbody2D.position.x
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="rigidbody2d">Target component</param>
        /// <param name="useMovePosition">Whether to use rigidbody2d.MovePosition()</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToPositionX<TOptions, TAnimationSpec>(this MotionBuilder<float, float, TOptions, TAnimationSpec> builder, Rigidbody2D rigidbody2d, bool useMovePosition = true)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<float, TOptions>
        {
            Error.IsNull(rigidbody2d);

            if (useMovePosition)
            {
                return builder.Bind(rigidbody2d, static (x, rigidbody2d) =>
                {
                    var p = rigidbody2d.position;
                    p.x = x;
                    rigidbody2d.MovePosition(p);
                });
            }
            else
            {
                return builder.Bind(rigidbody2d, static (x, rigidbody2d) =>
                {
                    var p = rigidbody2d.position;
                    p.x = x;
                    rigidbody2d.position = p;
                });
            }
        }

        /// <summary>
        /// Create a motion data and bind it to Rigidbody2D.position.y
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="rigidbody2d">Target component</param>
        /// <param name="useMovePosition">Whether to use rigidbody2d.MovePosition()</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToPositionY<TOptions, TAnimationSpec>(this MotionBuilder<float, float, TOptions, TAnimationSpec> builder, Rigidbody2D rigidbody2d, bool useMovePosition = true)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<float, TOptions>
        {
            Error.IsNull(rigidbody2d);

            if (useMovePosition)
            {
                return builder.Bind(rigidbody2d, static (y, rigidbody2d) =>
                {
                    var p = rigidbody2d.position;
                    p.y = y;
                    rigidbody2d.MovePosition(p);
                });
            }
            else
            {
                return builder.Bind(rigidbody2d, static (y, rigidbody2d) =>
                {
                    var p = rigidbody2d.position;
                    p.y = y;
                    rigidbody2d.position = p;
                });
            }
        }

        /// <summary>
        /// Create a motion data and bind it to Rigidbody2D.rotation
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="rigidbody2d">Target component</param>
        /// <param name="useMovePosition">Whether to use rigidbody2d.MoveRotation()</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToRotation<TOptions, TAnimationSpec>(this MotionBuilder<float, float, TOptions, TAnimationSpec> builder, Rigidbody2D rigidbody2d, bool useMovePosition = true)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<float, TOptions>
        {
            Error.IsNull(rigidbody2d);

            if (useMovePosition)
            {
                return builder.Bind(rigidbody2d, static (x, rigidbody2d) =>
                {
                    rigidbody2d.MoveRotation(x);
                });
            }
            else
            {
                return builder.Bind(rigidbody2d, static (x, rigidbody2d) =>
                {
                    rigidbody2d.rotation = x;
                });
            }
        }
    }
}

#endif