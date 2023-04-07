using System;

using Microsoft.Xna.Framework;

namespace AstralAssault;

public static class ExtensionMethods
{
    internal static int Mod(this int x, int y) => (Math.Abs(x * y) + x) % y;

    internal static Vector2 RotateVector(this Vector2 inVector, float rotation) =>
        Vector2.Transform(inVector, Matrix.CreateRotationZ(rotation));
}