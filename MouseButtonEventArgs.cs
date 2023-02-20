using System;
using static astral_assault.InputEventSource;

namespace astral_assault;

public class MouseButtonEventArgs : EventArgs
{
    public MouseButtons Button { get; }

    public MouseButtonEventArgs(MouseButtons button)
    {
        Button = button;
    }
}