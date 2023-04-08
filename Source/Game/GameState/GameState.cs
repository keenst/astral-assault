using System.Collections.Generic;

namespace AstralAssault;

public abstract class GameState : IUpdateEventListener
{
    public readonly Game1 Root;

    public GameState(Game1 root)
    {
        Root = root;
    }

    public abstract List<DrawTask> GetDrawTasks();
    public abstract void Enter();
    public abstract void Exit();
    public abstract void OnUpdate(object sender, UpdateEventArgs e);
}