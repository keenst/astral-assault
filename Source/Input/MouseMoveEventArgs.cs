#region
using System;
using Microsoft.Xna.Framework;
#endregion

namespace AstralAssault;

public class MouseMoveEventArgs : EventArgs
{
    public MouseMoveEventArgs(Point position)
    {
        Position = position;
    }

    public Point Position { get; }
}