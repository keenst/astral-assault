namespace AstralAssault;

public struct Animation
{
    public Frame[] Frames { get; set; }
    public bool HasRotation { get; set; }
    public bool IsLooping { get; set; }

    public Animation(Frame[] frames, bool hasRotation, bool isLooping = false)
    {
        Frames = frames;
        HasRotation = hasRotation;
        IsLooping = isLooping;
    }
}