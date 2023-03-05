using System;

namespace astral_assault;

public class UpdateEventArgs : EventArgs
{
    public float DeltaTime { get; }

    public UpdateEventArgs(float deltaTime)
    {
        DeltaTime = deltaTime;
    }
}