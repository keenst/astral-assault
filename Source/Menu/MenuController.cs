using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MouseButtons = AstralAssault.InputEventSource.MouseButtons;

namespace AstralAssault.Source.Menu;

public class MenuController : IMouseEventListener
{
    private bool _isMenuOpen;
    private Menu _menu;
    private IMenuItem _prevHoveredMenuItem;

    public MenuController(Menu menu)
    {
        _menu = menu;
    }
    
    private void OnUpdate(object sender, UpdateEventArgs e)
    {
        
    }
    
    public List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new();

        if (!_isMenuOpen) return drawTasks;
        
        throw new NotImplementedException();
    }

    public void Open()
    {
        StartListening();
        _isMenuOpen = true;
    }
    
    public void Close()
    {
        StopListening();
        _isMenuOpen = false;
    }

    public void Destroy()
    {
        StopListening();
    }
    
    private IMenuItem GetCollidingMenuItem(int x, int y)
    {
        foreach (IMenuItem menuItem in _menu.MenuItems)
        {
            Rectangle button = new(menuItem.X, menuItem.Y, menuItem.Width, menuItem.Height);
            if (button.Contains(x, y))
            {
                return menuItem;
            }
        }

        return null;
    }

    private void StartListening()
    {
        UpdateEventSource.UpdateEvent += OnUpdate;
        InputEventSource.MouseButtonEvent += OnMouseButtonEvent;
        InputEventSource.MouseMoveEvent += OnMouseMoveEvent;
    }

    private void StopListening()
    {
        UpdateEventSource.UpdateEvent -= OnUpdate;
        InputEventSource.MouseButtonEvent -= OnMouseButtonEvent;
        InputEventSource.MouseMoveEvent -= OnMouseMoveEvent;
    }

    public void OnMouseButtonEvent(object sender, MouseButtonEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;

        IMenuItem clickedMenuItem = GetCollidingMenuItem(e.Position.X, e.Position.Y);
        clickedMenuItem?.OnClick();
    }

    public void OnMouseMoveEvent(object sender, MouseMoveEventArgs e)
    {
        IMenuItem hoveredMenuItem = GetCollidingMenuItem(e.Position.X, e.Position.Y);
        
        if (hoveredMenuItem == null)
        {
            if (_prevHoveredMenuItem == null) return;
            
            _prevHoveredMenuItem.OnHoverExit();
            _prevHoveredMenuItem = null;
        }
        else
        {
            if (_prevHoveredMenuItem == hoveredMenuItem) return;

            _prevHoveredMenuItem?.OnHoverExit();

            hoveredMenuItem.OnHoverEnter();
            _prevHoveredMenuItem = hoveredMenuItem;
        }
    }
}