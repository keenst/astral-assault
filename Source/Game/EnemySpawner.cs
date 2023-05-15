using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
    
    private List<QueuedMissile> _missileQueue = new();
    private const int MissileSpawnDelay = 1600;

    private readonly Texture2D _missileWarningTexture;

    public EnemySpawner(GameplayState gameState)
    {
        _gameState = gameState;

        UpdateEventSource.UpdateEvent += OnUpdate;
        
        _debrisController = new DebrisController(gameState);

        _missileWarningTexture = AssetManager.Load<Texture2D>("MissileWarning");
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

    private void SpawnMissile(Vector2 position)
    {
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
        
        drawTasks.AddRange(GetMissileWarningDrawTasks());
        
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
                Vector2 position = GenerateEnemyPosition();
                QueuedMissile queuedMissile = new(timeNow + MissileSpawnDelay, position);
                _missileQueue.Add(queuedMissile);
            }
        }

        HandleQueuedMissiles();

        _asteroidSpawnInterval = BaseAsteroidSpawnInterval * MathF.Pow(0.98F, EnemiesKilled);
        _missileSpawnInterval = BaseMissileSpawnInterval * MathF.Pow(0.98F, EnemiesKilled);

        if (_gameState.EnemiesAlive == 0)
        {
            _asteroidSpawnInterval = 0;
        }
    }

    private List<DrawTask> GetMissileWarningDrawTasks()
    {
        List<DrawTask> drawTasks = new();
        
        const int frameCount = 32;
        const int timePerFrame = MissileSpawnDelay / frameCount;
        
        foreach (QueuedMissile queuedMissile in _missileQueue)
        {
            long timeSpawned = queuedMissile.TimeToLaunchMS - MissileSpawnDelay;
            int timeSinceSpawned = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - timeSpawned);
            int spriteIndex = timeSinceSpawned / timePerFrame;

            Rectangle sourceRectangle = new(24 * spriteIndex, 0, 24, 24);
            
            drawTasks.Add(new DrawTask(
                _missileWarningTexture,
                sourceRectangle, 
                GetMissileWarningPosition(queuedMissile.Position), 
                0,
                LayerDepth.HUD,
                new List<IDrawTaskEffect>(),
                Color.White,
                new Vector2(12, 12)));
        }

        return drawTasks;
    }
    
    private Vector2 GetMissileWarningPosition(Vector2 missileSpawnPoint)
    {
        int x = Math.Clamp((int)missileSpawnPoint.X, 24, Game1.TargetWidth - 24);
        int y = Math.Clamp((int)missileSpawnPoint.Y, 24, Game1.TargetHeight - 24);
        
        return new Vector2(x, y);
    }

    private void HandleQueuedMissiles()
    {
        if (_missileQueue.Count == 0) return;
        
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        
        List<QueuedMissile> missilesToRemove = new();

        foreach (QueuedMissile queuedMissile in _missileQueue)
        {
            if (timeNow < queuedMissile.TimeToLaunchMS) continue;
            
            SpawnMissile(queuedMissile.Position);
            missilesToRemove.Add(queuedMissile);
        }
        
        foreach (QueuedMissile queuedMissile in missilesToRemove)
        {
            _missileQueue.Remove(queuedMissile);
        }
    }
    
    public void StopListening()
    {
        UpdateEventSource.UpdateEvent -= OnUpdate;
    }
}