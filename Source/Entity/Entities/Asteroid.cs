using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class Asteroid : Entity
{
    public Asteroid(Game1 root, Vector2 position) : base(root, position)
    {
        Root = root;

        Texture2D spriteSheet = Root.Content.Load<Texture2D>("assets/asteroid1");

        Frame frame = new(
            new Rectangle(0, 0, 32, 32),
            new Rectangle(32, 0, 32, 32),
            new Rectangle(64, 0, 32, 32),
            new Rectangle(96, 0, 32, 32));

        Animation animation = new(new[] { frame }, true);

        SpriteRenderer = new SpriteRenderer(spriteSheet, new[] { animation });
        
        Collider = new Collider(
            this, 
            new Rectangle(
                new Point((int)Position.X - 12, (int)Position.Y - 12), 
                new Point(24, 24)));
        Root.CollisionSystem.AddCollider(Collider);
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        Rotation += 0.01F;
        if (Rotation > Math.PI * 2) Rotation = 0;
    }
}