using System;
using System.Collections.Generic;
using System.Linq;
using AstralAssault.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace AstralAssault;

public enum PowerUps
{
    QuadDamage,
    Haste
}

public class Player : Entity, IInputEventListener
{
    public float Multiplier = 1;
    
    private Vector2 _cursorPosition;
    private Tuple<Vector2, Vector2> _muzzle = new(Vector2.Zero, Vector2.Zero);
    private bool _lastCannon;
    private bool _isCrosshairActive = true;
    private bool _thrusterIsOn;
    private long _lastTimeFired;
    private float _delta;
    private readonly ParticleEmitter _particleEmitter;
    private readonly List<Tuple<long, PowerUps>> _powerUps = new(); // (time of pick up, power up)

    private float _moveSpeed = 200;
    private float _maxSpeed = 100;
    private float _tiltSpeed = 200;
    private const float Friction = 30;
    private const float Pi = 3.14F;
    private const float BulletSpeed = 250;
    private int _shootSpeed = 200;
    
    private const int PowerUpDuration = 10 * 1000;
    private Texture2D _square;

    public Player(GameplayState gameState, Vector2 position) : base(gameState, position)
    {
        _square = new Texture2D(GameState.Root.GraphicsDevice, 1, 1);
        _square.SetData(new[] { Color.White });
        
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
            radius = 10
        };

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

