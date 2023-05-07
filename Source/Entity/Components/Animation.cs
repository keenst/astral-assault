namespace AstralAssault;

public struct Animation
{
    internal Frame[] Frames { get; set; }
    internal bool HasRotation { get; set; }
    internal bool IsLooping { get; set; }

    internal Animation(Frame[] frames, bool hasRotation, bool isLooping = false)
    {
        Frames = frames;
        HasRotation = hasRotation;
        IsLooping = isLooping;
    }
}