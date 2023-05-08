using System.Collections.Generic;

namespace AstralAssault;

public abstract class GameState
{
    public readonly Game1 Root;

    protected GameState(Game1 root)
    {
        Root = root;
    }
    
    public abstract List<DrawTask> GetDrawTasks();
    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update(UpdateEventArgs e);
}