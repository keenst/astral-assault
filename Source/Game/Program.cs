#region
using AstralAssault;
#endregion

try
{
    using Game1 game = new();
    game.Run();
}
catch(System.Exception e)
{
    System.IO.File.WriteAllText("log.txt", e.ToString());
}