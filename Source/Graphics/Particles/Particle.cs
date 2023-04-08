using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public class Particle
{
    private Vector2 _startingPosition;
    private Vector2 _velocity;
    public EffectContainer EffectContainer = new();

    public int TextureIndex { get; set; }
    public long TimeSpawned { get; private set; }
    public bool IsActive { get; private set; }
    public Vector2 Position => 
        _startingPosition + _velocity * (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - TimeSpawned);
    
    public Particle(int textureIndex, Vector2 startingPosition, Vector2 velocity, long timeSpawned)
    {
        TextureIndex = textureIndex;
        _startingPosition = startingPosition;
        _velocity = velocity;
        TimeSpawned = timeSpawned;
        IsActive = true;
    }

    public void Set(int textureIndex, Vector2 startingPosition, Vector2 velocity, long timeSpawned)
    {
        TextureIndex = textureIndex;
        _startingPosition = startingPosition;
        _velocity = velocity;
        TimeSpawned = timeSpawned;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}