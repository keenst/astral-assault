using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault.Source.Menu;

public class MenuController
{
    private bool _isMenuOpen;
    private Menu _menu;
    private IMenuItem _prevHoveredMenuItem;
    private Texture2D _buttonTexture;

    public MenuController(Menu menu, GameState gameState)
    {
        _menu = menu;
        Game1 root = gameState.Root;

        _buttonTexture = new Texture2D(root.GraphicsDevice, 1, 1);
        _buttonTexture.SetData(new[] { Color.White });
        
        Open();
    }
    
    public void Update(UpdateEventArgs e)
    {
        HandleMouseButtons(e.MouseButtonsPressed, e.MousePosition);
        HandleMousePosition(e.MousePosition);
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
        _isMenuOpen = true;
    }
    
    public void Close()
    {
        _isMenuOpen = false;
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

    private void HandleMouseButtons(MouseButton[] mouseButtonsPressed, Point mousePosition)
    {
        if (!mouseButtonsPressed.Contains(MouseButton.Left)) return;
        
        IMenuItem clickedMenuItem = GetCollidingMenuItem(mousePosition.X, mousePosition.Y);
        clickedMenuItem?.ClickAction.Invoke(_menu);
    }

    private void HandleMousePosition(Point mousePosition)
    {
        IMenuItem hoveredMenuItem = GetCollidingMenuItem(mousePosition.X, mousePosition.Y);
        
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