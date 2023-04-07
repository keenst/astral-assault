using System;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public static class UpdateEventSource
{
    public static event EventHandler<UpdateEventArgs> UpdateEvent;

    public static void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        UpdateEventSource.UpdateEvent?.Invoke(null, new(deltaTime));
    }
}