using System.Collections.Generic;

namespace AstralAssault;

public class GameStateMachine
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

    public List<DrawTask> GetDrawTasks() => m_currentState.GetDrawTasks();
}