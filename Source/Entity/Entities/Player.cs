#region
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace AstralAssault;

public class Player : Entity, IInputEventListener
{
    private const float MoveSpeed = 200;
    private const float MaxSpeed = 100;
    private const float TiltSpeed = 200;
    private const float Friction = 30;
    private const float Pi = 3.14F;
    private const float BulletSpeed = 250;
    private const int ShootSpeed = 200;
    private readonly ParticleEmitter m_particleEmitter;
    private Vector2 m_cursorPosition;
    private float m_delta;
    private bool m_isCrosshairActive = true;
    private bool m_lastCannon;
    private long m_lastTimeFired;
    private Tuple<Vector2, Vector2> m_muzzle = new Tuple<Vector2, Vector2>(Vector2.Zero, Vector2.Zero);
    private bool m_thrusterIsOn;

    public Player(GameplayState gameState, Vector2 position) : base(gameState, position)
    {
        Position = position;
        Rotation = Pi / 2;

        InitSpriteRenderer();

        StartListening();

        Collider = new Collider
        (
            this,
            new Rectangle
            (
                new Point((int)Position.X - 12, (int)Position.Y - 12),
                new Point(24, 24)
            ),
            true,
            10
        );
        GameState.CollisionSystem.AddCollider(Collider);

        Texture2D particleSpriteSheet = AssetManager.Load<Texture2D>("Particle");

        Rectangle[] textureSources =
        {
            new Rectangle(24, 0, 8, 8), new Rectangle(16, 0, 8, 8), new Rectangle(8, 0, 8, 8), new Rectangle(0, 0, 8, 8)
        };

        IParticleProperty[] particleProperties =
        {
            new CauseOfDeathProperty(CauseOfDeathProperty.CausesOfDeath.LifeSpan, 210),
            new ColorChangeProperty
            (
                new[]
                {
                    Palette.Colors.Blue9,
                    Palette.Colors.Blue8,
                    Palette.Colors.Blue7,
                    Palette.Colors.Blue6,
                    Palette.Colors.Blue5,
                    Palette.Colors.Blue4,
                    Palette.Colors.Blue3
                },
                30
            ),
            new SpriteChangeProperty(0, textureSources.Length, 40),
            new VelocityProperty(-1F, 1F, 0.04F, 0.1F)
        };

        m_particleEmitter = new ParticleEmitter
        (
            particleSpriteSheet,
            textureSources,
            20,
            Position,
            Rotation,
            particleProperties,
            LayerDepth.ThrusterFlame
        );

        m_particleEmitter.StartSpawning();

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

        if (e.Keys.Contains(Keys.D))
        {
            xAxis = 1;
            SpriteRenderer.SetAnimationCondition("Tilt", 1);
        }
        else if (e.Keys.Contains(Keys.A))
        {
            xAxis = -1;
            SpriteRenderer.SetAnimationCondition("Tilt", -1);
        }
        else SpriteRenderer.SetAnimationCondition("Tilt", 0);

        if (e.Keys.Contains(Keys.W))
        {
            yAxis = 1;
            m_thrusterIsOn = true;
        }
        else if (e.Keys.Contains(Keys.S)) yAxis = -1;

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

    private void InitSpriteRenderer()
    {
        Texture2D spriteSheet = AssetManager.Load<Texture2D>("Player");

        Animation idleAnimation = new Animation
        (
            new[]
            {
                new Frame
                (
                    new Rectangle(0, 0, 32, 32), new Rectangle(0, 32, 32, 32), new Rectangle(0, 64, 32, 32),
                    new Rectangle(0, 96, 32, 32), 120
                )
            }, true
        );

        Animation tiltRightAnimation = new Animation
        (
            new[]
            {
                new Frame
                (
                    new Rectangle(352, 0, 32, 32), new Rectangle(352, 32, 32, 32), new Rectangle(352, 64, 32, 32),
                    new Rectangle(352, 96, 32, 32)
                )
            }, true
        );

        Animation tiltLeftAnimation = new Animation
        (
            new[]
            {
                new Frame
                (
                    new Rectangle(32, 0, 32, 32), new Rectangle(32, 32, 32, 32), new Rectangle(32, 64, 32, 32),
                    new Rectangle(32, 96, 32, 32)
                )
            }, true
        );

        Transition[] transitions =
        {
            new Transition(1, 0, new[] { 0 }, "Tilt", 0), new Transition(0, 1, new[] { 1 }, "Tilt", 1),
            new Transition(0, 2, new[] { 2 }, "Tilt", -1), new Transition(2, 0, new[] { 0 }, "Tilt", 0),
            new Transition(2, 1, new[] { 0, 1 }, "Tilt", 1), new Transition(1, 2, new[] { 0, 2 }, "Tilt", -1)
        };

        SpriteRenderer = new SpriteRenderer
        (
            spriteSheet,
            new[] { idleAnimation, tiltRightAnimation, tiltLeftAnimation },
            LayerDepth.Foreground,
            transitions,
            new[] { "Tilt" }
        );
    }

    public override List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new List<DrawTask>();

        if (m_thrusterIsOn) m_particleEmitter.StartSpawning();
        else m_particleEmitter.StopSpawning();

        drawTasks.AddRange(m_particleEmitter.CreateDrawTasks());
        drawTasks.AddRange(base.GetDrawTasks());

        m_thrusterIsOn = false;

        return drawTasks;
    }

    private void StartListening()
    {
        InputEventSource.KeyboardEvent += OnKeyboardEvent;
        InputEventSource.MouseMoveEvent += OnMouseMoveEvent;
        InputEventSource.MouseButtonEvent += OnMouseButtonEvent;
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
        Vector2 forward = -Vector2.UnitY.RotateVector(Rotation + Pi / 2) *
            MoveSpeed * m_delta;

        // tilting
        Vector2 right = Vector2.UnitX.RotateVector(Rotation + Pi / 2) * TiltSpeed *
            m_delta;

        // apply velocity vectors
        Velocity = new Vector2
        (
            Math.Clamp(Velocity.X + forward.X * yAxis + right.X * xAxis, -MaxSpeed, MaxSpeed),
            Math.Clamp(Velocity.Y + forward.Y * yAxis + right.Y * xAxis, -MaxSpeed, MaxSpeed)
        );

        if (Velocity.Length() > MaxSpeed)
        {
            Velocity.Normalize();
            Velocity *= MaxSpeed;
        }
        else if (Velocity.Length() < -MaxSpeed)
        {
            Velocity.Normalize();
            Velocity *= -MaxSpeed;
        }
    }

    private void HandleFiring()
    {
        if (!m_isCrosshairActive) return;

        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if ((m_lastTimeFired + ShootSpeed) > timeNow) return;

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
                BulletSpeed
            )
        );

        m_lastCannon = !m_lastCannon;
    }

    public override void Destroy()
    {
        StopListening();

        base.Destroy();
    }

    protected override void OnDeath()
    {
        Game1 root = GameState.Root;

        root.GameStateMachine.ChangeState(new GameOverState(root));

        base.OnDeath();
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        base.OnUpdate(sender, e);

        m_particleEmitter.OnUpdate(sender, e);

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

        float emitterRotation = (Rotation + Pi) % (2 * Pi);
        Vector2 emitterPosition = Position + new Vector2(11, 0).RotateVector(emitterRotation);

        m_particleEmitter.SetTransform(emitterPosition, emitterRotation);
    }
}