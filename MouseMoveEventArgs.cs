using System;
using Microsoft.Xna.Framework;

namespace astral_assault;

public class MouseMoveEventArgs : EventArgs
{
    public Point Position { get; }
    public int X { get; }
    public int Y { get; }

    public MouseMoveEventArgs(Point position)
    {
        Position = position;
        X = position.X;
        Y = position.Y;
    }
}