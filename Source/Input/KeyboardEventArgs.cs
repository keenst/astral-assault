using System;
using Microsoft.Xna.Framework.Input;

namespace astral_assault;

public class KeyboardEventArgs : EventArgs
{
    public Keys[] Keys { get; }

    public KeyboardEventArgs(Keys[] keys)
    {
        Keys = keys;
    }
}