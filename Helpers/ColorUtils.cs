using System.Diagnostics.Contracts;

namespace SER.Helpers;

using UnityEngine;

public static class ColorUtils
{
    [Pure]
    public static Color AverageColor(Color[] colors)
    {
        if (colors.Length == 0)
            return Color.clear;

        float r = 0f, g = 0f, b = 0f, a = 0f;

        foreach (Color color in colors)
        {
            r += color.r;
            g += color.g;
            b += color.b;
            a += color.a;
        }

        float count = colors.Length;
        return new Color(r / count, g / count, b / count, a / count);
    }
}