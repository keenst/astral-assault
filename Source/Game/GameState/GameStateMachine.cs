namespace TheGameOfDoomHmmm.Source.Game.GameState;

public sealed class GameStateMachine
{
    private GameState m_currentState;

    internal GameStateMachine(GameState initialState)
    {
        m_currentState = initialState;
        m_currentState.Enter();
    }

    internal void ChangeState(GameState newState)
    {
        m_currentState?.Exit();
        m_currentState = newState;
        m_currentState.Enter();
    }

    internal void Draw() => m_currentState.Draw();
}