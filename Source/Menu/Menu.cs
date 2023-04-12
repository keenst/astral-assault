using System.Collections.Generic;

namespace AstralAssault.Source.Menu;

public class Menu
{
    public List<IMenuItem> MenuItems { get; private set; }
    
    public Menu()
    {
        MenuItems = new List<IMenuItem>();
    }
    
    public void AddMenuItem(IMenuItem menuItem)
    {
        MenuItems.Add(menuItem);
    }
}