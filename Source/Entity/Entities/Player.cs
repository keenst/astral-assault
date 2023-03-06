using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace astral_assault;

public class Player : Entity, IInputEventListener
{
    private Point _cursorPosition;
    private Tuple<Vector2, Vector2> _muzzle;
    private bool _lastCannon;
    private long _lastTimeFired;
    private float _delta;

    private const float MoveSpeed = 100;
    private const float MaxSpeed = 100;
    private const float TiltSpeed = 80;
    private const float Friction = 30;
    private const float Pi = 3.14F;
    private const float BulletSpeed = 250;
    private const int   ShootSpeed = 200;

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
                new Point(24, 24)));
        _gameState.CollisionSystem.AddCollider(Collider);

        OutOfBoundsBehavior = OutOfBounds.Wrap;

        IsActor = true;
        MaxHP = 50;
        HP = MaxHP;
        IsFriendly = true;
    }

    private void InitSpriteRenderer()
    {
        Texture2D spriteSheet = _gameState.Root.Content.Load<Texture2D>("assets/player");
        
        Frame frame = new(
            new Rectangle(0, 0, 32, 32),
            new Rectangle(32, 0, 32, 32),
            new Rectangle(64, 0, 32, 32),
            new Rectangle(96, 0, 32, 32));

        Animation animation = new(new[] { frame }, true);

        SpriteRenderer = new SpriteRenderer(spriteSheet, new[] { animation });
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
    }

    private void HandleFiring()
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        
        if (_lastTimeFired + ShootSpeed > timeNow) return;

        _lastTimeFired = timeNow;
        
        float xDiff = _cursorPosition.X - (_lastCannon ? _muzzle.Item1.X : _muzzle.Item2.X);
        float yDiff = _cursorPosition.Y - (_lastCannon ? _muzzle.Item1.Y : _muzzle.Item2.Y);

        float rot = (float)Math.Atan2(yDiff, xDiff);
            
        _gameState.Entities.Add(
            new Bullet(
                _gameState, 
                _lastCannon ? _muzzle.Item1 : _muzzle.Item2, 
                rot, 
                BulletSpeed));
            
        _lastCannon = !_lastCannon;
    }

    public override void OnCollision(Collider other)
    {
        base.OnCollision(other);

        if (IsFriendly == other.Parent.IsFriendly) return;
        
        Vector2 direction = Position - other.Parent.Position;
        direction.Normalize();

        Velocity = direction * 50;
    }

    public override void Destroy()
    {
        StopListening();
        
        base.Destroy();
    }
    
    protected override void OnDeath()
    {
        Game1 root = _gameState.Root;
        
        root.GameStateMachine.ChangeState(new GameOverState(root));
        
        base.OnDeath();
    }

    public void OnKeyboardEvent(object sender, KeyboardEventArgs e)
    {
        int xAxis = e.Key switch
        {
            Keys.D => 1,
            Keys.A => -1,
            _ => 0
        };
        
        int yAxis = e.Key switch
        {
            Keys.W => 1,
            Keys.S => -1,
            _ => 0
        };
        
        HandleMovement(xAxis, yAxis);
    }

    public void OnMouseMoveEvent(object sender, MouseMoveEventArgs e)
    {
        Point scale = new((int)_gameState.Root.ScaleX, (int)_gameState.Root.ScaleY);
        _cursorPosition = e.Position / scale;
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

        // rotate player
        float xDiff = _cursorPosition.X - Position.X;
        float yDiff = _cursorPosition.Y - Position.Y;

        Rotation = (float)Math.Atan2(yDiff, xDiff);

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
    }
}