#region
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TheGameOfDoomHmmm.Source.Game;
#endregion

namespace TheGameOfDoomHmmm.Source.Input;

internal static class InputEventSource
{
    public enum MouseButtons { Left }

    private static Keys[] m_keysDown;
    private static Keys[] m_prevKeysDown;

    private static readonly List<MouseButtons> MouseDown = new List<MouseButtons>();
    private static List<MouseButtons> m_prevMouseDown = new List<MouseButtons>();

    private static Point m_mousePos;
    private static Point m_prevMousePos;

    public static event EventHandler<KeyboardEventArgs>
        KeyboardEvent,
        KeyboardPressedEvent;

    public static event EventHandler<MouseButtonEventArgs>
        MouseButtonEvent,
        MouseButtonPressedEvent;

    public static event EventHandler<MouseMoveEventArgs>
        MouseMoveEvent;

    public static void Init()
    {
        UpdateEventSource.UpdateEvent += OnUpdate;
    }

    private static void OnUpdate(object sender, UpdateEventArgs e)
    {
        HandleKeyboard();
        HandleMouseButtons();
        HandleMouseMovement();
    }

    private static void HandleKeyboard()
    {
        m_prevKeysDown = m_keysDown;
        m_keysDown = Keyboard.GetState().GetPressedKeys();

        if (m_keysDown.Length == 0) m_prevKeysDown = new[] { Keys.End };

        List<Keys> keysDown = new List<Keys>();
        List<Keys> keysPressed = new List<Keys>();

        foreach (Keys key in m_keysDown)
        {
            keysDown.Add(key);

            if (!m_prevKeysDown.Contains(key)) keysPressed.Add(key);
        }

        KeyboardEvent?.Invoke(null, new KeyboardEventArgs(keysDown.ToArray()));

        if (keysPressed.Count == 0) return;

        KeyboardPressedEvent?.Invoke(null, new KeyboardEventArgs(keysPressed.ToArray()));
    }

    private static void HandleMouseButtons()
    {
        MouseState mouseState = Mouse.GetState();

        m_prevMouseDown = MouseDown;
        MouseDown.Clear();

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

            MouseDown.Add(button);
            MouseButtonEvent?.Invoke(null, new MouseButtonEventArgs(button));

            if (!m_prevMouseDown.Contains(button))
                MouseButtonPressedEvent?.Invoke(null, new MouseButtonEventArgs(button));
        }
    }

    private static void HandleMouseMovement()
    {
        m_prevMousePos = m_mousePos;
        m_mousePos = Mouse.GetState().Position;

        if (m_mousePos != m_prevMousePos) MouseMoveEvent?.Invoke(null, new MouseMoveEventArgs(m_mousePos));
    }
}