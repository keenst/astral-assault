using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault.Source.Menu;

public class Menu
{
    public List<IMenuItem> MenuItems { get; }
    
    public Menu()
    {
        MenuItems = new List<IMenuItem>();
    }
    
    public void AddMenuItem(IMenuItem menuItem)
    {
        MenuItems.Add(menuItem);
    }
    
    public List<DrawTask> GetDrawTasks(Texture2D texture)
    {
        List<DrawTask> drawTasks = new();
        
        foreach (IMenuItem menuItem in MenuItems)
        {
            drawTasks.AddRange(menuItem.GetDrawTasks());
        }
        
        return drawTasks;
    }
}