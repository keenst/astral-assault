using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class Bullet : Entity
{
    private readonly Game1 _root;
    
    public Bullet(Game1 root, Vector2 position, float rotation, float speed) :base(position)
    {
        _root = root;
        
        Velocity = new Vector2(
            (float)Math.Cos(rotation),
            (float)Math.Sin(rotation)
            ) * speed;
        
        Texture2D spriteSheet = new(_root.GraphicsDevice, 2, 2);
        
        Color[] data = new Color[2 * 2];
        for(int i = 0; i < data.Length; ++i) data[i] = Color.White;
        spriteSheet.SetData(data);

        Frame frame = new(new Rectangle(0, 0, 2, 2));

        Animation animation = new(new[] { frame }, false);
        
        SpriteRenderer = new SpriteRenderer(spriteSheet, new[] { animation });
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        Position += Velocity * e.DeltaTime;

        if (Position.X is > Game1.TargetWidth or < 0 ||
            Position.Y is > Game1.TargetHeight or < 0)
        {
            _root.Entities.Remove(this);
        }
    }
}