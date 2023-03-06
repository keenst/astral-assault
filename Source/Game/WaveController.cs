using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class WaveController : IUpdateEventListener
{
    private readonly GameplayState _gameState;
    private readonly Game1 _root;
    private int _currentWave;

    private bool _drawWaveText;
    private long _waveTextTimer;
    private const long WaveTextDuration = 2000;
    
    private long _waveTimer;
    private const long WaveDelay = 5000;
    
    public WaveController(GameplayState gameState, Game1 root)
    {
        _gameState = gameState;
        _root = root;

        UpdateEventSource.UpdateEvent += OnUpdate;
        
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
            
            _gameState.Entities.Add(new Asteroid(_gameState, position, size));
        }

        _drawWaveText = true;
        _waveTextTimer = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (!_drawWaveText) return;
        spriteBatch.Write($"Wave{_currentWave}", new Vector2(10, 10), Color.White);
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        if (_drawWaveText && timeNow - _waveTextTimer > WaveTextDuration)
        {
            _drawWaveText = false;
        }

        int enemiesAlive = _gameState.Entities.Count(x => x is Asteroid);
        if (enemiesAlive == 0)
        {
            if (timeNow - _waveTimer < WaveDelay) return;
            
            StartNextWave();
            _waveTimer = timeNow;
        }
    }

    public void Start()
    {
        
    }

    public void Stop()
    {
        
    }
}