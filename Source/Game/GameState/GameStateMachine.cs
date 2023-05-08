namespace TheGameOfDoomHmmm.Source.Game.GameState;

public sealed class GameStateMachine
{
    private GameState m_currentState;

    public GameStateMachine(GameState initialState)
    {
        m_currentState = initialState;
        m_currentState.Enter();
    }

    public void ChangeState(GameState newState)
    {
        m_currentState?.Exit();
        m_currentState = newState;
        m_currentState.Enter();
    }

    public void Draw() => m_currentState.Draw();
}