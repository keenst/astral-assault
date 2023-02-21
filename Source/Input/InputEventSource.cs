using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace astral_assault;

public class InputEventSource
{
    public event EventHandler<KeyboardEventArgs>
        KeyboardEvent,
        KeyboardPressedEvent,
        KeyboardReleasedEvent;
    
    public event EventHandler<MouseButtonEventArgs> 
        MouseButtonEvent, 
        MouseButtonPressedEvent;

    public event EventHandler<MouseMoveEventArgs>
        MouseMoveEvent;

    private List<Keys> _keysDown = new(); 
    private List<Keys> _prevKeysDown = new();
    
    private readonly List<MouseButtons> _mouseDown = new();
    private List<MouseButtons> _prevMouseDown = new();

    private Point _mousePos;
    private Point _prevMousePos;

    public enum MouseButtons
    {
        Left,
        Right,
        Middle,
        Side1,
        Side2
    }
    
    public void Update()
    {
        HandleKeyboard();
        HandleMouseButtons();
        HandleMouseMovement();
    }

    private void HandleKeyboard()
    {
        _prevKeysDown = _keysDown;
        _keysDown = Keyboard.GetState().GetPressedKeys().ToList();

        foreach (Keys key in _prevKeysDown.Where(key => !_keysDown.Contains(key)))
        {
            KeyboardReleasedEvent?.Invoke(this, new KeyboardEventArgs(key));
        }
        
        if (_keysDown.Count == 0)
        {
            _prevKeysDown.Clear();
            return;
        }

        foreach (Keys key in _keysDown)
        {
            KeyboardEvent?.Invoke(this, new KeyboardEventArgs(key));
            
            if (!_prevKeysDown.Contains(key))
            {
                KeyboardPressedEvent?.Invoke(this, new KeyboardEventArgs(key));
            }
        }
    }
    
    private void HandleMouseButtons()
    {
        MouseState mouseState = Mouse.GetState();

        _prevMouseDown = _mouseDown;
        _mouseDown.Clear();

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
            
            _mouseDown.Add(button);
            MouseButtonEvent?.Invoke(this, new MouseButtonEventArgs(button));

            if (!_prevMouseDown.Contains(button))
            {
                MouseButtonPressedEvent?.Invoke(this, new MouseButtonEventArgs(button));
            }
        }
    }

    private void HandleMouseMovement()
    {
        _prevMousePos = _mousePos;
        _mousePos = Mouse.GetState().Position;

        if (_mousePos != _prevMousePos)
        {
            MouseMoveEvent?.Invoke(this, new MouseMoveEventArgs(_mousePos));
        }
    }
}