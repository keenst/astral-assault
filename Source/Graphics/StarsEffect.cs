using Microsoft.Xna.Framework;

namespace AstralAssault;

public struct StarsEffect : IDrawTaskEffect
{
    public readonly Vector2[] StarPositions;
    
    public StarsEffect(Vector2[] starPositions)
    {
        StarPositions = starPositions;
    }
}