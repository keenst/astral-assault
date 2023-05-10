namespace TheGameOfDoomHmmm.Source.Game.GameState;

public abstract class GameState : IUpdateEventListener
{
    internal readonly Game1 Root;

    protected GameState(Game1 root)
    {
        Root = root;
    }

    public abstract void OnUpdate(object sender, UpdateEventArgs e);

    internal abstract void Draw();
    internal abstract void Enter();
    internal abstract void Exit();
}