        _particleEmitter = new ParticleEmitter
        (
            particleSpriteSheet,
            textureSources,
            20,
            Position,
            Rotation,
            particleProperties,
            LayerDepth.ThrusterFlame
        );

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
                new Frame(
                    new Rectangle(352, 0,  32, 32),
                    new Rectangle(352, 32, 32, 32),
                    new Rectangle(352, 64, 32, 32),
                    new Rectangle(352, 96, 32, 32))
            },
            true);

        Animation tiltLeftAnimation = new Animation
        (
            new[]
            {
                new Frame(
                    new Rectangle(32, 0,  32, 32),
                    new Rectangle(32, 32, 32, 32),
                    new Rectangle(32, 64, 32, 32),
                    new Rectangle(32, 96, 32, 32))
            },
            true);

        Transition[] transitions =
        {
            new(1, 0, new[] { 0 },    "Tilt",  0),
            new(0, 1, new[] { 1 },    "Tilt",  1),
            new(0, 2, new[] { 2 },    "Tilt", -1),
            new(2, 0, new[] { 0 },    "Tilt",  0),
            new(2, 1, new[] { 0, 1 }, "Tilt",  1),
            new(1, 2, new[] { 0, 2 }, "Tilt", -1)
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

        if (_thrusterIsOn) _particleEmitter.StartSpawning();
        else _particleEmitter.StopSpawning();

        drawTasks.AddRange(_particleEmitter.CreateDrawTasks());
        drawTasks.AddRange(base.GetDrawTasks());
        
        _thrusterIsOn = false;
        
        for (int i = 0; i < _powerUps.Count; i++)
        {
            Tuple<long, PowerUps> powerUp = _powerUps[i];
            
            long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            if (timeNow - powerUp.Item1 > PowerUpDuration)
            {
                _powerUps.Remove(powerUp);
                i--;
                continue;
            }

            string powerUpName = powerUp.Item2 switch
            {
                PowerUps.QuadDamage => "quad damage",
                PowerUps.Haste => "haste",
                _ => throw new ArgumentOutOfRangeException()
            };
            
            Vector4 color = powerUp.Item2 switch
            {
                PowerUps.QuadDamage => Palette.GetColorVector(Palette.Colors.Purple6),
                PowerUps.Haste => Palette.GetColorVector(Palette.Colors.Red8),
                _ => throw new ArgumentOutOfRangeException()
            };

            Vector4 backgroundColor = Palette.GetColorVector(Palette.Colors.Black);
            Vector4 barColor = ((timeNow - powerUp.Item1) / (float)PowerUpDuration) switch
            {
                < 0.25F => Palette.GetColorVector(Palette.Colors.Green7),
                < 0.5F => Palette.GetColorVector(Palette.Colors.Green4),
                < 0.75F => Palette.GetColorVector(Palette.Colors.Red8),
                < 1 => Palette.GetColorVector(Palette.Colors.Red4),
                _ => Palette.GetColorVector(Palette.Colors.Black)
            };

            DrawTask lifetimeBackground = new(
                _square,
                new Rectangle(0, 0, 1, 1),
                new Rectangle(1, 28 + i * 12, 2, 8),
                0,
                LayerDepth.HUD,
                new List<IDrawTaskEffect> { new ColorEffect(backgroundColor) },
                Color.White);

            //int barLength = (int)((timeNow - powerUp.Item1) / (float)PowerUpDuration * 2);
            int barLength = 8 - (int)Math.Floor((timeNow - powerUp.Item1) / (float)PowerUpDuration * 8);
            
            DrawTask lifetimeBar = new(
                _square,
                new Rectangle(0, 0, 1, 1),
                new Rectangle(1, 36 + i * 12 - barLength, 2, barLength),
                0,
                LayerDepth.HUD,
                new List<IDrawTaskEffect> { new ColorEffect(barColor) },
                Color.White);

            List<DrawTask> powerUpTask = powerUpName.CreateDrawTasks(
                new Vector2(4, 28 + i * 12),
                Color.White,
                LayerDepth.HUD);
            
            foreach (DrawTask task in powerUpTask)
                task.EffectContainer.SetEffect<ColorEffect, Vector4>(color);

            drawTasks.AddRange(powerUpTask);
            drawTasks.Add(lifetimeBackground);
            drawTasks.Add(lifetimeBar);
        }

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
        Vector2 forward = new Vector2(
            (float)Math.Cos(Rotation), 
            (float)Math.Sin(Rotation)
        ) * _moveSpeed * _delta;
        
        Velocity = new Vector2(
            Math.Clamp(Velocity.X + forward.X * yAxis, -_maxSpeed, _maxSpeed),
            Math.Clamp(Velocity.Y + forward.Y * yAxis, -_maxSpeed, _maxSpeed));

        // tilting
        Vector2 right = new Vector2(
            (float)Math.Cos(Rotation + Pi / 2), 
            (float)Math.Sin(Rotation + Pi / 2)
        ) * _tiltSpeed * _delta;
        
        Velocity = new Vector2(
            Math.Clamp(Velocity.X + right.X * xAxis, -_maxSpeed, _maxSpeed),
            Math.Clamp(Velocity.Y + right.Y * xAxis, -_maxSpeed, _maxSpeed));

        if (Velocity.Length() > _maxSpeed)
        {
            Velocity.Normalize();
            Velocity *= _maxSpeed;
        }
        else if (Velocity.Length() < -_maxSpeed)
        {
            Velocity.Normalize();
            Velocity *= -_maxSpeed;
        }
    }

    private void HandleFiring()
    {
        if (!_isCrosshairActive) return;
        
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        
        if (_lastTimeFired + _shootSpeed > timeNow) return;

        Random rnd = new();
        string soundName = 
            (_powerUps.Any(t => t.Item2 is PowerUps.QuadDamage) ? "Quad" : "") + 
            "Shoot" +
            rnd.Next(1, 4);
        
        Jukebox.PlaySound(soundName, 0.5F);
        
        _lastTimeFired = timeNow;
        
        float xDiff = _cursorPosition.X - (_lastCannon ? _muzzle.Item1.X : _muzzle.Item2.X);
        float yDiff = _cursorPosition.Y - (_lastCannon ? _muzzle.Item1.Y : _muzzle.Item2.Y);

        float rot = (float)Math.Atan2(yDiff, xDiff);
            
        GameState.Entities.Add(
            new Bullet(
                GameState, 
                _lastCannon ? _muzzle.Item1 : _muzzle.Item2, 
                rot, 
                BulletSpeed,
                _powerUps.Any(t => t.Item2 == PowerUps.QuadDamage)));
            
        _lastCannon = !_lastCannon;
    }

    public override void OnCollision(Collider other)
    {
        base.OnCollision(other);

        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        
        if (other.Parent is Quad)
        {
            Jukebox.PlaySound("PickUp");
            
            if (_powerUps.Any(t => t.Item2 == PowerUps.QuadDamage))
            {
                _powerUps.RemoveAll(t => t.Item2 == PowerUps.QuadDamage);
            }
            _powerUps.Add(new Tuple<long, PowerUps>(timeNow, PowerUps.QuadDamage));
        }
        else if (other.Parent is Haste)
        {
            Jukebox.PlaySound("PickUp");
            
            if (_powerUps.Any(t => t.Item2 == PowerUps.Haste))
            {
                _powerUps.RemoveAll(t => t.Item2 == PowerUps.Haste);
            }
            _powerUps.Add(new Tuple<long, PowerUps>(timeNow, PowerUps.Haste));
        }
        else if (other.Parent is MegaHealth)
        {
            Jukebox.PlaySound("PickUp");
            
            HP = Math.Min(MaxHP, HP + 30);
        }
        else if (other.Parent is Asteroid)
        {
            if (Multiplier > 1)
            {
                Jukebox.PlaySound("MultiplierBroken");
            }
            
            Multiplier = 1;

            Random rnd = new();
            
            string soundName = rnd.Next(3) switch
            {
                0 => "Hurt1",
                1 => "Hurt2",
                2 => "Hurt3",
                _ => throw new ArgumentOutOfRangeException()
            };
        
            Jukebox.PlaySound(soundName, 0.5F);
        }
    }

    public override void Destroy()
    {
        StopListening();

        base.Destroy();
    }

    protected override void OnDeath()
    {
        Game1 root = GameState.Root;
        
        Jukebox.PlaySound("GameOver");
        
        root.GameStateMachine.ChangeState(new GameOverState(root));

        base.OnDeath();
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
        else
        {
            SpriteRenderer.SetAnimationCondition("Tilt", 0);
        }

        if (e.Keys.Contains(Keys.W))
        {
            yAxis = 1;
            _thrusterIsOn = true;
        }
        else if (e.Keys.Contains(Keys.S))
        {
            yAxis = -1;
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

        if (_powerUps.Any(t => t.Item2 == PowerUps.Haste))
        {
            _shootSpeed = 100;
            _maxSpeed = 150;
            _moveSpeed = 400;
            _tiltSpeed = 400;
        }
        else
        {
            _shootSpeed = 200;
            _maxSpeed = 100;
            _moveSpeed = 200;
            _tiltSpeed = 200;
        }
        
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
                Vector2.UnitX.RotateVector(direction) * Friction * _delta * sign;
        }

        // rotate the points for the cannon muzzles
        float rot = Pi / 8 * (float)Math.Round(Rotation / (Pi / 8));

        _muzzle = new Tuple<Vector2, Vector2>
        (
            Position + new Vector2(10, -8).RotateVector(rot),
            Position + new Vector2(8, 10).RotateVector(rot)
        );

        float emitterRotation = (Rotation + Pi) % (2 * Pi);
        Vector2 emitterPosition = Position + new Vector2(11, 0).RotateVector(emitterRotation);

        _particleEmitter.SetTransform(emitterPosition, emitterRotation);
    }
}