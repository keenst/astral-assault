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
    private readonly List<Tuple<long, Tuple<Vector2, int>>> _scores = new(); // (timeSpawned, (coords, score))
    
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
    
    public void SpawnDebris(Vector2 position, int asteroidSize)
    {
        int amount = _rnd.Next(4, (1 + asteroidSize) * 4);
        
        float angleFromPlayer = MathF.Atan2(
            position.Y - _gameplayState.Player.Position.Y,
            position.X - _gameplayState.Player.Position.X);

        _particleEmitter.SetTransform(position, angleFromPlayer);
        _particleEmitter.StartSpawning(amount);
        
        int score = (Asteroid.Sizes)asteroidSize switch
        {
            Asteroid.Sizes.Smallest => 100,
            Asteroid.Sizes.Small => 300,
            Asteroid.Sizes.Medium => 700,
            _ => 0
        };

        _scores.Add(new Tuple<long, Tuple<Vector2, int>>(
            DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond,
            new Tuple<Vector2, int>(position, score)));
    }
    
    public List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = _particleEmitter.CreateDrawTasks();

        if (_scores.Count == 0) return drawTasks;
        
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        for (int i = 0; i < _scores.Count; i++)
        {
            if (timeNow - _scores[i].Item1 > 1000)
            {
                _scores.RemoveAt(i);
                i--;
                continue;
            }
            
            string scoreText = $"{_scores[i].Item2.Item2}";
            int scoreX = (int)_scores[i].Item2.Item1.X - scoreText.Length * 4;
            Vector2 scorePosition = new(scoreX, _scores[i].Item2.Item1.Y - 5);
            
            drawTasks.AddRange(scoreText.CreateDrawTasks(
                scorePosition,
                Palette.GetColor(Palette.Colors.Grey9),
                LayerDepth.Background));
        }

        return drawTasks;
    }
}