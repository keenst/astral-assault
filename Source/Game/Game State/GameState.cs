using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public abstract class GameState
{
    public readonly Game1 Root;
    
    public GameState(Game1 root)
    {
        Root = root;
    }
    
    public abstract void Draw(SpriteBatch spriteBatch);
    public abstract void Enter();
    public abstract void Exit();
}