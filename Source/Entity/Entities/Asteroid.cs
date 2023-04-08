using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class Asteroid : Entity
{
    private readonly DebrisController m_debrisController;
    private readonly float m_rotSpeed;
    private readonly Sizes m_size;
    private bool m_hasExploded;

    public enum Sizes { Smallest, Small, Medium }

    public Asteroid(
        GameplayState gameState,
        Vector2 position,
        float direction,
        Sizes size,
        DebrisController debrisController)
        : base(gameState, position)
    {
        m_debrisController = debrisController;

        m_size = size;

        Random rnd = new Random();
        m_rotSpeed = rnd.Next(5, 20) / 10F;
        int speed = rnd.Next(30, 100);

        Velocity = Vector2.UnitX.RotateVector(direction) * speed;

        Texture2D spriteSheet;
        int colliderSize;
        int spriteSize;
        float mass;

        switch (size)
        {
        case Sizes.Smallest:
            spriteSheet = AssetManager.Load<Texture2D>("Asteroid1");
            spriteSize = 16;
            colliderSize = 10;
            MaxHP = 12;
            HP = MaxHP;
            ContactDamage = 5;
            mass = 6;

            break;

        case Sizes.Small:
            spriteSheet = AssetManager.Load<Texture2D>("Asteroid2");
            spriteSize = 24;
            colliderSize = 16;
            MaxHP = 24;
            HP = MaxHP;
            ContactDamage = 7;
            mass = 12;

            break;

        case Sizes.Medium:
            spriteSheet = AssetManager.Load<Texture2D>("Asteroid3");
            spriteSize = 32;
            colliderSize = 24;
            MaxHP = 36;
            HP = MaxHP;
            ContactDamage = 12;
            mass = 18;

            break;

        default:
            throw new ArgumentOutOfRangeException();
        }

        Frame frame = new Frame
        (
            new Rectangle(0, 0, spriteSize, spriteSize), new Rectangle(spriteSize, 0, spriteSize, spriteSize),
            new Rectangle(spriteSize * 2, 0, spriteSize, spriteSize),
            new Rectangle(spriteSize * 3, 0, spriteSize, spriteSize)
        );

        Animation animation = new Animation(new[] { frame }, true);

        SpriteRenderer = new SpriteRenderer(spriteSheet, new[] { animation }, LayerDepth.Foreground);

        Collider = new Collider
        (
            this,
            new Rectangle
            (
                new Point((int)Position.X - colliderSize / 2, (int)Position.Y - colliderSize / 2),
                new Point(colliderSize, colliderSize)
            ),
            true,
            mass
        );
        GameState.CollisionSystem.AddCollider(Collider);

        OutOfBoundsBehavior = OutOfBounds.Wrap;

        IsActor = true;
    }

    protected override void OnDeath()
    {
        if (!m_hasExploded && ((m_size - 1) >= 0))
        {
            Random rnd = new Random();
            int amount = rnd.Next(1, 4);

            Vector2 playerPosition = GameState.Player.Position;
            float angleToPlayer = MathF.Atan2(Position.Y - playerPosition.Y, Position.X - playerPosition.X);

            for (int i = 0; i < amount; i++)
            {
                angleToPlayer += (float)rnd.NextDouble() * MathF.PI / 1 - MathF.PI / 2;

                GameState.Entities.Add
                (
                    new Asteroid(GameState, Position, angleToPlayer, m_size - 1, m_debrisController)
                );
            }
        }

        m_hasExploded = true;

        m_debrisController.SpawnDebris(Position, (int)m_size);

        base.OnDeath();
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        base.OnUpdate(sender, e);

        m_debrisController.ParticleEmitter.OnUpdate(sender, e);

        Rotation += m_rotSpeed * e.DeltaTime;
        if (Rotation > Math.PI) Rotation = (float)-Math.PI;
    }
}