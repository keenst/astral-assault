#region
using System;
using Microsoft.Xna.Framework;
#endregion

namespace TheGameOfDoomHmmm.Source.Input;

public sealed class MouseMoveEventArgs : EventArgs
{
    internal MouseMoveEventArgs(Point position)
    {
        Position = position;
    }

    internal Point Position { get; }
}