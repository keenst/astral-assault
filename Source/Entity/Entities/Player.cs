using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace AstralAssault;

public class Player : Entity, IInputEventListener
{
    private Vector2 _cursorPosition;
    private Tuple<Vector2, Vector2> _muzzle = new(Vector2.Zero, Vector2.Zero);
    private bool _lastCannon;
    private bool _isCrosshairActive = true;
    private bool _thrusterIsOn;
    private long _lastTimeFired;
    private float _delta;
    private readonly ParticleEmitter _particleEmitter;

    private const float MoveSpeed = 200;
    private const float MaxSpeed = 100;
    private const float TiltSpeed = 200;
    private const float Friction = 30;
    private const float Pi = 3.14F;
    private const float BulletSpeed = 250;
    private const int ShootSpeed = 200;

    public Player(GameplayState gameState, Vector2 position) : base(gameState, position)
    {
        Position = position;
        Rotation = Player.Pi / 2;

        InitSpriteRenderer();

        StartListening();

        Collider = new(
            this,
            new(
                new((int)Position.X - 12, (int)Position.Y - 12),
                new(24, 24)),
            true,
            10);
        GameState.CollisionSystem.AddCollider(Collider);

        Texture2D particleSpriteSheet = AssetManager.Load<Texture2D>("Particle");

        Rectangle[] textureSources =
        {
            new(24, 0, 8, 8),
            new(16, 0, 8, 8),
            new(8, 0, 8, 8),
            new(0, 0, 8, 8)
        };

        IParticleProperty[] particleProperties =
        {
            new CauseOfDeathProperty(CauseOfDeathProperty.CausesOfDeath.LifeSpan, 210),
            new ColorChangeProperty(
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
                30),
            new SpriteChangeProperty(0, textureSources.Length, 40),
            new VelocityProperty(-1F, 1F, 0.04F, 0.1F)
        };

        _particleEmitter = new(
            particleSpriteSheet,
            textureSources,
            20,
            Position,
            Rotation,
            particleProperties,
            LayerDepth.ThrusterFlame);

        _particleEmitter.StartSpawning();

        OutOfBoundsBehavior = OutOfBounds.Wrap;

        IsActor = true;
        MaxHP = 50;
        HP = MaxHP;
        IsFriendly = true;
    }

    private void InitSpriteRenderer()
    {
        Texture2D spriteSheet = AssetManager.Load<Texture2D>("Player");

        Frame frame = new(
            new(0, 0, 32, 32),
            new(32, 0, 32, 32),
            new(64, 0, 32, 32),
            new(96, 0, 32, 32));

        Animation animation = new(new[] { frame }, true);

        SpriteRenderer = new(spriteSheet, new[] { animation }, LayerDepth.Foreground);
    }

    public override List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new();

        if (_thrusterIsOn)
        {
            _particleEmitter.StartSpawning();
        }
        else
        {
            _particleEmitter.StopSpawning();
        }

        drawTasks.AddRange(_particleEmitter.CreateDrawTasks());
        drawTasks.AddRange(base.GetDrawTasks());

        _thrusterIsOn = false;

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
        Vector2 forward = -Vector2.UnitY.RotateVector(Rotation + Player.Pi / 2) *
            Player.MoveSpeed * _delta;

        // tilting
        Vector2 right = Vector2.UnitX.RotateVector(Rotation + Player.Pi / 2) * Player.TiltSpeed *
            _delta;

        // apply velocity vectors
        Velocity = new(
            Math.Clamp(Velocity.X + forward.X * yAxis + right.X * xAxis, -Player.MaxSpeed, Player.MaxSpeed),
            Math.Clamp(Velocity.Y + forward.Y * yAxis + right.Y * xAxis, -Player.MaxSpeed, Player.MaxSpeed));

        if (Velocity.Length() > Player.MaxSpeed)
        {
            Velocity.Normalize();
            Velocity *= Player.MaxSpeed;
        }
        else if (Velocity.Length() < -Player.MaxSpeed)
        {
            Velocity.Normalize();
            Velocity *= -Player.MaxSpeed;
        }
    }

    private void HandleFiring()
    {
        if (!_isCrosshairActive) return;

        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (_lastTimeFired + Player.ShootSpeed > timeNow) return;

        _lastTimeFired = timeNow;

        float xDiff = _cursorPosition.X - (_lastCannon ? _muzzle.Item1.X : _muzzle.Item2.X);
        float yDiff = _cursorPosition.Y - (_lastCannon ? _muzzle.Item1.Y : _muzzle.Item2.Y);

        float rot = (float)Math.Atan2(yDiff, xDiff);

        GameState.Entities.Add(
            new Bullet(
                GameState,
                _lastCannon ? _muzzle.Item1 : _muzzle.Item2,
                rot,
                Player.BulletSpeed));

        _lastCannon = !_lastCannon;
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

    public void OnKeyboardEvent(object sender, KeyboardEventArgs e)
    {
        int xAxis = 0;
        int yAxis = 0;

        if (e.Keys.Contains(Keys.D))
            xAxis = 1;
        else if (e.Keys.Contains(Keys.A))
            xAxis = -1;

        if (e.Keys.Contains(Keys.W))
            yAxis = 1;
        else if (e.Keys.Contains(Keys.S))
            yAxis = -1;

        if (yAxis == 1)
        {
            _thrusterIsOn = true;
        }

        HandleMovement(xAxis, yAxis);
    }

    public void OnMouseMoveEvent(object sender, MouseMoveEventArgs e)
    {
        Point scale = new((int)GameState.Root.ScaleX, (int)GameState.Root.ScaleY);
        _cursorPosition.X = e.Position.ToVector2().X / scale.X;
        _cursorPosition.Y = e.Position.ToVector2().Y / scale.Y;
    }

    public void OnMouseButtonEvent(object sender, MouseButtonEventArgs e)
    {
        if (e.Button == InputEventSource.MouseButtons.Left)
        {
            HandleFiring();
        }
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        base.OnUpdate(sender, e);

        _particleEmitter.OnUpdate(sender, e);

        _delta = e.DeltaTime;

        // check range to cursor
        float distance = Vector2.Distance(Position, _cursorPosition);
        _isCrosshairActive = distance >= 12;

        // rotate player
        if (_isCrosshairActive)
        {
            float xDiff = _cursorPosition.X - Position.X;
            float yDiff = _cursorPosition.Y - Position.Y;

            Rotation = (float)Math.Atan2(yDiff, xDiff);
        }

        // apply friction
        float sign = Math.Sign(Velocity.Length());

        if (sign != 0)
        {
            float direction = (float)Math.Atan2(Velocity.Y, Velocity.X);

            Velocity -=
                ExtensionMethods.RotateVector(Vector2.UnitX, direction) * Player.Friction * _delta * sign;
        }

        // rotate the points for the cannon muzzles
        float rot = Player.Pi / 8 * (float)Math.Round(Rotation / (Player.Pi / 8));

        _muzzle = new(
            Position + new Vector2(10, -8).RotateVector(rot),
            Position + new Vector2(8, 10).RotateVector(rot));

        float emitterRotation = (Rotation + Player.Pi) % (2 * Player.Pi);
        Vector2 emitterPosition = this.Position + new Vector2(11, 0).RotateVector(emitterRotation);

        _particleEmitter.SetTransform(emitterPosition, emitterRotation);
    }
}