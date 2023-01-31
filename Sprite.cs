using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class Sprite
{
    private readonly Texture2D _texture;
    private readonly Vector2 _origin;

    public Sprite(Texture2D texture, Vector2 origin = new())
    {
        _texture = texture;
        origin = new Vector2(texture.Width / 2F, texture.Height / 2F);
        _origin = origin;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation = 0)
    {
        Rectangle rectangle = new(
            position.ToPoint(), 
            new Vector2(_texture.Width, _texture.Height).ToPoint());
        
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