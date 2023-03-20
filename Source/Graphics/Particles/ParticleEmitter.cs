using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class ParticleEmitter : IUpdateEventListener
{
    private readonly Texture2D _spriteSheet;
    private readonly Rectangle[] _textureSources;
    private readonly int _particlesPerSecond;
    private readonly List<Particle> _particles = new();
    private readonly IParticleProperty[] _particleProperties;
    private Vector2 _position;
    private float _rotation;
    private bool _isSpawning;
    private float TimeBetweenParticles => 1000F / _particlesPerSecond;
    private int _particlesSpawned;
    private int _particlesToSpawn;
    private long _lastTimeSpawned;
    
    public ParticleEmitter(
        Texture2D spriteSheet, 
        Rectangle[] textureSources, 
        int particlesPerSecond, 
        Vector2 position,
        IParticleProperty[] particleProperties)
    {
        _spriteSheet = spriteSheet;
        _textureSources = textureSources;
        _particlesPerSecond = particlesPerSecond;
        _position = position;
        _particleProperties = particleProperties;

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
        
        UpdateEventSource.UpdateEvent += OnUpdate;
    }
    
    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (timeNow - _lastTimeSpawned > TimeBetweenParticles && 
            _isSpawning &&
            (_particlesSpawned < _particlesToSpawn || _particlesToSpawn == 0))
        {
            Vector2 velocity = Vector2.Zero;
            
            if (_particleProperties.OfType<VelocityProperty>().Any())
            {
                velocity = _particleProperties.OfType<VelocityProperty>().First().GetVelocity();
            }

            ActivateParticle(
                _textureSources.Length - 1,
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
    
    public void StopListening()
    {
        UpdateEventSource.UpdateEvent -= OnUpdate;
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

    public void SetTransform(Vector2 position)
    {
        _position = position;
    }

    public List<DrawTask> CreateDrawTasks()
    {
        Debug.WriteLine($"Total:  {_particles.Count}");
        Debug.WriteLine($"Active: {_particles.Count(p => p.IsActive)}");
        
        List<DrawTask> drawTasks = new();

        foreach (Particle particle in _particles.Where(p => p.IsActive))
        {
            drawTasks.Add(new DrawTask(
                _spriteSheet,
                _textureSources[particle.TextureIndex],
                particle.Position,
                0,
                LayerDepth.Foreground,
                particle.Effects));
        }
        
        return drawTasks;
    }

    private void ActivateParticle(int textureIndex, Vector2 startingPosition, Vector2 velocity)
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (_particles.All(p => p.IsActive))
        {
            _particles.Add(new Particle(textureIndex, startingPosition, velocity, timeNow));
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
                    HandleCauseOfDeathProperty(particle, causeOfDeathProperty); break;
                case ColorChangeProperty colorChangeProperty:
                    HandleColorChangeProperty(particle, colorChangeProperty); break;
                case SpriteChangeProperty spriteChangeProperty:
                    HandleSpriteChangeProperty(particle, spriteChangeProperty); break;
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
        
        particle.SetEffect<ColorEffect, Vector4>(Palette.GetColorVector(property.Colors[colorIndex]));
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