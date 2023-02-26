using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace astral_assault;

public class Player : Entity, IInputEventListener
{
    private readonly Game1 _root;
    private Vector2 _velocity;
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

    public Player(Game1 root, Vector2 position) :base(position)
    {
        _root = root;
        _position = position;
        _rotation = Pi / 2;
        
        InitSpriteRenderer();
        
        StartListening();
    }

    private void InitSpriteRenderer()
    {
        Texture2D spriteSheet = _root.Content.Load<Texture2D>("assets/player");
        
        Frame frame = new(
            new Rectangle(0, 0, 32, 32),
            new Rectangle(32, 0, 32, 32),
            new Rectangle(64, 0, 32, 32),
            new Rectangle(96, 0, 32, 32));

        Animation animation = new(new[] { frame }, true);

        _spriteRenderer = new SpriteRenderer(spriteSheet, new[] { animation });
    }

    private void StartListening()
    {
        InputEventSource.KeyboardEvent += OnKeyboardEvent;
        InputEventSource.MouseMoveEvent += OnMouseMoveEvent;
        InputEventSource.MouseButtonEvent += OnMouseButtonEvent;
    }

    private void HandleMovement(int xAxis, int yAxis)
    {
        // acceleration and deceleration
        Vector2 forward = new Vector2(
            (float)Math.Cos(_rotation), 
            (float)Math.Sin(_rotation)
        ) * MoveSpeed * _delta;
        
        _velocity = new Vector2(
            Math.Clamp(_velocity.X + forward.X * yAxis, -MaxSpeed, MaxSpeed),
            Math.Clamp(_velocity.Y + forward.Y * yAxis, -MaxSpeed, MaxSpeed));

        // tilting
        Vector2 right = new Vector2(
            (float)Math.Cos(_rotation + Pi / 2), 
            (float)Math.Sin(_rotation + Pi / 2)
        ) * TiltSpeed * _delta;
        
        _velocity = new Vector2(
            Math.Clamp(_velocity.X + right.X * xAxis, -MaxSpeed, MaxSpeed),
            Math.Clamp(_velocity.Y + right.Y * xAxis, -MaxSpeed, MaxSpeed));
    }

    private void HandleFiring()
    {
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        
        if (_lastTimeFired + ShootSpeed > timeNow) return;

        _lastTimeFired = timeNow;
        
        float xDiff = _cursorPosition.X - (_lastCannon ? _muzzle.Item1.X : _muzzle.Item2.X);
        float yDiff = _cursorPosition.Y - (_lastCannon ? _muzzle.Item1.Y : _muzzle.Item2.Y);

        float rot = (float)Math.Atan2(yDiff, xDiff);
            
        _root.Bullets.Add(
            new Bullet(
                _root, 
                _lastCannon ? _muzzle.Item1 : _muzzle.Item2, 
                rot, 
                BulletSpeed));
            
        _lastCannon = !_lastCannon;
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
        Point scale = new((int)_root.ScaleX, (int)_root.ScaleY);
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
        _delta = e.DeltaTime;

        // rotate player
        float xDiff = _cursorPosition.X - _position.X;
        float yDiff = _cursorPosition.Y - _position.Y;

        _rotation = (float)Math.Atan2(yDiff, xDiff);
        
        // apply player velocity
        _position += _velocity * _delta;

        // apply friction
        float sign = Math.Sign(_velocity.Length());

        if (sign != 0)
        {
            float direction = (float)Math.Atan2(_velocity.Y, _velocity.X);
            
            _velocity -= 
                new Vector2((float)Math.Cos(direction), (float)Math.Sin(direction)) * Friction * _delta * sign;
        }
        
        // wrap position
        _position.X = _position.X switch
        {
            < -16 => Game1.TargetWidth - 16,
            > Game1.TargetWidth + 16 => 16,
            _ => _position.X
        };

        _position.Y = _position.Y switch
        {
            < -16 => Game1.TargetHeight - 16,
            > Game1.TargetHeight + 16 => 16,
            _ => _position.Y
        };
        
        // rotate the points for the cannon muzzles
        Vector2 muzzle1;
        Vector2 muzzle2;
        
        const float x =  8;
        const float y = 10;

        {
            float rot = Pi / 8 * (float)Math.Round(_rotation / (Pi / 8));

            float x2 = (float)(x * Math.Cos(rot) - y * Math.Sin(rot));
            float y2 = (float)(y * Math.Cos(rot) + x * Math.Sin(rot));

            muzzle1 = new Vector2(_position.X + x2, _position.Y + y2);
        }

        {
            float rot = Pi / 8 * (float)Math.Round(_rotation / (Pi / 8));

            float x2 = (float)(x * Math.Cos(rot) + y * Math.Sin(rot));
            float y2 = (float)(-y * Math.Cos(rot) + x * Math.Sin(rot));

            muzzle2 = new Vector2(_position.X + x2, _position.Y + y2);
        }

        _muzzle = new Tuple<Vector2, Vector2>(muzzle1, muzzle2);
    }
}