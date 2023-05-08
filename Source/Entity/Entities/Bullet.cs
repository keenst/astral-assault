using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class Bullet : Entity
{
    public Bullet(GameplayState gameState, Vector2 position, float rotation, float speed) :base(gameState, position)
    {
        Velocity = new Vector2(
            (float)Math.Cos(rotation),
            (float)Math.Sin(rotation)
            ) * speed;
        
        Texture2D spriteSheet = new(GameState.Root.GraphicsDevice, 2, 2);
        
        Color[] data = new Color[2 * 2];
        for(int i = 0; i < data.Length; ++i) data[i] = Color.White;
        spriteSheet.SetData(data);

        Frame frame = new(new Rectangle(0, 0, 2, 2));

        SpriteRenderer = new SpriteRenderer(spriteSheet, frame, LayerDepth.Foreground);
        
        Collider = new Collider(
            this, 
            new Rectangle(
                new Point((int)Position.X - 1, (int)Position.Y - 1), 
                new Point(2, 2)));
        GameState.CollisionSystem.AddCollider(Collider);

        OutOfBoundsBehavior = OutOfBounds.Destroy;

        ContactDamage = 4;
        IsFriendly = true;
    }

    public override void OnCollision(Collider other)
    {
        if (IsFriendly == other.Parent.IsFriendly) return;
        
        Destroy();
    }

    public override void Update(UpdateEventArgs e)
    {
        base.Update(e);

        if (Position.X is > Game1.TargetWidth or < 0 ||
            Position.Y is > Game1.TargetHeight or < 0)
        {
            Destroy();
        }
    }
}