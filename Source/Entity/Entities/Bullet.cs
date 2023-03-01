using System;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class Bullet : Entity
{
    public Bullet(Game1 root, Vector2 position, float rotation, float speed) :base(root, position)
    {
        Root = root;
        
        Velocity = new Vector2(
            (float)Math.Cos(rotation),
            (float)Math.Sin(rotation)
            ) * speed;
        
        Texture2D spriteSheet = new(Root.GraphicsDevice, 2, 2);
        
        Color[] data = new Color[2 * 2];
        for(int i = 0; i < data.Length; ++i) data[i] = Color.White;
        spriteSheet.SetData(data);

        Frame frame = new(new Rectangle(0, 0, 2, 2));

        Animation animation = new(new[] { frame }, false);
        
        SpriteRenderer = new SpriteRenderer(spriteSheet, new[] { animation });
        
        Collider = new Collider(
            this, 
            new Rectangle(
                new Point((int)Position.X - 1, (int)Position.Y - 1), 
                new Point(2, 2)));
        Root.CollisionSystem.AddCollider(Collider);

        OutOfBoundsBehavior = OutOfBounds.Destroy;

        ContactDamage = 4;
        IsFriendly = true;
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