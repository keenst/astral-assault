#region
using System;
#endregion

namespace AstralAssault;

public class UpdateEventArgs : EventArgs
{
    public UpdateEventArgs(float deltaTime)
    {
        DeltaTime = deltaTime;
    }

    public float DeltaTime { get; }
}