using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class Sprite
{
    private readonly Texture2D _texture;
    private readonly Vector2 _origin;
    private readonly int _width;
    private readonly int _height;

    public Sprite(Texture2D texture, Vector2 origin = new())
    {
        _texture = texture;
        origin = new Vector2(texture.Width / 2F, texture.Height / 2F);
        _origin = origin;
        _width = texture.Width;
        _height = texture.Height;
    }

    public void Draw(
        SpriteBatch spriteBatch, 
        Vector2 position, 
        float rotation = 0, 
        bool wrap = false)
    {
        Rectangle rectangle = new(position.ToPoint(), new Point(_width, _height));
        
        spriteBatch.Draw(
            _texture, 
            rectangle, 
            null, 
            Color.White, 
            rotation, 
            _origin, 
            SpriteEffects.None, 
            0);

        if (!wrap) return;

        // wrap the sprite
        int half = _width / 2;
        
        if (position.X < half || position.X + half > Game1.TargetWidth)
        {
            float x = position.X + (position.X < half ? Game1.TargetWidth : -Game1.TargetWidth);
            float y = position.Y;
            
            rectangle = new Rectangle(new Vector2(x, y).ToPoint(), new Point(_width, _height));
            
            spriteBatch.Draw(
                _texture,
                rectangle,
                null,
                Color.White,
                rotation,
                _origin,
                SpriteEffects.None,
                0);
        }
        
        if (position.Y < half || position.Y + half > Game1.TargetHeight)
        {
            float x = position.X;
            float y = position.Y + (position.Y < half ? Game1.TargetHeight : -Game1.TargetHeight);
            
            rectangle = new Rectangle(new Vector2(x, y).ToPoint(), new Point(_width, _height));
            
            spriteBatch.Draw(
                _texture,
                rectangle,
                null,
                Color.White,
                rotation,
                _origin,
                SpriteEffects.None,
                0);
        }
    }
}