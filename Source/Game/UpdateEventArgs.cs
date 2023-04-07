using System;

namespace AstralAssault;

public class UpdateEventArgs : EventArgs
{
    public float DeltaTime { get; }

    public UpdateEventArgs(float deltaTime) => DeltaTime = deltaTime;
}