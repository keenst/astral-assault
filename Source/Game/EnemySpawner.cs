using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public class EnemySpawner : IUpdateEventListener
{
    public int EnemiesKilled { get; set; }
    
    private readonly long _timeStarted;
    
    private readonly GameplayState _gameState;
    private readonly DebrisController _debrisController;

    private const float BaseSpawnInterval = 6000;
    private float _spawnInterval = BaseSpawnInterval;
    private long _lastSpawnTime;

    public EnemySpawner(GameplayState gameState)
    {
        _gameState = gameState;

        UpdateEventSource.UpdateEvent += OnUpdate;
        
        _debrisController = new DebrisController(gameState);
        
        _timeStarted = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    private void SpawnEnemy()
    {
        Random rnd = new();
        int side = rnd.Next(0, 4);

        int x = side switch
        {
            0 => 0,
            1 => Game1.TargetWidth,
            2 => rnd.Next(0, Game1.TargetWidth),
            3 => rnd.Next(0, Game1.TargetWidth),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        int y = side switch
        {
            0 => rnd.Next(0, Game1.TargetHeight),
            1 => rnd.Next(0, Game1.TargetHeight),
            2 => 0,
            3 => Game1.TargetHeight,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        Vector2 position = new(x, y);
        Asteroid.Sizes size = (Asteroid.Sizes)rnd.Next(0, 3);

        Vector2 gameCenter = new(Game1.TargetWidth / 2F, Game1.TargetHeight / 2F);
        float angleToCenter = MathF.Atan2(gameCenter.Y - position.Y, gameCenter.X - position.X);
        angleToCenter += MathHelper.ToRadians(rnd.Next(-45, 45));
        
        _gameState.Entities.Add(new Asteroid(_gameState, position, angleToCenter, size, _debrisController));
    }

    public List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = _debrisController.GetDrawTasks();
        
        
        
        return drawTasks;
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        if (timeNow - _lastSpawnTime < _spawnInterval) return;
        
        SpawnEnemy();

        long timeSinceStart = timeNow - _timeStarted;
        _spawnInterval = BaseSpawnInterval * MathF.Pow(0.999F, EnemiesKilled);
        
        _lastSpawnTime = timeNow;
    }

    public void StopListening()
    {
        UpdateEventSource.UpdateEvent -= OnUpdate;
    }
}