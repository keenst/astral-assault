using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AstralAssault;

public static class InputEventSource
{
    public static event EventHandler<KeyboardEventArgs>
        KeyboardEvent,
        KeyboardPressedEvent;

    public static event EventHandler<MouseButtonEventArgs>
        MouseButtonEvent,
        MouseButtonPressedEvent;

    public static event EventHandler<MouseMoveEventArgs>
        MouseMoveEvent;

    private static List<Keys> _keysDown = new();
    private static List<Keys> _prevKeysDown = new();

    private static readonly List<MouseButtons> MouseDown = new();
    private static List<MouseButtons> _prevMouseDown = new();

    private static Point _mousePos;
    private static Point _prevMousePos;

    public enum MouseButtons
    {
        Left,
        Right,
        Middle,
        Side1,
        Side2
    }

    public static void Init()
    {
        UpdateEventSource.UpdateEvent += InputEventSource.OnUpdate;
    }

    private static void OnUpdate(object sender, UpdateEventArgs e)
    {
        InputEventSource.HandleKeyboard();
        InputEventSource.HandleMouseButtons();
        InputEventSource.HandleMouseMovement();
    }

    private static void HandleKeyboard()
    {
        InputEventSource._prevKeysDown = InputEventSource._keysDown;
        InputEventSource._keysDown = Keyboard.GetState().GetPressedKeys().ToList();

        if (InputEventSource._keysDown.Count == 0)
        {
            InputEventSource._prevKeysDown.Clear();

            return;
        }

        List<Keys> keysDown = new();
        List<Keys> keysPressed = new();

        foreach (Keys key in InputEventSource._keysDown)
        {
            keysDown.Add(key);

            if (!InputEventSource._prevKeysDown.Contains(key))
            {
                keysPressed.Add(key);
            }
        }

        InputEventSource.KeyboardEvent?.Invoke(null, new(keysDown.ToArray()));

        if (keysPressed.Count == 0) return;
        InputEventSource.KeyboardPressedEvent?.Invoke(null, new(keysPressed.ToArray()));
    }

    private static void HandleMouseButtons()
    {
        MouseState mouseState = Mouse.GetState();

        InputEventSource._prevMouseDown = InputEventSource.MouseDown;
        InputEventSource.MouseDown.Clear();

        ButtonState[] buttonStates = new ButtonState[5];
        buttonStates[0] = mouseState.LeftButton;
        buttonStates[1] = mouseState.RightButton;
        buttonStates[2] = mouseState.MiddleButton;
        buttonStates[3] = mouseState.XButton1;
        buttonStates[4] = mouseState.XButton2;

        for (int i = 0; i < 5; i++)
        {
            if (buttonStates[i] != ButtonState.Pressed) continue;

            MouseButtons button = (MouseButtons)i;

            InputEventSource.MouseDown.Add(button);
            InputEventSource.MouseButtonEvent?.Invoke(null, new(button));

            if (!InputEventSource._prevMouseDown.Contains(button))
            {
                InputEventSource.MouseButtonPressedEvent?.Invoke(null, new(button));
            }
        }
    }

    private static void HandleMouseMovement()
    {
        InputEventSource._prevMousePos = InputEventSource._mousePos;
        InputEventSource._mousePos = Mouse.GetState().Position;

        if (InputEventSource._mousePos != InputEventSource._prevMousePos)
        {
            InputEventSource.MouseMoveEvent?.Invoke(null, new(InputEventSource._mousePos));
        }
    }
}