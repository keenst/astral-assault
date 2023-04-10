#region
using System;
using Microsoft.Xna.Framework.Input;
#endregion

namespace AstralAssault;

public class KeyboardEventArgs : EventArgs
{
    public KeyboardEventArgs(Keys[] keys)
    {
        Keys = keys;
    }

    public Keys[] Keys { get; }
}