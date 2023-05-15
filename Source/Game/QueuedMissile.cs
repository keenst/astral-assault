using Microsoft.Xna.Framework;

namespace AstralAssault;

public struct QueuedMissile
{
    public long TimeToLaunchMS { get; }
    public Vector2 Position { get; }
    
    public QueuedMissile(long timeToLaunchMS, Vector2 position)
    {
        TimeToLaunchMS = timeToLaunchMS;
        Position = position;
    }
}