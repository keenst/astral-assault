#region
using System;
using System.Collections.Generic;
using System.Linq;
using AstralAssault.Items;
using AstralAssault.Source.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace AstralAssault;

public enum PowerUps { QuadDamage, Haste }

public class Player : Entity, IInputEventListener
{
    private const float Friction = 30;
    private const float Pi = 3.14F;
    private const float BulletSpeed = 250;

    private const int PowerUpDuration = 10 * 1000;

    private readonly List<Tuple<long, PowerUps>>
        m_powerUps = new List<Tuple<long, PowerUps>>(); // (time of pick up, power up)

    private readonly Texture2D m_square;

    private Vector2 m_cursorPosition;
    private float m_delta;
    private bool m_isCrosshairActive = true;
    private bool m_lastCannon;
    private long m_lastTimeFired;
    private float m_maxSpeed = 100;

    private float m_moveSpeed = 200;
    private Tuple<Vector2, Vector2> m_muzzle = new Tuple<Vector2, Vector2>(Vector2.Zero, Vector2.Zero);
    private int m_shootSpeed = 200;
    private bool m_thrusterIsOn;
    private float m_tiltSpeed = 200;
    public float Multiplier = 1;

    public Player(GameplayState gameState, Vector2 position) : base(gameState, position)
    {
        m_square = new Texture2D(GameState.Root.GraphicsDevice, 1, 1);
        m_square.SetData(new[] { Color.White });

        Position = position;
        Rotation = Pi / 2;

        InitSpriteRenderer();

        StartListening();

        Collider = new Collider
        (
            this,
            true,
            10
        )
        {
            Radius = 10
        };

        GameState.CollisionSystem.AddCollider(Collider);

        OutOfBoundsBehavior = OutOfBounds.Wrap;

        IsActor = true;
        MaxHP = 50;
        HP = MaxHP;
        IsFriendly = true;
    }

    public void OnKeyboardEvent(object sender, KeyboardEventArgs e)
    {
        int xAxis = 0;
        int yAxis = 0;

        if (Array.IndexOf(e.Keys, Keys.D) >= 0)
        {
            xAxis = 1;
            SpriteRenderer.SetAnimationCondition("Tilt", 1);
        }
        else if (Array.IndexOf(e.Keys, Keys.A) >= 0)
        {
            xAxis = -1;
            SpriteRenderer.SetAnimationCondition("Tilt", -1);
        }
        else SpriteRenderer.SetAnimationCondition("Tilt", 0);

        if (Array.IndexOf(e.Keys, Keys.W) >= 0)
        {
            yAxis = 1;
            m_thrusterIsOn = true;
        }
        else if (Array.IndexOf(e.Keys, Keys.S) >= 0) yAxis = -1;

        HandleMovement(xAxis, yAxis);
    }

    public void OnMouseMoveEvent(object sender, MouseMoveEventArgs e)
    {
        Point scale = new Point((int)GameState.Root.ScaleX, (int)GameState.Root.ScaleY);
        m_cursorPosition.X = e.Position.ToVector2().X / scale.X;
        m_cursorPosition.Y = e.Position.ToVector2().Y / scale.Y;
    }

    public void OnMouseButtonEvent(object sender, MouseButtonEventArgs e)
    {
        if (e.Button == InputEventSource.MouseButtons.Left) HandleFiring();
    }

    private void StartListening()
    {
        InputEventSource.KeyboardEvent += OnKeyboardEvent;
        InputEventSource.MouseMoveEvent += OnMouseMoveEvent;
        InputEventSource.MouseButtonEvent += OnMouseButtonEvent;
    }

    private void InitSpriteRenderer()
    {
        Texture2D spriteSheet = AssetManager.Load<Texture2D>("Player");

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
            spriteSheet,
            new[] { idleAnimation, tiltLeftAnimation, tiltRightAnimation },
            transitions,
            new[] { "Tilt" }
        );
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        base.OnUpdate(sender, e);

        bool haste = false;

