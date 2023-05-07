#region
using System;
#endregion

namespace AstralAssault;

public sealed class MouseButtonEventArgs : EventArgs
{
    internal MouseButtonEventArgs(InputEventSource.MouseButtons button)
    {
        Button = button;
    }

    internal InputEventSource.MouseButtons Button { get; }
}