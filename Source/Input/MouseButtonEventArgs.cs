#region
using System;
#endregion

namespace AstralAssault;

public class MouseButtonEventArgs : EventArgs
{
    public MouseButtonEventArgs(InputEventSource.MouseButtons button)
    {
        Button = button;
    }

    public InputEventSource.MouseButtons Button { get; }
}