using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class Bullet
{
    private readonly Game1 _root;
    private readonly Vector2 _velocity;
    
    public Vector2 Position;

    public Bullet(Game1 root, Vector2 position, float rotation, float speed)
    {
        _root = root;
        
        Position = position;

        _velocity = new Vector2(
            (float)Math.Cos(rotation),
            (float)Math.Sin(rotation)
            ) * speed;
        
        LoadContent();
    }

    private void LoadContent()
    {
        
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Position += _velocity * delta;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Texture2D rect = new(_root.GraphicsDevice, 2, 2);
        
        Color[] data = new Color[2 * 2];
        for(int i = 0; i < data.Length; ++i) data[i] = Color.White;
        rect.SetData(data);
        
        spriteBatch.Draw(rect, Position, Color.Green);
    }
}