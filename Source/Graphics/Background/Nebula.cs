using Microsoft.Xna.Framework;

namespace AstralAssault.Background;

public readonly struct Nebula
{
    public Point Position { get; }
    public Rectangle Rectangle => new(new Point(Position.X - 32, Position.Y - 32), new Point(64, 64));

    public Nebula(Point position)
    {
        Position = position;
    }
}