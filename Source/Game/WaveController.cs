#region
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using AstralAssault.Source.Graphics;
using Microsoft.Xna.Framework;
#endregion

namespace AstralAssault;

public class WaveController
{
    private const long WaveTextDuration = 2000;
    private const long WaveDelay = 5000;
    public readonly GameplayState GameState;
    private readonly Game1 m_root;
    private bool levelUppHehe = false;
    private int m_currentWave;

    private bool m_drawWaveText;
    private long m_waveTextTimer;

    private long m_waveTimer;

    public WaveController(GameplayState gameState, Game1 root)
    {
        GameState = gameState;
        m_root = root;

        StartNextWave();
    }

    public void StartNextWave()
    {
        GameState.ItemController.NewWave();

        m_currentWave++;

        if (m_currentWave == 6)
        {
            levelUppHehe = true;
        }

        int enemiesToSpawn = (int)(m_currentWave * 1.1F);

        Random rnd = new Random();

        switch (levelUppHehe)
        {
        case true:
            break;
        case false:
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                int side = rnd.Next(0, 4);

                int x = side switch
                {
                    0 => 0,
                    1 => Game1.TargetWidth,
                    2 => rnd.Next(0, Game1.TargetWidth),
                    3 => rnd.Next(0, Game1.TargetWidth),
                    var _ => throw new ArgumentOutOfRangeException()
                };

                int y = side switch
                {
                    0 => rnd.Next(0, Game1.TargetHeight),
                    1 => rnd.Next(0, Game1.TargetHeight),
                    2 => 0,
                    3 => Game1.TargetHeight,
                    var _ => throw new ArgumentOutOfRangeException()
                };

                Vector2 position = new Vector2(x, y);
                Asteroid.Sizes size = (Asteroid.Sizes)rnd.Next(0, 3);

                Vector2 gameCenter = new Vector2(Game1.TargetWidth / 2F, Game1.TargetHeight / 2F);
                float angleToCenter = MathF.Atan2(gameCenter.Y - position.Y, gameCenter.X - position.X);
                angleToCenter += MathHelper.ToRadians(rnd.Next(-45, 45));

                GameState.Entities.Add(new Asteroid(GameState, position, angleToCenter, size));
            }

            break;
        }

        m_drawWaveText = true;
        m_waveTextTimer = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    public void Draw()
    {
        //if (!_drawWaveText) return drawTasks;

        string text = $"Wave: {m_currentWave}";
        Color color = Palette.GetColor(Palette.Colors.Grey9);
        text.Draw(new Vector2(4, 16), color, 0f, new Vector2(0, 0), 1f, LayerOrdering.Hud);
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (m_drawWaveText && ((timeNow - m_waveTextTimer) > WaveTextDuration)) m_drawWaveText = false;

        int enemiesAlive = 0;

        foreach (Entity entity in GameState.Entities)
        {
            if (entity is Asteroid) enemiesAlive++;
        }

        if (enemiesAlive == 0)
        {
            if ((timeNow - m_waveTimer) < WaveDelay) return;

            StartNextWave();
            m_waveTimer = timeNow;
        }
    }
}