using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class ParticleEmitter
{
    private readonly Texture2D _spriteSheet;
    private readonly Rectangle[] _textureSources;
    private readonly int _particlesPerSecond;
    private readonly List<Particle> _particles = new();
    private readonly IParticleProperty[] _particleProperties;
    private readonly LayerDepth _layerDepth;
    private Vector2 _position;
    private float _rotation;
    private float TimeBetweenParticles => 1000F / _particlesPerSecond;
    private int _particlesSpawned;
    private int _particlesToSpawn;
    private long _lastTimeSpawned;
    private bool _isSpawning;

    public ParticleEmitter(
        Texture2D spriteSheet,
        Rectangle[] textureSources,
        int particlesPerSecond,
        Vector2 position,
        float rotation,
        IParticleProperty[] particleProperties,
        LayerDepth layerDepth)
    {
        _spriteSheet = spriteSheet;
        _textureSources = textureSources;
        _particlesPerSecond = particlesPerSecond;
        _position = position;
        _rotation = rotation;
        _particleProperties = particleProperties;
        _layerDepth = layerDepth;

        List<Type> particlePropertyTypes = new();

        foreach (IParticleProperty particleProperty in particleProperties)
        {
            Type particlePropertyType = particleProperty.GetType();

            if (particlePropertyTypes.Contains(particlePropertyType))
            {
                throw new ArgumentException();
            }
        }

        Type causeOfDeathPropertyType = typeof(CauseOfDeathProperty);

        if (!particleProperties.Any(p => causeOfDeathPropertyType.IsInstanceOfType(p)))
        {
            throw new ArgumentException();
        }
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if ((timeNow - _lastTimeSpawned > TimeBetweenParticles || _particlesPerSecond == 0) &&
            _isSpawning &&
            (_particlesSpawned < _particlesToSpawn || _particlesToSpawn == 0))
        {
            Vector2 velocity = Vector2.Zero;

            if (_particleProperties.OfType<VelocityProperty>().Any())
            {
                velocity = _particleProperties.OfType<VelocityProperty>().First().GetVelocity();
                velocity = Vector2.Transform(velocity, Matrix.CreateRotationZ(_rotation));
            }

            int textureIndex = _textureSources.Length - 1;

            if (_particleProperties.OfType<RandomSpriteProperty>().Any())
            {
                textureIndex = _particleProperties.OfType<RandomSpriteProperty>().First().SpriteIndex;
            }

            ActivateParticle(
                textureIndex,
                _position,
                velocity);

            _lastTimeSpawned = timeNow;
            _particlesSpawned++;
        }

        foreach (Particle particle in _particles.Where(particle => particle.IsActive))
        {
            HandleParticleProperties(particle);
        }
    }

    public void StartSpawning(int particlesToSpawn = 0)
    {
        _particlesToSpawn = particlesToSpawn;
        _particlesSpawned = 0;
        _isSpawning = true;
    }

    public void StopSpawning()
    {
        _isSpawning = false;
    }

    public void SetTransform(Vector2 position, float rotation)
    {
        _position = position;
        _rotation = rotation;
    }

    public void SetPosition(Vector2 position)
    {
        _position = position;
    }

    public List<DrawTask> CreateDrawTasks()
    {
        List<DrawTask> drawTasks = new();

        foreach (Particle particle in _particles.Where(p => p.IsActive))
        {
            drawTasks.Add(new(
                _spriteSheet,
                _textureSources[particle.TextureIndex],
                particle.Position,
                0,
                _layerDepth,
                particle.EffectContainer.Effects));
        }

        return drawTasks;
    }

    private void ActivateParticle(int textureIndex, Vector2 startingPosition, Vector2 velocity)
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (_particles.All(p => p.IsActive))
        {
            _particles.Add(new(textureIndex, startingPosition, velocity, timeNow));

            return;
        }

        int inactiveParticleIndex = _particles.FindIndex(p => !p.IsActive);
        _particles[inactiveParticleIndex].Set(textureIndex, startingPosition, velocity, timeNow);
    }

    private void HandleParticleProperties(Particle particle)
    {
        foreach (IParticleProperty particleProperty in _particleProperties)
        {
            switch (particleProperty)
            {
                case CauseOfDeathProperty causeOfDeathProperty:
                    ParticleEmitter.HandleCauseOfDeathProperty(particle, causeOfDeathProperty);

                    break;

                case ColorChangeProperty colorChangeProperty:
                    ParticleEmitter.HandleColorChangeProperty(particle, colorChangeProperty);

                    break;

                case SpriteChangeProperty spriteChangeProperty:
                    ParticleEmitter.HandleSpriteChangeProperty(particle, spriteChangeProperty);

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

                if (timeNow - particle.TimeSpawned > property.LifeSpan)
                {
                    particle.Deactivate();
                }

                break;
            }

            case CauseOfDeathProperty.CausesOfDeath.OutOfBounds:
            {
                if (particle.Position.X is < 0 or > Game1.TargetWidth ||
                    particle.Position.Y is < 0 or > Game1.TargetHeight)
                {
                    particle.Deactivate();
                }

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