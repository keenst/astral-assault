using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class Missile : Entity
{
    private const float RotationSpeed = 1F;
    private const float Speed = 110F;
    
    public Missile(GameplayState gameState, Vector2 position) : base(gameState, position)
    {
        MaxHP = 30;
        HP = MaxHP;
        
        Rotation = GetRotationToPlayer();
        Velocity = new Vector2(MathF.Cos(Rotation), MathF.Sin(Rotation)) * Speed;

        Texture2D spriteSheet = AssetManager.Load<Texture2D>("Missile");

        Frame frame = new(
            new Rectangle(0, 0, 16, 16),
            new Rectangle(16, 0, 16, 16),
            new Rectangle(32, 0, 16, 16),
            new Rectangle(48, 0, 16, 16));
        
        SpriteRenderer = new SpriteRenderer(spriteSheet, frame, LayerDepth.Foreground);

        Collider = new Collider(
            this,
            new Rectangle((int)Position.X - 6, (int)Position.Y - 6, 12, 12),
            true,
            6);
        GameState.CollisionSystem.AddCollider(Collider);
        
        OutOfBoundsBehavior = OutOfBounds.Wrap;

        IsActor = true;

        HealthBarYOffset = 16;

        ContactDamage = 40;
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        base.OnUpdate(sender, e);

        float rotationToPlayer = GetRotationToPlayer();
        float rotationDiff = rotationToPlayer - Rotation;

        int diffSign = Math.Sign(rotationDiff);
        
        float toRotate = Math.Min(Math.Abs(rotationDiff), RotationSpeed * e.DeltaTime);
        
        Rotation += toRotate * diffSign;
        
        Velocity = new Vector2(MathF.Cos(Rotation), MathF.Sin(Rotation)) * Speed;
    }

    public override void OnCollision(Collider other)
    {
        base.OnCollision(other);
        
        switch (other.Parent)
        {
            case Player:
            {
                OnDeath();
                break;
            }
            case Bullet:
            {
                Random rnd = new();
                string soundName = "Hurt" + rnd.Next(1, 4);
                
                Jukebox.PlaySound(soundName, 0.5F);
                break;
            }
        }
    }

    protected override void OnDeath()
    {
        GameState.ExplosionController.SpawnExplosion(Position);
        
        Random rnd = new();
        string soundName = "Explosion" + rnd.Next(1, 4);
        
        Jukebox.PlaySound(soundName, 0.5F);
        
        base.OnDeath();
    }

    private float GetRotationToPlayer()
    {
        Player player = GameState.Player;

        if (player is null) return Rotation;
        
        float xDiff = player.Position.X - Position.X;
        float yDiff = player.Position.Y - Position.Y;
        
        return MathF.Atan2(yDiff, xDiff);
    }
}