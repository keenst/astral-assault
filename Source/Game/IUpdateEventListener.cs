namespace TheGameOfDoomHmmm.Source.Game;

internal interface IUpdateEventListener
{
    void OnUpdate(object sender, UpdateEventArgs e);
}