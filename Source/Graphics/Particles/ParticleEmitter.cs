using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class ParticleEmitter
{
    private readonly Texture2D m_spriteSheet;
    private readonly Rectangle[] m_textureSources;
    private readonly int m_particlesPerSecond;
    private readonly List<Particle> m_particles = new List<Particle>();
    private readonly IParticleProperty[] m_particleProperties;
    private readonly LayerDepth m_layerDepth;
    private Vector2 m_position;
    private float m_rotation;

    private float TimeBetweenParticles
    {
        get => 1000F / m_particlesPerSecond;
    }

    private int m_particlesSpawned;
    private int m_particlesToSpawn;
    private long m_lastTimeSpawned;
    private bool m_isSpawning;

    public ParticleEmitter(
        Texture2D spriteSheet,
        Rectangle[] textureSources,
        int particlesPerSecond,
        Vector2 position,
        float rotation,
        IParticleProperty[] particleProperties,
        LayerDepth layerDepth)
    {
        m_spriteSheet = spriteSheet;
        m_textureSources = textureSources;
        m_particlesPerSecond = particlesPerSecond;
        m_position = position;
        m_rotation = rotation;
        m_particleProperties = particleProperties;
        m_layerDepth = layerDepth;

        List<Type> particlePropertyTypes = new List<Type>();

        foreach (IParticleProperty particleProperty in particleProperties)
        {
            Type particlePropertyType = particleProperty.GetType();

            if (particlePropertyTypes.Contains(particlePropertyType)) throw new ArgumentException();
        }

        Type causeOfDeathPropertyType = typeof(CauseOfDeathProperty);

        if (!particleProperties.Any(p => causeOfDeathPropertyType.IsInstanceOfType(p))) throw new ArgumentException();
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if ((((timeNow - m_lastTimeSpawned) > TimeBetweenParticles) || (m_particlesPerSecond == 0)) &&
            m_isSpawning &&
            ((m_particlesSpawned < m_particlesToSpawn) || (m_particlesToSpawn == 0)))
        {
            Vector2 velocity = Vector2.Zero;

            if (m_particleProperties.OfType<VelocityProperty>().Any())
            {
                velocity = m_particleProperties.OfType<VelocityProperty>().First().GetVelocity();
                velocity = Vector2.Transform(velocity, Matrix.CreateRotationZ(m_rotation));
            }

            int textureIndex = m_textureSources.Length - 1;

            if (m_particleProperties.OfType<RandomSpriteProperty>().Any()) textureIndex = m_particleProperties.OfType<RandomSpriteProperty>().First().SpriteIndex;

            ActivateParticle
            (
                textureIndex,
                m_position,
                velocity
            );

            m_lastTimeSpawned = timeNow;
            m_particlesSpawned++;
        }

        foreach (Particle particle in m_particles.Where(static particle => particle.IsActive)) HandleParticleProperties(particle);
    }

    public void StartSpawning(int particlesToSpawn = 0)
    {
        m_particlesToSpawn = particlesToSpawn;
        m_particlesSpawned = 0;
        m_isSpawning = true;
    }

    public void StopSpawning()
    {
        m_isSpawning = false;
    }

    public void SetTransform(Vector2 position, float rotation)
    {
        m_position = position;
        m_rotation = rotation;
    }

    public void SetPosition(Vector2 position)
    {
        m_position = position;
    }

    public List<DrawTask> CreateDrawTasks()
    {
        List<DrawTask> drawTasks = new List<DrawTask>();

        foreach (Particle particle in m_particles.Where(static p => p.IsActive))
        {
            drawTasks.Add
            (
                new DrawTask
                (
                    m_spriteSheet,
                    m_textureSources[particle.TextureIndex],
                    particle.Position,
                    0,
                    m_layerDepth,
                    particle.EffectContainer.Effects
                )
            );
        }

        return drawTasks;
    }

    private void ActivateParticle(int textureIndex, Vector2 startingPosition, Vector2 velocity)
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (m_particles.All(static p => p.IsActive))
        {
            m_particles.Add(new Particle(textureIndex, startingPosition, velocity, timeNow));

            return;
        }

        int inactiveParticleIndex = m_particles.FindIndex(static p => !p.IsActive);
        m_particles[inactiveParticleIndex].Set(textureIndex, startingPosition, velocity, timeNow);
    }

    private void HandleParticleProperties(Particle particle)
    {
        foreach (IParticleProperty particleProperty in m_particleProperties)
        {
            switch (particleProperty)
            {
            case CauseOfDeathProperty causeOfDeathProperty:
                HandleCauseOfDeathProperty(particle, causeOfDeathProperty);

                break;

            case ColorChangeProperty colorChangeProperty:
                HandleColorChangeProperty(particle, colorChangeProperty);

                break;

            case SpriteChangeProperty spriteChangeProperty:
                HandleSpriteChangeProperty(particle, spriteChangeProperty);

                break;
            }
        }
    }

    private static void HandleCauseOfDeathProperty(Particle particle, CauseOfDeathProperty property)
    {
        switch (property.CauseOfDeath)
        {
        case CauseOfDeathProperty.CausesOfDeath.LifeSpan:
        {
            long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            if ((timeNow - particle.TimeSpawned) > property.LifeSpan) particle.Deactivate();

            break;
        }

        case CauseOfDeathProperty.CausesOfDeath.OutOfBounds:
        {
            if (particle.Position.X is < 0 or > Game1.TargetWidth ||
                particle.Position.Y is < 0 or > Game1.TargetHeight) particle.Deactivate();

            break;
        }

        default:
            throw new ArgumentOutOfRangeException();
        }
    }

    private static void HandleColorChangeProperty(Particle particle, ColorChangeProperty property)
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        float timeSinceSpawned = timeNow - particle.TimeSpawned;

        int colorIndex = (int)(timeSinceSpawned / property.TimeBetweenColorsMS);

        if (colorIndex >= property.Colors.Length) return;

        particle.EffectContainer.SetEffect<ColorEffect, Vector4>(Palette.GetColorVector(property.Colors[colorIndex]));
    }

    private static void HandleSpriteChangeProperty(Particle particle, SpriteChangeProperty property)
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        float timeSinceSpawned = timeNow - particle.TimeSpawned;

        int spriteIndex = (int)(timeSinceSpawned / property.TimeBetweenChangesMS);

        if (spriteIndex >= property.EndIndex) return;

        particle.TextureIndex = spriteIndex;
    }
}