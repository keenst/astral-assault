#region
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
#endregion

namespace AstralAssault;

public class WaveController
{
    private const long WaveTextDuration = 2000;
    private const long WaveDelay = 5000;
    public readonly GameplayState GameState;
    private readonly DebrisController m_debrisController;
    private readonly Game1 m_root;
    private int m_currentWave;

    private bool m_drawWaveText;
    private long m_waveTextTimer;

    private long m_waveTimer;

    public WaveController(GameplayState gameState, Game1 root)
    {
        GameState = gameState;
        m_root = root;

        m_debrisController = new DebrisController(gameState);

        StartNextWave();
    }

    public void StartNextWave()
    {
        m_currentWave++;

        int enemiesToSpawn = (int)(m_currentWave * 1.5F);

        Random rnd = new Random();

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

            GameState.Entities.Add(new Asteroid(GameState, position, angleToCenter, size, m_debrisController));
        }

        m_drawWaveText = true;
        m_waveTextTimer = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    public List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = m_debrisController.GetDrawTasks();

        if (!m_drawWaveText) return drawTasks;

        string text = $"Wave: {m_currentWave}";
        drawTasks.AddRange
            (text.CreateDrawTasks(new Vector2(10, 10), Palette.GetColor(Palette.Colors.Grey9), LayerDepth.HUD));

        return drawTasks;
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (m_drawWaveText && ((timeNow - m_waveTextTimer) > WaveTextDuration)) m_drawWaveText = false;

        int enemiesAlive = GameState.Entities.Count(static x => x is Asteroid);

        if (enemiesAlive == 0)
        {
            if ((timeNow - m_waveTimer) < WaveDelay) return;

            StartNextWave();
            m_waveTimer = timeNow;
        }
    }
}