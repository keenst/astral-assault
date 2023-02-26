using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class Asteroid : Entity
{
    private Game1 _root;
    
    public Asteroid(Game1 root, Vector2 position) : base(position)
    {
        _root = root;

        Texture2D spriteSheet = _root.Content.Load<Texture2D>("assets/asteroid1");

        Frame frame = new(
            new Rectangle(0, 0, 32, 32),
            new Rectangle(32, 0, 32, 32),
            new Rectangle(64, 0, 32, 32),
            new Rectangle(96, 0, 32, 32));

        Animation animation = new(new[] { frame }, true);

        SpriteRenderer = new SpriteRenderer(spriteSheet, new[] { animation });
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        Rotation += 0.01F;
        if (Rotation > Math.PI * 2) Rotation = 0;
    }
}