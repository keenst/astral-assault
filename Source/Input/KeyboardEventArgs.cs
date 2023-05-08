#region
using System;
using Microsoft.Xna.Framework.Input;
#endregion

namespace TheGameOfDoomHmmm.Source.Input;

public sealed class KeyboardEventArgs : EventArgs
{
    internal KeyboardEventArgs(Keys[] keys)
    {
        Keys = keys;
    }

    internal Keys[] Keys { get; }
}