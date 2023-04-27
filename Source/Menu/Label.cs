using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault.Source.Menu;

public class Label : IMenuItem
{
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }
    public Action ClickAction { get; }
    public string Text { get; }
    public Texture2D Texture { get; }
    public bool IsHovered { get; set; }

    public Label(int x, int y, int width, int height, string text)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        ClickAction = () => { };
        Text = text;

        Texture = null;
    }

    public List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new();

        int textX = X + Width / 2 - Text.Length * 4;
        int textY = Y + Height / 2 - 4;
        Vector2 textPos = new(textX, textY);
        List<DrawTask> textTasks = Text.CreateDrawTasks(textPos, Color.White, LayerDepth.HUD);
        
        drawTasks.AddRange(textTasks);

        return drawTasks;
    }
}