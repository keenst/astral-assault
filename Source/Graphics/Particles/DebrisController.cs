using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class DebrisController
{
    public readonly ParticleEmitter ParticleEmitter;
    private readonly Random m_rnd = new Random();
    private readonly GameplayState m_gameplayState;

    public DebrisController(GameplayState gameplayState)
    {
        m_gameplayState = gameplayState;

        Texture2D particleSpriteSheet = AssetManager.Load<Texture2D>("AsteroidDebris");

        Rectangle[] textureSources =
        {
            new Rectangle(0, 0, 8, 8), new Rectangle(8, 0, 8, 8), new Rectangle(16, 0, 8, 8), new Rectangle(32, 0, 8, 8)
        };

        IParticleProperty[] particleProperties =
        {
            new VelocityProperty(-1, 1, 0.05F, 0.15F),
            new RandomSpriteProperty(0, textureSources.Length - 1),
            new CauseOfDeathProperty(CauseOfDeathProperty.CausesOfDeath.OutOfBounds)
        };

        ParticleEmitter = new ParticleEmitter
        (
            particleSpriteSheet,
            textureSources,
            0,
            Vector2.Zero,
            0,
            particleProperties,
            LayerDepth.Debris
        );
    }

    public void SpawnDebris(Vector2 position, int asteroidSize)
    {
        int amount = m_rnd.Next(4, (1 + asteroidSize) * 4);

        float angleFromPlayer = MathF.Atan2
        (
            position.Y - m_gameplayState.Player.Position.Y,
            position.X - m_gameplayState.Player.Position.X
        );

        ParticleEmitter.SetTransform(position, angleFromPlayer);
        ParticleEmitter.StartSpawning(amount);
    }

    public List<DrawTask> GetDrawTasks() => ParticleEmitter.CreateDrawTasks();
}