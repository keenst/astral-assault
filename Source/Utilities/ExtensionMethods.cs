#region
using System;
using Microsoft.Xna.Framework;
using TheGameOfDoomHmmm.Source.Graphics;
#endregion

namespace TheGameOfDoomHmmm;

internal static class ExtensionMethods
{
    private static readonly int LayerCount = Enum.GetValues(typeof(LayerOrdering)).Length;

    public static int Mod(this int x, int y) => (Math.Abs(x * y) + x) % y;

    public static Vector2 RotateVector(this Vector2 inVector, float rotation) =>
        Vector2.Transform(inVector, Matrix.CreateRotationZ(rotation));

    public static float GetDisplayLayerValue(this LayerOrdering layer)
    {
        int layerValue = (int)layer;

        // Scale the layer value from the range 1-layerCount to the range 0.0f-1.0f
        float scaledValue = (float)layerValue / (LayerCount - 1);

        return scaledValue;
    }
}