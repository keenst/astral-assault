using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault.Source.Menu;

public interface IMenuItem
{
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }
    public Rectangle Rectangle => new(X, Y, Width, Height);
    public Action ClickAction { get; }
    public string Text { get; }
    public Texture2D Texture { get; }
    public bool IsHovered { get; set; }
    
    public List<DrawTask> GetDrawTasks();
}