        foreach (Tuple<long, PowerUps> powerUp in m_powerUps)
        {
            if (powerUp.Item2 != PowerUps.Haste) continue;

            haste = true;

            break;
        }

        m_shootSpeed = haste ? 100 : 200;
        m_maxSpeed = haste ? 150 : 100;
        m_moveSpeed = haste ? 400 : 200;
        m_tiltSpeed = haste ? 400 : 200;

        m_delta = e.DeltaTime;

        // check range to cursor
        float distance = Vector2.Distance(Position, m_cursorPosition);
        m_isCrosshairActive = distance >= 12;

        // rotate player
        if (m_isCrosshairActive)
        {
            float xDiff = m_cursorPosition.X - Position.X;
            float yDiff = m_cursorPosition.Y - Position.Y;

            Rotation = (float)Math.Atan2(yDiff, xDiff);
        }

        // apply friction
        float sign = Math.Sign(Velocity.Length());

        if (sign != 0)
        {
            float direction = (float)Math.Atan2(Velocity.Y, Velocity.X);

            Velocity -=
                Vector2.UnitX.RotateVector(direction) * Friction * m_delta * sign;
        }

        // rotate the points for the cannon muzzles
        float rot = Pi / 8 * (float)Math.Round(Rotation / (Pi / 8));

