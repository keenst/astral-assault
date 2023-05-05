#region
using System;
using AstralAssault.Source.Graphics;
using Microsoft.Xna.Framework;
#endregion

namespace AstralAssault;

public static class ExtensionMethods
{
    internal static Vector2 RotateVector(this Vector2 inVector, float rotation) =>
        Vector2.Transform(inVector, Matrix.CreateRotationZ(rotation));

    public static int Mod(this int x, int y) => (Math.Abs(x * y) + x) % y;

    public static float GetDisplayLayerValue(this LayerOrdering layer)
    {
        int layerCount = Enum.GetNames(typeof(LayerOrdering)).Length;
        int layerValue = (int)layer;

        // Scale the layer value from the range 1-layerCount to the range 0.0f-1.0f
        float scaledValue = (float)(layerValue - 1) / (layerCount - 1);

        return scaledValue;
    }


}