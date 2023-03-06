using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class GameOverState : GameState
{
    private Texture2D _gameOverText;
    
    public GameOverState(Game1 root) : base(root)
    {
        
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        if (_gameOverText == null) return;
        spriteBatch.Draw(_gameOverText, new Vector2(0, 0), Color.White);
    }

    public override void Enter()
    {
        _gameOverText = Root.Content.Load<Texture2D>("assets/game over");
    }

    public override void Exit()
    {
        
    }
}