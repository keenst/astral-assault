using Microsoft.Xna.Framework;

namespace AstralAssault;

public class StarsEffect : IDrawTaskEffect
{
    public Vector2[] StarPositions;
    
    public StarsEffect(Vector2[] starPositions)
    {
        StarPositions = starPositions;
    }
}