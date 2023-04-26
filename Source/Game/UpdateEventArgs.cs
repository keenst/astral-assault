#region
using System;
using Microsoft.Xna.Framework;
#endregion

namespace AstralAssault;

public class UpdateEventArgs : EventArgs
{
    public UpdateEventArgs(float deltaTime, GameTime gt)
    {
        DeltaTime = deltaTime;
        Gt = gt;
    }

    public float DeltaTime { get; }
    public GameTime Gt { get; }
}