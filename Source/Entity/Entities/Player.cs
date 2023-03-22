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

    public Player(GameplayState gameState, Vector2 position) :base(gameState, position)
    {
        Position = position;
        Rotation = Pi / 2;

        InitSpriteRenderer();
        
        StartListening();
        
        Collider = new Collider(
            this, 
            new Rectangle(
                new Point((int)Position.X - 12, (int)Position.Y - 12), 
                new Point(24, 24)),
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

        _particleEmitter = new ParticleEmitter(
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
            new Rectangle(0, 0, 32, 32),
            new Rectangle(32, 0, 32, 32),
            new Rectangle(64, 0, 32, 32),
            new Rectangle(96, 0, 32, 32));

        Animation animation = new(new[] { frame }, true);

        SpriteRenderer = new SpriteRenderer(spriteSheet, new[] { animation }, LayerDepth.Foreground);
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
        _particleEmitter.StopListening();
    }

    private void HandleMovement(int xAxis, int yAxis)
    {
        // acceleration and deceleration
        Vector2 forward = new Vector2(
            (float)Math.Cos(Rotation), 
            (float)Math.Sin(Rotation)
        ) * MoveSpeed * _delta;
        
        Velocity = new Vector2(
            Math.Clamp(Velocity.X + forward.X * yAxis, -MaxSpeed, MaxSpeed),
            Math.Clamp(Velocity.Y + forward.Y * yAxis, -MaxSpeed, MaxSpeed));

        // tilting
        Vector2 right = new Vector2(
            (float)Math.Cos(Rotation + Pi / 2), 
            (float)Math.Sin(Rotation + Pi / 2)
        ) * TiltSpeed * _delta;
        
        Velocity = new Vector2(
            Math.Clamp(Velocity.X + right.X * xAxis, -MaxSpeed, MaxSpeed),
            Math.Clamp(Velocity.Y + right.Y * xAxis, -MaxSpeed, MaxSpeed));

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
        if (!_isCrosshairActive) return;
        
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        
        if (_lastTimeFired + ShootSpeed > timeNow) return;

        _lastTimeFired = timeNow;
        
        float xDiff = _cursorPosition.X - (_lastCannon ? _muzzle.Item1.X : _muzzle.Item2.X);
        float yDiff = _cursorPosition.Y - (_lastCannon ? _muzzle.Item1.Y : _muzzle.Item2.Y);

        float rot = (float)Math.Atan2(yDiff, xDiff);
            
        GameState.Entities.Add(
            new Bullet(
                GameState, 
                _lastCannon ? _muzzle.Item1 : _muzzle.Item2, 
                rot, 
                BulletSpeed));
            
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
                new Vector2((float)Math.Cos(direction), (float)Math.Sin(direction)) * Friction * _delta * sign;
        }

        // rotate the points for the cannon muzzles
        Vector2 muzzle1;
        Vector2 muzzle2;
        
        const float x =  8;
        const float y = 10;

        {
            float rot = Pi / 8 * (float)Math.Round(Rotation / (Pi / 8));

            float x2 = (float)(x * Math.Cos(rot) - y * Math.Sin(rot));
            float y2 = (float)(y * Math.Cos(rot) + x * Math.Sin(rot));

            muzzle1 = new Vector2(Position.X + x2, Position.Y + y2);
        }

        {
            float rot = Pi / 8 * (float)Math.Round(Rotation / (Pi / 8));

            float x2 = (float)(x * Math.Cos(rot) + y * Math.Sin(rot));
            float y2 = (float)(-y * Math.Cos(rot) + x * Math.Sin(rot));

            muzzle2 = new Vector2(Position.X + x2, Position.Y + y2);
        }

        _muzzle = new Tuple<Vector2, Vector2>(muzzle1, muzzle2);

        float emitterRotation = (Rotation + Pi) % (2 * Pi);
        Vector2 emitterPosition = new(11, 0);

        {
            float x2 = 
                (float)(emitterPosition.X * Math.Cos(emitterRotation) + emitterPosition.Y * Math.Sin(emitterRotation));
            float y2 = 
                (float)(emitterPosition.Y * Math.Cos(emitterRotation) + emitterPosition.X * Math.Sin(emitterRotation));

            emitterPosition = new Vector2(Position.X + x2, Position.Y + y2);
        }
        
        _particleEmitter.SetTransform(emitterPosition, emitterRotation);
    }
}