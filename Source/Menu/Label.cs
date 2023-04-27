using System;

namespace AstralAssault.Source.Menu;

public struct Label : IMenuItem
{
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }
    public Action ClickAction { get; }
    public string Text { get; }

    public Label(int x, int y, int width, int height, string text)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        ClickAction = () => { };
        Text = text;
    }
    
    public void OnClick()
    {
        
    }

    public void OnHoverEnter()
    {
        
    }

    public void OnHoverExit()
    {
        
    }
}