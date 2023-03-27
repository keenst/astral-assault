using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public class WaveController
{
    public readonly GameplayState GameState;
    private readonly Game1 _root;
    private readonly DebrisController _debrisController;
    private int _currentWave;

    private bool _drawWaveText;
    private long _waveTextTimer;
    private const long WaveTextDuration = 2000;
    
    private long _waveTimer;
    private const long WaveDelay = 5000;
    
    public WaveController(GameplayState gameState, Game1 root)
    {
        GameState = gameState;
        _root = root;

        _debrisController = new DebrisController(gameState);
        
        StartNextWave();
    }
    
    private void StartNextWave()
    {
        _currentWave++;
        
        int enemiesToSpawn = (int)(_currentWave * 1.5F);
        
        Random rnd = new();
        for (int i = 0; i < enemiesToSpawn; i++)
        {
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
            
            GameState.Entities.Add(new Asteroid(GameState, position, angleToCenter, size, _debrisController));
        }

        _drawWaveText = true;
        _waveTextTimer = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }
    
    public List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = _debrisController.GetDrawTasks();
        
        if (!_drawWaveText) return drawTasks;
        
        string text = $"Wave: {_currentWave}";
        drawTasks.AddRange(text.CreateDrawTasks(new Vector2(10, 10), Color.White, LayerDepth.HUD));

        return drawTasks;
    }

    public void Update(float deltaTime)
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        if (_drawWaveText && timeNow - _waveTextTimer > WaveTextDuration)
        {
            _drawWaveText = false;
        }

        int enemiesAlive = GameState.Entities.Count(x => x is Asteroid);
        if (enemiesAlive == 0)
        {
            if (timeNow - _waveTimer < WaveDelay) return;
            
            StartNextWave();
            _waveTimer = timeNow;
        }
    }

    public void StopListening()
    {
    }
}