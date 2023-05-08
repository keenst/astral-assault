using System;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public class MouseMoveEventArgs : EventArgs
{
    public Point Position { get; }

    public MouseMoveEventArgs(Point position)
    {
        Position = position;
    }
}