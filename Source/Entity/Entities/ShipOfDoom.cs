#region
using System;
using AstralAssault.Source.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AstralAssault;

public class ShipOfDoom : Entity
{
    private readonly float m_rotSpeed;
    private bool m_lastCannon;
    private long m_lastTimeFired;
    private Tuple<Vector2, Vector2> m_muzzle = new Tuple<Vector2, Vector2>(Vector2.Zero, Vector2.Zero);

    public ShipOfDoom(
        GameplayState gameState,
        Vector2 position,
        float direction
    )
        : base(gameState, position)
    {
        Random rnd = new Random();
        m_rotSpeed = rnd.Next(5, 20) / 10F;
        int speed = rnd.Next(30, 100);

        Velocity = Vector2.UnitX.RotateVector(direction) * speed;

        InitSpriteRenderer();

        Collider = new Collider
        (
            this,
            true,
            10
        )
        {
            Radius = 10
        };

        MaxHP = 85;
        HP = MaxHP;

        GameState.CollisionSystem.AddCollider(Collider);

        OutOfBoundsBehavior = OutOfBounds.Wrap;

        IsActor = true;
    }

    private void InitSpriteRenderer()
    {
        Texture2D spriteSheet = AssetManager.Load<Texture2D>("ShipOfDoom");

        Animation idleAnimation = AnimationCreator.CreateAnimFromSpriteSheet
        (
            32,
            32,
            0,
            12,
            1,
            new[] { 0 },
            true,
            true,
            false,
            false,
            0
        );

        Animation tiltLeftAnimation = AnimationCreator.CreateAnimFromSpriteSheet
        (
            32,
            32,
            1,
            12,
            1,
            new[] { 0 },
            true,
            false,
            false,
            false,
            0
        );

        Animation tiltRightAnimation = AnimationCreator.CreateAnimFromSpriteSheet
        (
            32,
            32,
            11,
            12,
            1,
            new[] { 0 },
            true,
            false,
            false,
            false,
            0
        );

        Transition[] transitions =
        {
            new Transition(1, 0, new[] { 0 }, "Tilt", 0),
            new Transition(0, 1, new[] { 1 }, "Tilt", -1),
            new Transition(0, 2, new[] { 2 }, "Tilt", 1),
            new Transition(2, 0, new[] { 0 }, "Tilt", 0),
            new Transition(2, 1, new[] { 0, 1 }, "Tilt", -1),
            new Transition(1, 2, new[] { 0, 2 }, "Tilt", 1)
        };

        SpriteRenderer = new SpriteRenderer
        (
            this,
            spriteSheet,
            new[] { idleAnimation, tiltLeftAnimation, tiltRightAnimation },
            transitions,
            new[] { "Tilt" },
            GameState.Root
        );
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        base.OnUpdate(sender, e);

        Vector2 distanceToPlayer = GameState.Player.Position - Position;
        Vector2 dir = Vector2.Normalize(distanceToPlayer);

        float angle = MathF.Atan2(distanceToPlayer.Y, distanceToPlayer.X);

        Velocity = dir * 35;

        Rotation = angle;

        // rotate the points for the cannon muzzles
        float rot = MathF.PI / 8 * (float)Math.Round(Rotation / (MathF.PI / 8));

        m_muzzle = new Tuple<Vector2, Vector2>
        (
            Position + new Vector2(10, -8).RotateVector(rot),
            Position + new Vector2(8, 10).RotateVector(rot)
        );

        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if ((m_lastTimeFired + 200) > timeNow) return;

        m_lastTimeFired = timeNow;

        GameState.Entities.Add
        (
            new Bullet
            (
                GameState,
                m_lastCannon ? m_muzzle.Item1 : m_muzzle.Item2,
                rot,
                250,
                false,
                this
            )
        );

        m_lastCannon = !m_lastCannon;
    }

    public override void OnCollision(Collider other)
    {
        base.OnCollision(other);

        if (other.Parent is not Bullet) return;

        Random rnd = new Random();

        string soundName = rnd.Next(3) switch
        {
            0 => "Hurt1",
            1 => "Hurt2",
            2 => "Hurt3",
            var _ => throw new ArgumentOutOfRangeException()
        };

        Jukebox.PlaySound(soundName, 0.5F);
    }

    protected override void OnDeath()
    {
        Random rnd = new Random();

        string explosionSound = rnd.Next(3) switch
        {
            0 => "Explosion1",
            1 => "Explosion2",
            2 => "Explosion3",
            var _ => throw new ArgumentOutOfRangeException()
        };

        Jukebox.PlaySound(explosionSound);

        GameState.Player.Multiplier += 0.5F;

        GameState.Root.Score += (int)(1600 * GameState.Player.Multiplier);

        base.OnDeath();
    }
}