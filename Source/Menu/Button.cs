using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault.Source.Menu;

public struct Button : IMenuItem
{
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }
    public Action ClickAction { get; }
    public string Text { get; }
    public Texture2D Texture { get; }
    
    public Button(int x, int y, int width, int height, string text, Action clickAction)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Text = text;
        ClickAction = clickAction;

        Texture = AssetManager.Load<Texture2D>("Button");
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

    public List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new();

        
        
        return drawTasks;
    } 
}