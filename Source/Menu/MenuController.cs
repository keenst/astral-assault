using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MouseButtons = AstralAssault.InputEventSource.MouseButtons;

namespace AstralAssault.Source.Menu;

public class MenuController : IMouseMoveEventListener, IMousePressedEventListener
{
    public Game1 Root { get; }
    
    private bool _isMenuOpen;
    private Menu _menu;
    private IMenuItem _prevHoveredMenuItem;
    private Texture2D _buttonTexture;

    public MenuController(Menu menu, GameState gameState)
    {
        _menu = menu;
        Root = gameState.Root;

        _buttonTexture = new Texture2D(Root.GraphicsDevice, 1, 1);
        _buttonTexture.SetData(new[] { Color.White });
        
        Open();
    }
    
    private void OnUpdate(object sender, UpdateEventArgs e)
    {
        
    }
    
    public List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new();

        if (!_isMenuOpen) return drawTasks;
        
        drawTasks.AddRange(_menu.GetDrawTasks(_buttonTexture));
        return drawTasks;
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
        InputEventSource.MousePressedEvent += OnMousePressedEvent;
        InputEventSource.MouseMoveEvent += OnMouseMoveEvent;
    }

    private void StopListening()
    {
        UpdateEventSource.UpdateEvent -= OnUpdate;
        InputEventSource.MousePressedEvent -= OnMousePressedEvent;
        InputEventSource.MouseMoveEvent -= OnMouseMoveEvent;
    }

    public void OnMousePressedEvent(object sender, MouseButtonEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;
        
        IMenuItem clickedMenuItem = GetCollidingMenuItem(e.Position.X, e.Position.Y);
        clickedMenuItem?.ClickAction.Invoke(_menu);
    }

    public void OnMouseMoveEvent(object sender, MouseMoveEventArgs e)
    {
        IMenuItem hoveredMenuItem = GetCollidingMenuItem(e.Position.X, e.Position.Y);
        
        if (hoveredMenuItem == null)
        {
            if (_prevHoveredMenuItem == null) return;

            _prevHoveredMenuItem.IsHovered = false;
            _prevHoveredMenuItem = null;
        }
        else
        {
            if (_prevHoveredMenuItem == hoveredMenuItem) return;
            
            if (_prevHoveredMenuItem != null) _prevHoveredMenuItem.IsHovered = false;

            hoveredMenuItem.IsHovered = true;
            _prevHoveredMenuItem = hoveredMenuItem;
        }
    }
}