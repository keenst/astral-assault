using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AstralAssault;

public class UpdateEventArgs : EventArgs
{
    public float DeltaTime { get; }
    public Keys[] KeysDown { get; }
    public Keys[] KeysPressed { get; }
    public MouseButton[] MouseButtonsDown { get; }
    public MouseButton[] MouseButtonsPressed { get; }
    public Point MousePosition { get; }
    
    public UpdateEventArgs(
        float deltaTime, 
        Keys[] keysDown, 
        Keys[] keysPressed, 
        MouseButton[] mouseButtonsDown, 
        MouseButton[] mouseButtonsPressed, 
        Point mousePosition)
    {
        DeltaTime = deltaTime;
        KeysDown = keysDown;
        KeysPressed = keysPressed;
        MouseButtonsDown = mouseButtonsDown;
        MouseButtonsPressed = mouseButtonsPressed;
        MousePosition = mousePosition;
    }
}