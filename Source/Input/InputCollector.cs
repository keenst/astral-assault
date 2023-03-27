using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AstralAssault;

public class InputCollector
{
    public List<Keys> _keysDown = new(); 
    public List<Keys> _prevKeysDown = new();
    
    public readonly List<MouseButtons> MouseDown = new();
    public List<MouseButtons> _prevMouseDown = new();

    public Point _mousePos;
    public Point _prevMousePos;

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

        if (_keysDown.Count == 0)
        {
            _prevKeysDown.Clear();
            return;
        }

        List<Keys> keysDown = new();
        List<Keys> keysPressed = new();
        
        foreach (Keys key in _keysDown)
        {
            keysDown.Add(key);
            
            if (!_prevKeysDown.Contains(key))
            {
                keysPressed.Add(key);
            }
        }
    }
    
    private void HandleMouseButtons()
    {
        MouseState mouseState = Mouse.GetState();

        _prevMouseDown = MouseDown;
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
        }
    }

    private void HandleMouseMovement()
    {
        _prevMousePos = _mousePos;
        _mousePos = Mouse.GetState().Position;
    }
}