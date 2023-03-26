using System;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public static class ExtensionMethods
{
    public static int Mod(this int x, int y)
    {
        return (Math.Abs(x * y) + x) % y;
    }

    public static Vector2 RotatedUnit(float angle)
    {
        float x = MathF.Cos(angle);
        float y = MathF.Sin(angle);
        return new(x, y);
    }
}