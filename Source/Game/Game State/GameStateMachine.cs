using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class GameStateMachine
{
    private GameState _currentState;

    public GameStateMachine(GameState initialState)
    {
        _currentState = initialState;
        _currentState.Enter();
    }

    public void ChangeState(GameState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        _currentState.Draw(spriteBatch);
    }
}