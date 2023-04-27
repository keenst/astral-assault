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
    private Texture2D empty;

    public Button(int x, int y, int width, int height, string text, Action clickAction)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Text = text;
        ClickAction = clickAction;

        Texture = AssetManager.Load<Texture2D>("Button");

        empty = new Texture2D(Texture.GraphicsDevice, 1, 1);
        empty.SetData(new Color[] { Color.White });
        
        if (width % 2 != 0 && height % 2 != 0)
        {
            throw new ArgumentException("Button width and height must be even");
        }
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

        int textX = X + Width / 2 - Text.Length * 4;
        int textY = Y + Height / 2 - 4;
        Vector2 textPos = new(textX, textY);
        List<DrawTask> textTasks = Text.CreateDrawTasks(textPos, Color.White, LayerDepth.MenuText);
        
        drawTasks.AddRange(textTasks);

        int halfWidth = Width / 2;
        int halfHeight = Height / 2;
        
        for (int x = 0; x < halfWidth; x++)
        {
            for (int y = 0; y < halfHeight; y++)
            {
                int tile;
                
                if (x == 0 && y == 0)
                {
                    tile = 0;
                }
                else if (x == 0 && y == halfHeight - 1)
                {
                    tile = 3;
                }
                else if (x == 0 && y < halfHeight - 1)
                {
                    tile = 1;
                }
                else if (x == halfWidth - 1 && y == 0)
                {
                    tile = 3;
                }
                else if (x < halfWidth - 1 && y == 0)
                {
                    tile = 1;
                }
                else if (x < halfWidth - 1 && y == halfHeight - 1)
                {
                    tile = 4;
                }
                else if (x == halfWidth - 1 && y > 0)
                {
                    tile = 4;
                }
                else
                {
                    tile = 2;
                }

                Rectangle source = new(tile * 2, IsHovered ? 2 : 0, 2, 2);
                Vector2 position = new(X + 1 + x * 2, Y + 1 + y * 2);

                DrawTask tileTask = new(Texture, source, position, 0, LayerDepth.Menu, new List<IDrawTaskEffect>());
                drawTasks.Add(tileTask);
            }
        }

        return drawTasks;
    } 
}