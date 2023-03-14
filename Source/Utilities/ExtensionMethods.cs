using System;
using System.Drawing;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public static class ExtensionMethods
{
    public static int Mod(this int x, int y)
    {
        return (Math.Abs(x * y) + x) % y;
    }

    public static PointF ToPointF(this Vector2 vector)
    {
        return new PointF(vector.X, vector.Y);
    }
}