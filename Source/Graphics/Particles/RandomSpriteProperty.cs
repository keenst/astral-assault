using System;

namespace AstralAssault;

public struct RandomSpriteProperty : IParticleProperty
{
    private readonly Random _rnd = new();
    private readonly int _rangeStart;
    private readonly int _rangeEnd;
    public int SpriteIndex => _rnd.Next(_rangeStart, _rangeEnd + 1);

    public RandomSpriteProperty(int rangeStart, int rangeEnd)
    {
        _rangeStart = rangeStart;
        _rangeEnd = rangeEnd;
    }
}