namespace TheGameOfDoomHmmm.Source.Entity.Components;

public struct Animation
{
    internal Frame[] Frames { get; set; }
    internal bool HasRotation { get; init; }
    internal bool IsLooping { get; init; }

    internal Animation(Frame[] frames, bool hasRotation, bool isLooping = false)
    {
        Frames = frames;
        HasRotation = hasRotation;
        IsLooping = isLooping;
    }
}