        m_muzzle = new Tuple<Vector2, Vector2>
        (
            Position + new Vector2(10, -8).RotateVector(rot),
            Position + new Vector2(8, 10).RotateVector(rot)
        );
    }

    public override void Draw()
    {
        base.Draw();

        for (int i = 0; i < m_powerUps.Count; i++)
        {
            Tuple<long, PowerUps> powerUp = m_powerUps[i];

            long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            if ((timeNow - powerUp.Item1) > PowerUpDuration)
            {
                m_powerUps.Remove(powerUp);
                i--;

                continue;
            }

            string powerUpName = powerUp.Item2 switch
            {
                PowerUps.QuadDamage => "quad damage",
                PowerUps.Haste => "haste",
                var _ => throw new ArgumentOutOfRangeException()
            };

            Vector4 color = powerUp.Item2 switch
            {
                PowerUps.QuadDamage => Palette.GetColorVector(Palette.Colors.Purple6),
                PowerUps.Haste => Palette.GetColorVector(Palette.Colors.Red8),
                var _ => throw new ArgumentOutOfRangeException()
            };

            Vector4 backgroundColor = Palette.GetColorVector(Palette.Colors.Black);
            Vector4 barColor = ((timeNow - powerUp.Item1) / (float)PowerUpDuration) switch
            {
                < 0.25F => Palette.GetColorVector(Palette.Colors.Green7),
                < 0.5F => Palette.GetColorVector(Palette.Colors.Green4),
                < 0.75F => Palette.GetColorVector(Palette.Colors.Red8),
                < 1 => Palette.GetColorVector(Palette.Colors.Red4),
                var _ => Palette.GetColorVector(Palette.Colors.Black)
            };

            m_square.DrawTexture2D
            (
                new Rectangle(0, 0, 1, 1), new Rectangle(1, 28 + i * 12, 2, 8), 0,
                new Color(backgroundColor), LayerOrdering.Powerups
            );

            int barLength = 8 - (int)Math.Floor((timeNow - powerUp.Item1) / (float)PowerUpDuration * 8);

            m_square.DrawTexture2D
            (
                new Rectangle(0, 0, 1, 1), new Rectangle(1, 36 + i * 12 - barLength, 2, barLength), 0,
                new Color(barColor), LayerOrdering.Powerups
            );

            powerUpName.Draw
            (
                new Vector2(4, 28 + i * 12),
                new Color(color),
                0f,
                new Vector2(0, 0),
                1f, LayerOrdering.Powerups
            );
        }
    }

    public override void OnCollision(Collider other)
    {
        base.OnCollision(other);

        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (other.Parent is Quad)
        {
            Jukebox.PlaySound("PickUp");

            for (int i = m_powerUps.Count - 1; i >= 0; i--)
            {
                if (m_powerUps[i].Item2 == PowerUps.QuadDamage)
                {
                    m_powerUps.RemoveAt(i);
                }
            }


            m_powerUps.Add(new Tuple<long, PowerUps>(timeNow, PowerUps.QuadDamage));
        }
        else if (other.Parent is Haste)
        {
            Jukebox.PlaySound("PickUp");

            for (int i = m_powerUps.Count - 1; i >= 0; i--)
            {
                if (m_powerUps[i].Item2 == PowerUps.Haste)
                {
                    m_powerUps.RemoveAt(i);
                }
            }

            m_powerUps.Add(new Tuple<long, PowerUps>(timeNow, PowerUps.Haste));
        }
        else if (other.Parent is MegaHealth)
        {
            Jukebox.PlaySound("PickUp");

            HP = Math.Min(MaxHP, HP + 30);
        }
        else if (other.Parent is Asteroid)
        {
            if (Multiplier > 1) Jukebox.PlaySound("MultiplierBroken");

            Multiplier = 1;

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
    }

    protected override void OnDeath()
    {
        Game1 root = GameState.Root;

        Jukebox.PlaySound("GameOver");

        root.GameStateMachine.ChangeState(new GameOverState(root));

        base.OnDeath();
    }

    public override void Destroy()
    {
        StopListening();

        base.Destroy();
    }

    private void StopListening()
    {
        InputEventSource.KeyboardEvent -= OnKeyboardEvent;
        InputEventSource.MouseMoveEvent -= OnMouseMoveEvent;
        InputEventSource.MouseButtonEvent -= OnMouseButtonEvent;
    }

    private void HandleMovement(int xAxis, int yAxis)
    {
        // acceleration and deceleration
        Vector2 forward = Vector2.UnitX.RotateVector(Rotation) *
            m_moveSpeed * m_delta;

        Velocity = new Vector2
        (
            Math.Clamp(Velocity.X + forward.X * yAxis, -m_maxSpeed, m_maxSpeed),
            Math.Clamp(Velocity.Y + forward.Y * yAxis, -m_maxSpeed, m_maxSpeed)
        );

        // tilting
        Vector2 right = Vector2.UnitX.RotateVector(Rotation + Pi / 2) * m_tiltSpeed * m_delta;

        Velocity = new Vector2
        (
            Math.Clamp(Velocity.X + right.X * xAxis, -m_maxSpeed, m_maxSpeed),
            Math.Clamp(Velocity.Y + right.Y * xAxis, -m_maxSpeed, m_maxSpeed)
        );

        if (Velocity.Length() > m_maxSpeed)
        {
            Velocity.Normalize();
            Velocity *= m_maxSpeed;
        }
        else if (Velocity.Length() < -m_maxSpeed)
        {
            Velocity.Normalize();
            Velocity *= -m_maxSpeed;
        }
    }

    private void HandleFiring()
    {
        if (!m_isCrosshairActive) return;

        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if ((m_lastTimeFired + m_shootSpeed) > timeNow) return;

        Random rnd = new Random();
        string soundName =
            (m_powerUps.Any(t => t.Item2 is PowerUps.QuadDamage) ? "Quad" : "") +
            "Shoot" +
            rnd.Next(1, 4);

        Jukebox.PlaySound(soundName, 0.5F);

        m_lastTimeFired = timeNow;

        float xDiff = m_cursorPosition.X - (m_lastCannon ? m_muzzle.Item1.X : m_muzzle.Item2.X);
        float yDiff = m_cursorPosition.Y - (m_lastCannon ? m_muzzle.Item1.Y : m_muzzle.Item2.Y);

        float rot = (float)Math.Atan2(yDiff, xDiff);

        GameState.Entities.Add
        (
            new Bullet
            (
                GameState,
                m_lastCannon ? m_muzzle.Item1 : m_muzzle.Item2,
                rot,
                BulletSpeed,
                m_powerUps.Any(t => t.Item2 == PowerUps.QuadDamage)
            )
        );

        m_lastCannon = !m_lastCannon;
    }
}