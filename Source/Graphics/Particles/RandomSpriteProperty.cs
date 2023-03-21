using System;

namespace AstralAssault;

public struct RandomSpriteProperty : IParticleProperty
{
    public int SpriteIndex { get; }
    
    public RandomSpriteProperty(int rangeStart, int rangeEnd)
    {
        Random rnd = new();
        SpriteIndex = rnd.Next(rangeStart, rangeEnd + 1);
    }
}