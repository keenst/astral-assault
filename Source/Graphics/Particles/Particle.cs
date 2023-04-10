#region
using System;
using Microsoft.Xna.Framework;
#endregion

namespace AstralAssault;

public class Particle
{
    public EffectContainer EffectContainer = new EffectContainer();
    private Vector2 m_startingPosition;
    private Vector2 m_velocity;

    public Particle(int textureIndex, Vector2 startingPosition, Vector2 velocity, long timeSpawned)
    {
        TextureIndex = textureIndex;
        m_startingPosition = startingPosition;
        m_velocity = velocity;
        TimeSpawned = timeSpawned;
        IsActive = true;
    }

    public int TextureIndex { get; set; }
    public long TimeSpawned { get; private set; }
    public bool IsActive { get; private set; }

    public Vector2 Position
    {
        get => m_startingPosition + m_velocity * (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - TimeSpawned);
    }

    public void Set(int textureIndex, Vector2 startingPosition, Vector2 velocity, long timeSpawned)
    {
        TextureIndex = textureIndex;
        m_startingPosition = startingPosition;
        m_velocity = velocity;
        TimeSpawned = timeSpawned;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}