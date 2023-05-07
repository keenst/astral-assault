#region
using System;
using AstralAssault.Items;
using Microsoft.Xna.Framework;
#endregion

namespace AstralAssault;

public class WaveController
{
    public readonly GameplayState GameState;
    private int m_currentWave;
    private bool m_levelUpHehe;

    public WaveController(GameplayState gameState, Game1 root)
    {
        GameState = gameState;

        StartNextWave();
    }

    public void StartNextWave()
    {
        GameState.ItemController.NewWave();

        m_currentWave++;

        if (m_currentWave == 5) m_levelUpHehe = true;

        int enemiesToSpawn = (int)(m_currentWave * 1.1F);

        Random rnd = new Random();

        switch (m_levelUpHehe)
        {
        case true:
            int shipsOfDoomToSpawn = m_currentWave - 4;

            for (int i = 0; i < (enemiesToSpawn - shipsOfDoomToSpawn); i++)
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

            for (int i = 0; i < shipsOfDoomToSpawn; i++)
            {
                int pside = rnd.Next(0, 4);
                int px = pside switch
                {
                    0 => 0,
                    1 => Game1.TargetWidth,
                    2 => rnd.Next(0, Game1.TargetWidth),
                    3 => rnd.Next(0, Game1.TargetWidth),
                    var _ => throw new ArgumentOutOfRangeException()
                };
                int py = pside switch
                {
                    0 => rnd.Next(0, Game1.TargetHeight),
                    1 => rnd.Next(0, Game1.TargetHeight),
                    2 => 0,
                    3 => Game1.TargetHeight,
                    var _ => throw new ArgumentOutOfRangeException()
                };

                GameState.Entities.Add(new ShipOfDoom(GameState, new Vector2(px, py), 0));
            }

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
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        int enemiesAlive = 0;

        foreach (Entity ez in GameState.Entities)
        {
            if (ez is Quad or Haste or MegaHealth or Crosshair or Player) continue;

            enemiesAlive++;
        }

        if (enemiesAlive == 0) StartNextWave();
    }

    public void Draw()
    {
        string text = $"Wave: {m_currentWave}";
        Color color = Palette.GetColor(Palette.Colors.Grey9);
        text.Draw(new Vector2(4, 16), color, 0f, new Vector2(0, 0), 1f, LayerOrdering.Hud);
    }
}