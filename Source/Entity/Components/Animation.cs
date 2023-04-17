namespace AstralAssault;

public struct Animation
{
    public Frame[] Frames { get; }
    public bool HasRotation { get; }
    public bool IsLooping { get; }

    public Animation(Frame[] frames, bool hasRotation, bool isLooping = false)
    {
        Frames = frames;
        HasRotation = hasRotation;
        IsLooping = isLooping;
    }
}