#region
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

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

        Velocity = Vector2.UnitX.RotateVector(direction) * speed;

        Texture2D spriteSheet;
        int colliderSize;
        int spriteSize;
        int mass;

        switch (size)
        {
        case Sizes.Smallest:
            spriteSheet = AssetManager.Load<Texture2D>("Asteroid1");
            spriteSize = 16;
            colliderSize = 6;
            MaxHP = 12;
            HP = MaxHP;
            ContactDamage = 5;
            mass = 6;

            break;

        case Sizes.Small:
            spriteSheet = AssetManager.Load<Texture2D>("Asteroid2");
            spriteSize = 24;
            colliderSize = 12;
            MaxHP = 24;
            HP = MaxHP;
            ContactDamage = 7;
            mass = 12;

            break;

        case Sizes.Medium:
            spriteSheet = AssetManager.Load<Texture2D>("Asteroid3");
            spriteSize = 32;
            colliderSize = 20;
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

        SpriteRenderer = new SpriteRenderer(spriteSheet, frame, LayerDepth.Foreground);

        Collider = new Collider
        (
            this,
            true,
            mass
        )
        {
            radius = colliderSize
        };

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

                GameState.Entities.Add
                (
                    new Asteroid(GameState, Position, angleToPlayer, _size - 1, _debrisController)
                );
            }
        }

        _hasExploded = true;

        _debrisController.SpawnDebris(Position, (int)_size);
        
        GameState.Player.Multiplier += 0.1F;

        int score = _size switch
        {
            Sizes.Smallest => 100,
            Sizes.Small => 300,
            Sizes.Medium => 700,
            _ => 0
        };
        
        GameState.Root.Score += (int)(score * GameState.Player.Multiplier);
        
        base.OnDeath();
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        base.OnUpdate(sender, e);

        _debrisController._particleEmitter.OnUpdate(sender, e);

        Rotation += _rotSpeed * e.DeltaTime;
        if (Rotation > Math.PI) Rotation = (float)-Math.PI;

        if (Velocity.Length() > 200)
        {
            Velocity = Vector2.Normalize(Velocity) * 200;
        }
    }

    public override void OnCollision(Collider other)
    {
        base.OnCollision(other);

        if (other.Parent is not Bullet) return;
        
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