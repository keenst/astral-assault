#region
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace AstralAssault;

public static class ExtensionMethods
{
    internal static Vector2 RotateVector(this Vector2 inVector, float rotation) =>
        Vector2.Transform(inVector, Matrix.CreateRotationZ(rotation));

    public static int Mod(this int x, int y) => (Math.Abs(x * y) + x) % y;

    public static Tuple<TKey, TValue> ToTuple<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair) =>
        new Tuple<TKey, TValue>(keyValuePair.Key, keyValuePair.Value);
}