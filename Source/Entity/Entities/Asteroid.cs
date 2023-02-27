using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class Asteroid : Entity
{
    private float _rotSpeed;

    public enum Size
    {
        Smallest,
        Small,
        Medium
    }

    public Asteroid(Game1 root, Vector2 position, Size size) : base(root, position)
    {
        Root = root;

        Random rnd = new();
        _rotSpeed = rnd.Next(5, 20) / 10F;
        Velocity.X = rnd.Next(-100, 100);
        Velocity.Y = rnd.Next(-100, 100);

        Texture2D spriteSheet = default;
        int colliderSize = 0;
        int spriteSize = 0;

        switch (size)
        {
            case Size.Smallest:
                spriteSheet = Root.Content.Load<Texture2D>("assets/asteroid1");
                spriteSize = 16;
                colliderSize = 24;
                break;
            case Size.Small:
                spriteSheet = Root.Content.Load<Texture2D>("assets/asteroid2");
                spriteSize = 24;
                colliderSize = 24;
                break;
            case Size.Medium:
                spriteSheet = Root.Content.Load<Texture2D>("assets/asteroid3");
                spriteSize = 32;
                colliderSize = 24;
                break;
        }
        
        Frame frame = new(
              e:new Rectangle(0,              0, spriteSize, spriteSize),
            see:new Rectangle(spriteSize,     0, spriteSize, spriteSize),
             se:new Rectangle(spriteSize * 2, 0, spriteSize, spriteSize),
            sse:new Rectangle(spriteSize * 3, 0, spriteSize, spriteSize));

        Animation animation = new(new[] { frame }, true);

        SpriteRenderer = new SpriteRenderer(spriteSheet, new[] { animation });
        
        Collider = new Collider(
            this, 
            new Rectangle(
                new Point((int)Position.X - colliderSize / 2, (int)Position.Y - colliderSize / 2), 
                new Point(colliderSize, colliderSize)));
        Root.CollisionSystem.AddCollider(Collider);

        OutOfBoundsBehavior = OutOfBounds.Wrap;
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        base.OnUpdate(sender, e);
        
        Rotation += _rotSpeed * e.DeltaTime;
        if (Rotation > Math.PI) Rotation = (float)-Math.PI;
    }
}