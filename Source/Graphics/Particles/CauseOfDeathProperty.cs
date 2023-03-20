namespace AstralAssault;

public struct CauseOfDeathProperty : IParticleProperty
{
    public enum CausesOfDeath
    {
        OutOfBounds,
        LifeSpan
    }
    
    public CausesOfDeath CauseOfDeath { get; }
    public int LifeSpan { get; }
    
    public CauseOfDeathProperty(CausesOfDeath causeOfDeath, int lifeSpan = 0)
    {
        CauseOfDeath = causeOfDeath;
        LifeSpan = lifeSpan;
    }
}