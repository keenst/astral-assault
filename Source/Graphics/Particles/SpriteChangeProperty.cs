namespace AstralAssault;

public struct SpriteChangeProperty : IParticleProperty
{
    public int StartIndex { get; }
    public int EndIndex { get; }
    public int TimeBetweenChangesMS { get; }

    public SpriteChangeProperty(int startIndex, int endIndex, int timeBetweenChangesMS)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
        TimeBetweenChangesMS = timeBetweenChangesMS;
    }
}