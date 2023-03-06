using System;
using Microsoft.Xna.Framework;

namespace astral_assault;

public class MouseMoveEventArgs : EventArgs
{
    public Point Position { get; }

    public MouseMoveEventArgs(Point position)
    {
        Position = position;
    }
}