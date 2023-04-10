#region
using System;
using static AstralAssault.InputEventSource;
#endregion

namespace AstralAssault;

public class MouseButtonEventArgs : EventArgs
{
    public MouseButtonEventArgs(MouseButtons button)
    {
        Button = button;
    }

    public MouseButtons Button { get; }
}