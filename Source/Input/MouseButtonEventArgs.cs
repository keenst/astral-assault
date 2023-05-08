#region
using System;
#endregion

namespace TheGameOfDoomHmmm.Source.Input;

public sealed class MouseButtonEventArgs : EventArgs
{
    internal MouseButtonEventArgs(InputEventSource.MouseButtons button)
    {
        Button = button;
    }

    internal InputEventSource.MouseButtons Button { get; }
}