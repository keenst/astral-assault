#region
using System;
using Microsoft.Xna.Framework;
#endregion

namespace TheGameOfDoomHmmm.Source.Input;

public sealed class MouseMoveEventArgs : EventArgs
{
    public MouseMoveEventArgs(Point position)
    {
        Position = position;
    }

    public Point Position { get; }
}