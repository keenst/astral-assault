using System;
using static AstralAssault.InputEventSource;

namespace AstralAssault;

public class MouseButtonEventArgs : EventArgs
{
    public MouseButtons Button { get; }

    public MouseButtonEventArgs(MouseButtons button) => Button = button;
}