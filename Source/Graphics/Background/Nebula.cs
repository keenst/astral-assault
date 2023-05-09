using Microsoft.Xna.Framework;

namespace AstralAssault.Background;

public struct Nebula
{
    public Point Position { get; }
    
    public Nebula(Point position)
    {
        Position = position;
    }
}