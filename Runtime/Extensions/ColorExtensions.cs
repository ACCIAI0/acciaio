using UnityEngine;

namespace Acciaio
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Returns the given Color with the updated red value
        /// </summary>
        public static Color WithRed(this Color color, float r) => new(r, color.g, color.b, color.a);

        /// <summary>
        /// Returns the given Color with the updated green value
        /// </summary>
        public static Color WithGreen(this Color color, float g) => new(color.r, g, color.b, color.a);

        /// <summary>
        /// Returns the given Color with the updated blue value
        /// </summary>
        public static Color WithBlue(this Color color, float b) => new(color.r, color.g, b, color.a);

        /// <summary>
        /// Returns the given Color with the updated alpha value
        /// </summary>
        public static Color WithAlpha(this Color color, float a) => new(color.r, color.g, color.b, a);

        /// <summary>
        /// Returns the given Color with the updated hue value
        /// </summary>
        public static Color WithHue(this Color color, float h)
        {
            Color.RGBToHSV(color, out _, out var s, out var v);
            return Color.HSVToRGB(h, s, v);
        }

        /// <summary>
        /// Returns the given Color with the updated saturation value
        /// </summary>
        public static Color WithSaturation(this Color color, float s)
        {
            Color.RGBToHSV(color, out var h, out _, out var v);
            return Color.HSVToRGB(h, s, v);
        }

        /// <summary>
        /// Returns the given Color with the updated value v
        /// </summary>
        public static Color WithValue(this Color color, float v)
        {
            Color.RGBToHSV(color, out var h, out var s, out _);
            return Color.HSVToRGB(h, s, v);
        }
    }
}
