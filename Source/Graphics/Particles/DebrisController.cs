using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class DebrisController
{
    private readonly ParticleEmitter _particleEmitter;
    private readonly Random _rnd = new();
    private readonly GameplayState _gameplayState;
    
    public DebrisController(GameplayState gameplayState)
    {
        _gameplayState = gameplayState;
        
        Texture2D particleSpriteSheet = AssetManager.Load<Texture2D>("AsteroidDebris");
        
        Rectangle[] textureSources =
        {
            new(0,  0, 8, 8),
            new(8,  0, 8, 8),
            new(16, 0, 8, 8),
            new(32, 0, 8, 8)
        };

        IParticleProperty[] particleProperties =
        {
            new VelocityProperty(-1, 1, 0.05F, 0.15F),
            new RandomSpriteProperty(0, textureSources.Length - 1),
            new CauseOfDeathProperty(CauseOfDeathProperty.CausesOfDeath.OutOfBounds)
        };

        _particleEmitter = new ParticleEmitter(
            particleSpriteSheet,
            textureSources,
            0,
            Vector2.Zero,
            0,
            particleProperties,
            LayerDepth.Debris);
    }
    
    public void Update(float deltaTime)
    {
        _particleEmitter.Update(deltaTime);
    }
    
    public void SpawnDebris(Vector2 position, int asteroidSize)
    {
        int amount = _rnd.Next(4, (1 + asteroidSize) * 4);
        
        float angleFromPlayer = MathF.Atan2(
            position.Y - _gameplayState.Player.Position.Y,
            position.X - _gameplayState.Player.Position.X);

        _particleEmitter.SetTransform(position, angleFromPlayer);
        _particleEmitter.StartSpawning(amount);
    }
    
    public List<DrawTask> GetDrawTasks()
    {
        return _particleEmitter.CreateDrawTasks();
    }
}