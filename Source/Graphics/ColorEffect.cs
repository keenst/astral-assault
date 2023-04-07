using Vector4 = Microsoft.Xna.Framework.Vector4;

namespace AstralAssault;

public struct ColorEffect : IDrawTaskEffect
{
    public Vector4 Color { get; }

    public ColorEffect(Vector4 color)
    {
        Color = color;
    }
}