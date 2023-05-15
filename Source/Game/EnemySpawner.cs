using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public class EnemySpawner : IUpdateEventListener
{
    public int EnemiesKilled { get; set; }

    private readonly GameplayState _gameState;
    private readonly DebrisController _debrisController;

    private const float BaseAsteroidSpawnInterval = 24000;
    private const float BaseMissileSpawnInterval = 36000;
    private float _asteroidSpawnInterval = BaseAsteroidSpawnInterval;
    private float _missileSpawnInterval = BaseMissileSpawnInterval;
    private long _lastAsteroidSpawnTime;
    private long _lastMissileSpawnTime;

    public EnemySpawner(GameplayState gameState)
    {
        _gameState = gameState;

        UpdateEventSource.UpdateEvent += OnUpdate;
        
        _debrisController = new DebrisController(gameState);
    }

    private void SpawnAsteroid()
    {
        Random rnd = new();
        
        Vector2 position = GenerateEnemyPosition();
        Asteroid.Sizes size = (Asteroid.Sizes)rnd.Next(0, 3);

        Vector2 gameCenter = new(Game1.TargetWidth / 2F, Game1.TargetHeight / 2F);
        float angleToCenter = MathF.Atan2(gameCenter.Y - position.Y, gameCenter.X - position.X);
        angleToCenter += MathHelper.ToRadians(rnd.Next(-45, 45));
        
        _gameState.Entities.Add(new Asteroid(_gameState, position, angleToCenter, size, _debrisController));
    }

    private void SpawnMissile()
    {
        Vector2 position = GenerateEnemyPosition();
        
        _gameState.Entities.Add(new Missile(_gameState, position));
    }

    private static Vector2 GenerateEnemyPosition()
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
        
        return new Vector2(x, y);
    }

    public List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = _debrisController.GetDrawTasks();
        
        
        
        return drawTasks;
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        
        if (timeNow - _lastAsteroidSpawnTime > _asteroidSpawnInterval)
        {
            _lastAsteroidSpawnTime = timeNow;
            SpawnAsteroid();
        }

        if (timeNow - _lastMissileSpawnTime > _missileSpawnInterval)
        {
            _lastMissileSpawnTime = timeNow;

            int amountToSpawn = (int)MathF.Pow(1.2F, EnemiesKilled / 5F);

            for (int i = 0; i < amountToSpawn; i++)
            {
                SpawnMissile();   
            }
        }

        _asteroidSpawnInterval = BaseAsteroidSpawnInterval * MathF.Pow(0.98F, EnemiesKilled);
        _missileSpawnInterval = BaseMissileSpawnInterval * MathF.Pow(0.98F, EnemiesKilled);
        
        Debug.WriteLine(_gameState.Entities.Count + " : " + _gameState.EnemiesAlive);

        if (_gameState.EnemiesAlive == 0)
        {
            _asteroidSpawnInterval = 0;
        }
    }

    public void StopListening()
    {
        UpdateEventSource.UpdateEvent -= OnUpdate;
    }
}