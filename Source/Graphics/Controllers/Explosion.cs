using System;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public readonly struct Explosion
{
    private readonly long _timeSpawned;
    public Point Position { get; }
    public long TimeSinceSpawned => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - _timeSpawned;
    
    public Explosion(Point position)
    {
        _timeSpawned = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        Position = position;
    }
}