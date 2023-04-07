using System;
using Microsoft.Xna.Framework.Input;

namespace AstralAssault;

public class KeyboardEventArgs : EventArgs
{
    public Keys[] Keys { get; }

    public KeyboardEventArgs(Keys[] keys) => Keys = keys;
}