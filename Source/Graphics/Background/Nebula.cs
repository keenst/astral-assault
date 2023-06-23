using Microsoft.Xna.Framework;

namespace AstralAssault.Background;

public readonly struct Nebula
{
    public Vector3 Position { get; }
    public Rectangle Rectangle => new(new Point((int)Position.X - 32, (int)Position.Y - 32), new Point(64, 64));

    public Nebula(Vector3 position)
    {
        Position = position;
    }
}