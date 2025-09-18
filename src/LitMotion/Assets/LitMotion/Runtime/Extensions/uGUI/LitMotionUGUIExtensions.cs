#if LITMOTION_SUPPORT_UGUI
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
#if LITMOTION_SUPPORT_ZSTRING
using Cysharp.Text;
#endif

namespace LitMotion.Extensions
{
    /// <summary>
    /// Provides binding extension methods for Unity UI (uGUI) components.
    /// </summary>
    public static class LitMotionUGUIExtensions
    {
        /// <summary>
        /// Create a motion data and bind it to Graphic.color
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToColor<TOptions, TAnimationSpec>(this MotionBuilder<Color, Color, TOptions, TAnimationSpec> builder, Graphic graphic)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<Color, TOptions>
        {
            Error.IsNull(graphic);
            return builder.Bind(graphic, static (x, target) =>
            {
                target.color = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Graphic.color.r
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToColorR<TOptions, TAnimationSpec>(this MotionBuilder<float, float, TOptions, TAnimationSpec> builder, Graphic graphic)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<float, TOptions>
        {
            Error.IsNull(graphic);
            return builder.Bind(graphic, static (x, target) =>
            {
                var c = target.color;
                c.r = x;
                target.color = c;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Graphic.color.g
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToColorG<TOptions, TAnimationSpec>(this MotionBuilder<float, float, TOptions, TAnimationSpec> builder, Graphic graphic)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<float, TOptions>
        {
            Error.IsNull(graphic);
            return builder.Bind(graphic, static (x, target) =>
            {
                var c = target.color;
                c.g = x;
                target.color = c;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Graphic.color.b
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToColorB<TOptions, TAnimationSpec>(this MotionBuilder<float, float, TOptions, TAnimationSpec> builder, Graphic graphic)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<float, TOptions>
        {
            Error.IsNull(graphic);
            return builder.Bind(graphic, static (x, target) =>
            {
                var c = target.color;
                c.b = x;
                target.color = c;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Graphic.color.a
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToColorA<TOptions, TAnimationSpec>(this MotionBuilder<float, float, TOptions, TAnimationSpec> builder, Graphic graphic)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<float, TOptions>
        {
            Error.IsNull(graphic);
            return builder.Bind(graphic, static (x, target) =>
            {
                var c = target.color;
                c.a = x;
                target.color = c;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Image.FillAmount
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToFillAmount<TOptions, TAnimationSpec>(this MotionBuilder<float, float, TOptions, TAnimationSpec> builder, Image image)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<float, TOptions>
        {
            Error.IsNull(image);
            return builder.Bind(image, static (x, target) =>
            {
                target.fillAmount = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to CanvasGroup.alpha
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToAlpha<TOptions, TAnimationSpec>(this MotionBuilder<float, float, TOptions, TAnimationSpec> builder, CanvasGroup canvasGroup)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<float, TOptions>
        {
            Error.IsNull(canvasGroup);
            return builder.Bind(canvasGroup, static (x, target) =>
            {
                target.alpha = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Text.fontSize
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToFontSize<TOptions, TAnimationSpec>(this MotionBuilder<int, int, TOptions, TAnimationSpec> builder, Text text)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<int, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.fontSize = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Text.text
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToText<TOptions, TAnimationSpec>(this MotionBuilder<FixedString32Bytes, FixedString32Bytes, TOptions, TAnimationSpec> builder, Text text)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<FixedString32Bytes, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.text = x.ConvertToString();
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Text.text
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToText<TOptions, TAnimationSpec>(this MotionBuilder<FixedString64Bytes, FixedString64Bytes, TOptions, TAnimationSpec> builder, Text text)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<FixedString64Bytes, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.text = x.ConvertToString();
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Text.text
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToText<TOptions, TAnimationSpec>(this MotionBuilder<FixedString128Bytes, FixedString128Bytes, TOptions, TAnimationSpec> builder, Text text)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<FixedString128Bytes, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.text = x.ConvertToString();
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Text.text
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToText<TOptions, TAnimationSpec>(this MotionBuilder<FixedString512Bytes, FixedString512Bytes, TOptions, TAnimationSpec> builder, Text text)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<FixedString512Bytes, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.text = x.ConvertToString();
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Text.text
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToText<TOptions, TAnimationSpec>(this MotionBuilder<FixedString4096Bytes, FixedString4096Bytes, TOptions, TAnimationSpec> builder, Text text)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<FixedString4096Bytes, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.text = x.ConvertToString();
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Text.text
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToText<TOptions, TAnimationSpec>(this MotionBuilder<int, int, TOptions, TAnimationSpec> builder, Text text)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<int, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.text = x.ToString();
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Text.text
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <param name="format">Format string</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToText<TOptions, TAnimationSpec>(this MotionBuilder<int, int, TOptions, TAnimationSpec> builder, Text text, string format)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<int, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, format, static (x, text, format) =>
            {
#if LITMOTION_SUPPORT_ZSTRING
                text.text = ZString.Format(format, x);
#else
                text.text = string.Format(format, x);
#endif
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Text.text
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToText<TOptions, TAnimationSpec>(this MotionBuilder<long, long, TOptions, TAnimationSpec> builder, Text text)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<long, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.text = x.ToString();
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Text.text
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <param name="format">Format string</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToText<TOptions, TAnimationSpec>(this MotionBuilder<long, long, TOptions, TAnimationSpec> builder, Text text, string format)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<long, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, format, static (x, text, format) =>
            {
#if LITMOTION_SUPPORT_ZSTRING
                text.text = ZString.Format(format, x);
#else
                text.text = string.Format(format, x);
#endif
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Text.text
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToText<TOptions, TAnimationSpec>(this MotionBuilder<float, float, TOptions, TAnimationSpec> builder, Text text)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<float, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.text = x.ToString();
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Text.text
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <param name="format">Format string</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToText<TOptions, TAnimationSpec>(this MotionBuilder<float, float, TOptions, TAnimationSpec> builder, Text text, string format)
            where TOptions : unmanaged, ITweenOptions
            where TAnimationSpec : unmanaged, IVectorizedAnimationSpec<float, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, format, static (x, text, format) =>
            {
#if LITMOTION_SUPPORT_ZSTRING
                text.text = ZString.Format(format, x);
#else
                text.text = string.Format(format, x);
#endif
            });
        }
    }
}
#endif
