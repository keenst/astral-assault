using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MouseButtons = AstralAssault.InputEventSource.MouseButtons;

namespace AstralAssault.Source.Menu;

public class MenuController : IMouseMoveEventListener, IMousePressedEventListener
{
    private bool _isMenuOpen;
    private Menu _menu;
    private IMenuItem _prevHoveredMenuItem;
    private Texture2D _buttonTexture;
    private Game1 _root;

    public MenuController(Menu menu, GameState gameState)
    {
        _menu = menu;
        _root = gameState.Root;

        _buttonTexture = new Texture2D(_root.GraphicsDevice, 1, 1);
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
        
        DrawTask drawTask = new(
            _buttonTexture,
            new Rectangle(0, 0, 1, 1),
            new Rectangle(0, 0, 32, 32),
            0,
            LayerDepth.HUD,
            new List<IDrawTaskEffect>(),
            Color.White);

        drawTasks.Add(drawTask);
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
        clickedMenuItem?.ClickAction();
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