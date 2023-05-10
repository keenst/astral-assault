#region
using System;
using Microsoft.Xna.Framework.Input;
#endregion

namespace TheGameOfDoomHmmm.Source.Input;

public sealed class KeyboardEventArgs : EventArgs
{
    public KeyboardEventArgs(Keys[] keys)
    {
        Keys = keys;
    }

    public Keys[] Keys { get; }
}