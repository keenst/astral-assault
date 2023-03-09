using System;
using static AstralAssault.InputEventSource;

namespace AstralAssault;

public class MouseButtonEventArgs : EventArgs
{
    public InputEventSource.MouseButtons Button { get; }

    public MouseButtonEventArgs(InputEventSource.MouseButtons button)
    {
        Button = button;
    }
}