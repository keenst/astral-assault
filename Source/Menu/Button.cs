using System;

namespace AstralAssault.Source.Menu;

public struct Button : IMenuItem
{
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }

    public Button(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
    
    public void OnClick()
    {
        throw new NotImplementedException();
    }

    public void OnHoverEnter()
    {
        throw new NotImplementedException();
    }

    public void OnHoverExit()
    {
        throw new NotImplementedException();
    }
}