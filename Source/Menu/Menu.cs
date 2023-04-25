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
            DrawTask drawTask = new(
                texture,
                new Rectangle(0, 0, 1, 1),
                menuItem.Rectangle,
                0,
                LayerDepth.HUD,
                new List<IDrawTaskEffect> { new ColorEffect(Palette.GetColorVector(Palette.Colors.Grey7)) },
                Color.White);
            
            drawTasks.Add(drawTask);

            int textX = menuItem.X + menuItem.Width / 2 - menuItem.Text.Length * 4;
            int textY = menuItem.Y + menuItem.Height / 2 - 4;
            Vector2 textPos = new(textX, textY);
            List<DrawTask> textTasks = menuItem.Text.CreateDrawTasks(textPos, Color.White, LayerDepth.HUD);
            
            drawTasks.AddRange(textTasks);
        }
        
        return drawTasks;
    }
}