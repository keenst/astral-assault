using System;
using Microsoft.Xna.Framework.Input;

namespace astral_assault;

public class KeyboardEventArgs : EventArgs
{
    public Keys Key { get; }

    public KeyboardEventArgs(Keys key)
    {
        Key = key;
    }
}