#region
using System;
using Microsoft.Xna.Framework.Input;
#endregion

namespace AstralAssault;

public sealed class KeyboardEventArgs : EventArgs
{
    internal KeyboardEventArgs(Keys[] keys)
    {
        Keys = keys;
    }

    internal Keys[] Keys { get; }
}