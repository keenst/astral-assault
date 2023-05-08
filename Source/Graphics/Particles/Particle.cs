using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public class Particle
{
    public readonly EffectContainer EffectContainer = new();

    public Vector2 Velocity { get; private set; }
    public int TextureIndex { get; set; }
    public float TimeAlive { get; set; }
    public bool IsActive { get; private set; }
    public Vector2 Position { get; set; }
    
    public Particle(int textureIndex, Vector2 position, Vector2 velocity)
    {
        TextureIndex = textureIndex;
        Position = position;
        Velocity = velocity;
        IsActive = true;
    }

    public void Set(int textureIndex, Vector2 position, Vector2 velocity)
    {
        TextureIndex = textureIndex;
        Position = position;
        Velocity = velocity;
        TimeAlive = 0;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}