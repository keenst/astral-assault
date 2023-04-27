using System.Collections.Generic;

namespace AstralAssault;

public class GameStateMachine
{
    public GameState CurrentState { get; private set; }

    public GameStateMachine(GameState initialState)
    {
        CurrentState = initialState;
        CurrentState.Enter();
    }

    public void ChangeState(GameState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
    
    public List<DrawTask> GetDrawTasks()
    {
        return CurrentState.GetDrawTasks();
    }
}