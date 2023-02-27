using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

        OutOfBoundsBehavior = OutOfBounds.Wrap;
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        base.OnUpdate(sender, e);
        
        Rotation += 1F * e.DeltaTime;
        if (Rotation > Math.PI) Rotation = (float)-Math.PI;
    }
}