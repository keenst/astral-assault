using System;
using Microsoft.Xna.Framework;
using static AstralAssault.InputEventSource;

namespace AstralAssault;

public class MouseButtonEventArgs : EventArgs
{
    public MouseButtons Button { get; }
    public Point Position { get; }

    public MouseButtonEventArgs(MouseButtons button, Point position)
    {
        Button = button;
        Position = position;
    }
}