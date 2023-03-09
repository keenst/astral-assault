namespace AstralAssault;

public struct Animation
{
    public Frame[] Frames { get; }
    public bool HasRotation { get; }

    public Animation(Frame[] frames, bool hasRotation)
    {
        Frames = frames;
        HasRotation = hasRotation;
    }
}