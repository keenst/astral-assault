using System;
using Microsoft.Xna.Framework;

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

    public void OnClick();
    public void OnHoverEnter();
    public void OnHoverExit();
}