namespace TheGameOfDoomHmmm.Source.Game.GameState;

public abstract class GameState : IUpdateEventListener
{
    public readonly Game1 Root;

    public GameState(Game1 root)
    {
        Root = root;
    }

    public abstract void OnUpdate(object sender, UpdateEventArgs e);

    public abstract void Draw();
    public abstract void Enter();
    public abstract void Exit();
}