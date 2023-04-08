using System;

namespace AstralAssault;

public struct RandomSpriteProperty : IParticleProperty
{
    private readonly Random m_rnd = new Random();
    private readonly int m_rangeStart;
    private readonly int m_rangeEnd;

    public int SpriteIndex
    {
        get => m_rnd.Next(m_rangeStart, m_rangeEnd + 1);
    }

    public RandomSpriteProperty(int rangeStart, int rangeEnd)
    {
        m_rangeStart = rangeStart;
        m_rangeEnd = rangeEnd;
    }
}