using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault.Source.Menu;

public class Button : IMenuItem
{
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }
    public Action ClickAction { get; }
    public string Text { get; }
    public Texture2D Texture { get; }
    public bool IsHovered { get; set; }

    private readonly List<DrawTask> _default = new();
    private readonly List<DrawTask> _hovered = new();

    public Button(int x, int y, int width, int height, string text, Action clickAction)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Text = text;
        ClickAction = clickAction;

        Texture = AssetManager.Load<Texture2D>("Button");

        if (width % 2 != 0 && height % 2 != 0)
        {
            throw new ArgumentException("Button width and height must be even");
        }

        InitDrawTasks();
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
        return IsHovered ? _hovered : _default;
    }
    
    private void InitDrawTasks()
    {
        int textX = X + Width / 2 - Text.Length * 4;
        int textY = Y + Height / 2 - 4;
        Vector2 textPos = new(textX, textY);
        List<DrawTask> textTasks = Text.CreateDrawTasks(textPos, Color.White, LayerDepth.MenuText);

        _default.AddRange(textTasks);
        _hovered.AddRange(textTasks);

        int halfWidth = Width / 2;
        int halfHeight = Height / 2;

        for (int x = 0; x < halfWidth; x++)
        {
            for (int y = 0; y < halfHeight; y++)
            {
                int tile = GetTileIndex(x, y);

                Rectangle defaultSource = new(tile * 2, 0, 2, 2);
                Rectangle hoveredSource = new(tile * 2, 2, 2, 2);
                Vector2 position = new(X + 1 + x * 2, Y + 1 + y * 2);

                DrawTask defaultTask =
                    new(Texture, defaultSource, position, 0, LayerDepth.Menu, new List<IDrawTaskEffect>());
                DrawTask hoveredTask =
                    new(Texture, hoveredSource, position, 0, LayerDepth.Menu, new List<IDrawTaskEffect>());

                _default.Add(defaultTask);
                _hovered.Add(hoveredTask);
            }
        }
    }

    private int GetTileIndex(int x, int y)
    {
        int halfWidth = Width / 2;
        int halfHeight = Height / 2;
        
        if (x == 0 && y == 0)
        {
            return 0;
        }
        
        if (x == 0 && y == halfHeight - 1)
        {
            return 3;
        }
        
        if (x == 0 && y < halfHeight - 1)
        {
            return 1;
        }
        
        if (x == halfWidth - 1 && y == 0)
        {
            return 3;
        }
        
        if (x < halfWidth - 1 && y == 0)
        {
            return 1;
        }
        
        if (x < halfWidth - 1 && y == halfHeight - 1)
        {
            return 4;
        }
        
        if (x == halfWidth - 1 && y > 0)
        {
            return 4;
            
        }
        return 2;
    }
}