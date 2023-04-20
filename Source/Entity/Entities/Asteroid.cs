using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class Asteroid : Entity
{
    private readonly DebrisController _debrisController;
    private readonly float _rotSpeed;
    private readonly Sizes _size;
    private bool _hasExploded;

    public enum Sizes
    {
        Smallest,
        Small,
        Medium
    }

    public Asteroid(
        GameplayState gameState, 
        Vector2 position, 
        float direction, 
        Sizes size, 
        DebrisController debrisController) 
        :base(gameState, position)
    {
        _debrisController = debrisController;
        
        _size = size;

        Random rnd = new();
        _rotSpeed = rnd.Next(5, 20) / 10F;
        int speed = rnd.Next(30, 100);

        Velocity = new Vector2((float)Math.Cos(direction), (float)Math.Sin(direction)) * speed;
        
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
        
        Frame frame = new(
              e:new Rectangle(0,              0, spriteSize, spriteSize),
            see:new Rectangle(spriteSize,     0, spriteSize, spriteSize),
             se:new Rectangle(spriteSize * 2, 0, spriteSize, spriteSize),
            sse:new Rectangle(spriteSize * 3, 0, spriteSize, spriteSize));

        SpriteRenderer = new SpriteRenderer(spriteSheet, frame, LayerDepth.Foreground);
        
        Collider = new Collider(
            this, 
            new Rectangle(
                new Point((int)Position.X - colliderSize / 2, (int)Position.Y - colliderSize / 2), 
                new Point(colliderSize, colliderSize)),
            true,
            mass);
        GameState.CollisionSystem.AddCollider(Collider);

        OutOfBoundsBehavior = OutOfBounds.Wrap;

        IsActor = true;
    }

    protected override void OnDeath()
    {
        if (!_hasExploded)
        {
            Random rnd = new();

            string soundToPlay = rnd.Next(3) switch
            {
                0 => "Explosion1",
                1 => "Explosion2",
                2 => "Explosion3",
                _ => throw new ArgumentOutOfRangeException()
            };
                
            Jukebox.PlaySound(soundToPlay);

            if (_size - 1 < 0)
            {
                _hasExploded = true;
                return;
            }
            
            int amount = rnd.Next(1, 4);

            Vector2 playerPosition = GameState.Player.Position;
            float angleToPlayer = MathF.Atan2(Position.Y - playerPosition.Y, Position.X - playerPosition.X);

            for (int i = 0; i < amount; i++)
            {
                angleToPlayer += (float)rnd.NextDouble() * MathF.PI / 1 - MathF.PI / 2;
                
                GameState.Entities.Add(
                    new Asteroid(GameState, Position, angleToPlayer, _size - 1, _debrisController));
            }
        }
        
        _hasExploded = true;

        _debrisController.SpawnDebris(Position, (int)_size);
        
        GameState.Root.Score += _size switch
        {
            Sizes.Smallest => 100,
            Sizes.Small => 300,
            Sizes.Medium => 700,
            _ => 0
        };
        
        base.OnDeath();
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        base.OnUpdate(sender, e);
        
        Rotation += _rotSpeed * e.DeltaTime;
        if (Rotation > Math.PI) Rotation = (float)-Math.PI;
    }
}