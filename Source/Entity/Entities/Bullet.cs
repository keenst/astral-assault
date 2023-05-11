using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class Bullet : Entity
{
    public Bullet(GameplayState gameState, Vector2 position, float rotation, float speed, BulletType bulletType) 
        :base(gameState, position)
    {
        Velocity = new Vector2(
            (float)Math.Cos(rotation),
            (float)Math.Sin(rotation)
            ) * speed;

        Texture2D spriteSheet = AssetManager.Load<Texture2D>("Bullet");
        
        Frame frame = new(new Rectangle(0, 0, 4, 4));

        SpriteRenderer = new SpriteRenderer(spriteSheet, frame, LayerDepth.Foreground);
        
        Collider = new Collider(
            this, 
            new Rectangle(
                new Point((int)Position.X - 1, (int)Position.Y - 1), 
                new Point(2, 2)));
        GameState.CollisionSystem.AddCollider(Collider);

        OutOfBoundsBehavior = OutOfBounds.Destroy;

        ContactDamage = bulletType switch
        {
            BulletType.Light => 4,
            BulletType.Heavy => 8,
            _ => throw new ArgumentOutOfRangeException(nameof(bulletType), bulletType, null)
        };
        
        IsFriendly = true;
    }

    public override void OnCollision(Collider other)
    {
        if (IsFriendly == other.Parent.IsFriendly) return;
        
        Destroy();
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        base.OnUpdate(sender, e);

        if (Position.X is > Game1.TargetWidth or < 0 ||
            Position.Y is > Game1.TargetHeight or < 0)
        {
            Destroy();
        }
    }
}