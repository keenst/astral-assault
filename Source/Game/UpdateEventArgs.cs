#region
using System;
using Microsoft.Xna.Framework;
#endregion

namespace TheGameOfDoomHmmm.Source.Game;

public sealed class UpdateEventArgs : EventArgs
{
    internal UpdateEventArgs(float deltaTime, GameTime gt)
    {
        DeltaTime = deltaTime;
    }

    internal float DeltaTime { get; }
}