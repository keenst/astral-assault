#region
using System;
using System.IO;
using TheGameOfDoomHmmm.Source.Game;
#endregion

try
{
    using Game1 game = new Game1();
    game.Run();
}
catch (Exception e)
{
    File.WriteAllText("log.txt", e.ToString());
}