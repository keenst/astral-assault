#region
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using TheGameOfDoomHmmm.Source.Entity.Entities.Items;
using TheGameOfDoomHmmm.Source.Game.GameState;
#endregion

namespace TheGameOfDoomHmmm.Source.Game;

internal sealed class ItemController : IUpdateEventListener
{
    private const int SpawnInterval = 10000;
    private readonly GameplayState m_gameState;
    private readonly Random m_rnd = new Random();
    private long m_lastSpawnTimeMS;
    private int m_spawnedThisWave;

    public ItemController(GameplayState gameState)
    {
        m_gameState = gameState;
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if ((timeNow - m_lastSpawnTimeMS) <= SpawnInterval) return;

        m_lastSpawnTimeMS = timeNow;
        SpawnItem();
    }

    public void StartListening()
    {
        UpdateEventSource.UpdateEvent += OnUpdate;
    }

    public void StopListening()
    {
        UpdateEventSource.UpdateEvent -= OnUpdate;
    }

    private void SpawnItem()
    {
        if (m_spawnedThisWave >= 3) return;

        m_spawnedThisWave++;

        Vector2 position;
        bool isTooCloseToPlayer;

        do
        {
            position = new Vector2(m_rnd.Next(Game1.TargetWidth), m_rnd.Next(Game1.TargetHeight));

            if ((Vector2.Distance(position, m_gameState.Player.Position) < 100) ||
                position.X is < 100 or > Game1.TargetWidth - 100 ||
                position.Y is < 100 or > Game1.TargetHeight - 100)
            {
                isTooCloseToPlayer = true;

                continue;
            }

            isTooCloseToPlayer = false;
        } while (isTooCloseToPlayer);

        bool powerupExistsInWorld = false;
        Entity.Entities.Entity item;

        item = m_rnd.Next(3) switch
        {
            0 => new Quad(m_gameState, position),
            1 => new Haste(m_gameState, position),
            2 => new MegaHealth(m_gameState, position),
            var _ => throw new ArgumentOutOfRangeException()
        };

        m_gameState.Entities.Add(item);
    }

    public void NewWave()
    {
        m_spawnedThisWave = 0;
    }
}