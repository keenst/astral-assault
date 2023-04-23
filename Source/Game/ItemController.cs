using System;
using AstralAssault.Items;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public class ItemController : IUpdateEventListener
{
    private long _lastSpawnTimeMS;
    private int _spawnInterval = 5000;
    private readonly GameplayState _gameState;
    private readonly Random _rnd = new();
    private int _spawnedThisWave;
    
    public ItemController(GameplayState gameState)
    {
        _gameState = gameState;
    }

    public void StartListening()
    {
        UpdateEventSource.UpdateEvent += OnUpdate;
    }
    
    public void StopListening()
    {
        UpdateEventSource.UpdateEvent -= OnUpdate;
    }
    
    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        
        if (timeNow - _lastSpawnTimeMS <= _spawnInterval) return;
        
        _lastSpawnTimeMS = timeNow;
        SpawnItem();
    }

    private void SpawnItem()
    {
        if (_spawnedThisWave >= 3) return;
        
        _spawnedThisWave++;
        
        Vector2 position;
        bool isTooCloseToPlayer;

        do
        {
            position = new Vector2(_rnd.Next(Game1.TargetWidth), _rnd.Next(Game1.TargetHeight));
            if (Vector2.Distance(position, _gameState.Player.Position) < 100 ||
                position.X is < 100 or > Game1.TargetWidth - 100 ||
                position.Y is < 100 or > Game1.TargetHeight - 100)
            {
                isTooCloseToPlayer = true;
                continue;
            }

            isTooCloseToPlayer = false;
        }
        while (isTooCloseToPlayer);
        
        Entity item = _rnd.Next(3) switch
        {
            0 => new Quad(_gameState, position),
            1 => new Haste(_gameState, position),
            2 => new MegaHealth(_gameState, position),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        _gameState.Entities.Add(item);
    }
    
    public void NewWave()
    {
        _spawnedThisWave = 0;
    }
}