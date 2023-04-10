#region
using Microsoft.Xna.Framework;
#endregion

namespace AstralAssault;

public struct ColorEffect : IDrawTaskEffect
{
    public Vector4 Color { get; }

    public ColorEffect(Vector4 color)
    {
        Color = color;
    }
}