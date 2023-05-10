#region
using System;
#endregion

namespace TheGameOfDoomHmmm.Source.Input;

public sealed class MouseButtonEventArgs : EventArgs
{
    public MouseButtonEventArgs(InputEventSource.MouseButtons button)
    {
        Button = button;
    }

    public InputEventSource.MouseButtons Button { get; }
}