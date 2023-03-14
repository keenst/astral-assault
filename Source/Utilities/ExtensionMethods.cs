using System;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public static class ExtensionMethods
{
    public static int Mod(this int x, int y)
    {
        return (Math.Abs(x * y) + x) % y;
    }

    public static Vector2 Normalized(this Vector2 a)
    {
        Vector2 normalized = a;
        normalized.Normalize();
        return normalized;
    }
}