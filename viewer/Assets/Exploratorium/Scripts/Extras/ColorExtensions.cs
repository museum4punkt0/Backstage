using UnityEngine;

namespace Exploratorium.Extras
{
    public static class ColorExtensions
    {
        public static Color ContrastColor(this Color source)
        {
            if (source.grayscale > 0.5f)
                return Color.black;
            else
                return Color.white;
        }
    }
}