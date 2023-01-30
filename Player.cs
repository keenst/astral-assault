using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class Player
{
    private Game1 _root;
    private Texture2D _sprite;
    private Vector2 _position;

    public Player(Game1 root, Vector2 position)
    {
        _root = root;
        _position = position;
        
        LoadContent();
    }

    private void LoadContent()
    {
        _sprite = _root.Content.Load<Texture2D>("assets/player1");
    }

    public void Update(GameTime gameTime)
    {
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_sprite, _position, Color.White);
    }
}