using UnityEngine;

namespace LitMotion.Extensions
{
    /// <summary>
    /// Provides binding extension methods for Material.
    /// </summary>
    public static class LitMotionMaterialExtensions
    {
        /// <summary>
        /// Create a motion data and bind it to a material property.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToMaterialFloat<TOptions, TAnimationSpec>(this MotionBuilder<float, float, TOptions, TAnimationSpec> builder, Material material, string name)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<float, TOptions>
        {
            Error.IsNull(material);
            return builder.Bind(material, name, static (x, material, name) =>
            {
                material.SetFloat(name, x);
            });
        }

        /// <summary>
        /// Create a motion data and bind it to a material property.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToMaterialFloat<TOptions, TAnimationSpec>(this MotionBuilder<float, float, TOptions, TAnimationSpec> builder, Material material, int nameID)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<float, TOptions>
        {
            Error.IsNull(material);
            return builder.Bind(material, Box.Create(nameID), static (x, material, nameID) =>
            {
                material.SetFloat(nameID.Value, x);
            });
        }

        /// <summary>
        /// Create a motion data and bind it to a material property.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToMaterialInt<TOptions, TAnimationSpec>(this MotionBuilder<int, int, TOptions, TAnimationSpec> builder, Material material, string name)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<int, TOptions>
        {
            Error.IsNull(material);
            return builder.Bind(material, name, static (x, material, name) =>
            {
                material.SetInteger(name, x);
            });
        }

        /// <summary>
        /// Create a motion data and bind it to a material property.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToMaterialInt<TOptions, TAnimationSpec>(this MotionBuilder<int, int, TOptions, TAnimationSpec> builder, Material material, int nameID)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<int, TOptions>
        {
            Error.IsNull(material);
            return builder.Bind(material, Box.Create(nameID), static (x, material, nameID) =>
            {
                material.SetInteger(nameID.Value, x);
            });
        }

        /// <summary>
        /// Create a motion data and bind it to a material property.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToMaterialColor<TOptions, TAnimationSpec>(this MotionBuilder<Color, Color, TOptions, TAnimationSpec> builder, Material material, string name)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<Color, TOptions>
        {
            Error.IsNull(material);
            return builder.Bind(material, name, static (x, material, name) =>
            {
                material.SetColor(name, x);
            });
        }

        /// <summary>
        /// Create a motion data and bind it to a material property.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToMaterialColor<TOptions, TAnimationSpec>(this MotionBuilder<Color, Color, TOptions, TAnimationSpec> builder, Material material, int nameID)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<Color, TOptions>
        {
            Error.IsNull(material);
            return builder.Bind(material, Box.Create(nameID), static (x, material, nameID) =>
            {
                material.SetColor(nameID.Value, x);
            });
        }
    }
}
