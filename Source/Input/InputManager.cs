using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AstralAssault;

public class InputManager
{
    private List<Keys> _keysDown = new(); 
    private List<Keys> _prevKeysDown = new();
    
    private readonly List<MouseButton> _mouseDown = new();
    private List<MouseButton> _prevMouseDown = new();

    private Point _mousePos;

    private Game1 _root;

    public InputManager(Game1 root)
    {
        _root = root;
    }
    
    public void GetInputs(
        out Keys[] keysDown, 
        out Keys[] keysPressed, 
        out MouseButton[] mouseButtonsDown, 
        out MouseButton[] mouseButtonsPressed, 
        out Point mousePosition)
    {
        HandleKeyboard(out keysDown, out keysPressed);
        HandleMouseButtons(out mouseButtonsDown, out mouseButtonsPressed);
        HandleMouseMovement(out mousePosition);
    }

    private void HandleKeyboard(out Keys[] keysDown, out Keys[] keysPressed)
    {
        _prevKeysDown = _keysDown;
        _keysDown = Keyboard.GetState().GetPressedKeys().ToList();

        if (_keysDown.Count == 0)
        {
            _prevKeysDown.Clear();
        }
        
        List<Keys> keysDownList = new();
        List<Keys> keysPressedList = new();
        
        foreach (Keys key in _keysDown)
        {
            keysDownList.Add(key);
            
            if (!_prevKeysDown.Contains(key))
            {
                keysPressedList.Add(key);
            }
        }
        
        keysDown = keysDownList.ToArray();

        if (keysPressedList.Count == 0)
        {
            keysPressed = Array.Empty<Keys>();
            return;
        }
        
        keysPressed = keysPressedList.ToArray();
    }
    
    private void HandleMouseButtons(out MouseButton[] mouseButtonsDown, out MouseButton[] mouseButtonsPressed)
    {
        MouseState mouseState = Mouse.GetState();

        _prevMouseDown = _mouseDown.ToList();
        _mouseDown.Clear();

        ButtonState[] buttonStates = new ButtonState[5];
        buttonStates[0] = mouseState.LeftButton;
        buttonStates[1] = mouseState.RightButton;
        buttonStates[2] = mouseState.MiddleButton;
        buttonStates[3] = mouseState.XButton1;
        buttonStates[4] = mouseState.XButton2;

        List<MouseButton> buttonsPressed = new();
        
        for (int i = 0; i < 5; i++)
        {
            if (buttonStates[i] != ButtonState.Pressed) continue;

            MouseButton button = (MouseButton)i;
            
            _mouseDown.Add(button);

            if (!_prevMouseDown.Contains(button))
            {
                buttonsPressed.Add(button);
            }
        }
        
        mouseButtonsDown = _mouseDown.ToArray();
        mouseButtonsPressed = buttonsPressed.ToArray();
    }

    private void HandleMouseMovement(out Point mousePosition)
    {
        _mousePos = Mouse.GetState().Position;
        mousePosition = GetScreenPosition(_mousePos);
    }

    private Point GetScreenPosition(Point position)
    {
        Point scale = new((int)_root.ScaleX, (int)_root.ScaleY);
        return position / scale;
    }
}