using Microsoft.Xna.Framework;

namespace AstralAssault;

public class StarsEffect : IDrawTaskEffect
{
    public readonly Vector2[] StarPositions;
    
    public StarsEffect(Vector2[] starPositions)
    {
        StarPositions = starPositions;
    }
}