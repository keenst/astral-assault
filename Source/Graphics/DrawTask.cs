#region
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AstralAssault;

public struct DrawTask
{
    public Texture2D Texture { get; }
    public Rectangle Source { get; }
    public Rectangle Destination { get; }
    public float Rotation { get; }
    public LayerDepth LayerDepth { get; }
    public Color Color { get; }
    public Vector2 Origin { get; }

    public DrawTask(
        Texture2D texture,
        Rectangle source,
        Rectangle destination,
        float rotation,
        LayerDepth layerDepth,
        Color color)
    {
        Texture = texture;
        Source = source;
        Destination = destination;
        Rotation = rotation;
        LayerDepth = layerDepth;
        Color = color;
        Origin = new Vector2
        (
            (float)Math.Round(Source.Width / 2D),
            (float)Math.Round(Source.Height / 2D)
        );
    }

    public DrawTask(
        Texture2D texture,
        Rectangle source,
        Vector2 position,
        float rotation,
        LayerDepth layerDepth,
        Color color,
        Vector2 origin)
    {
        Texture = texture;
        Source = source;
        Destination = new Rectangle
        (
            (int)position.X,
            (int)position.Y,
            source.Width,
            source.Height
        );
        Rotation = rotation;
        LayerDepth = layerDepth;
        Color = color;
        Origin = origin;
    }

    public DrawTask(
        Texture2D texture,
        Vector2 position,
        float rotation,
        LayerDepth layerDepth,
        Color color,
        Vector2 origin)
    {
        Texture = texture;
        Source = new Rectangle
        (
            0,
            0,
            texture.Width,
            texture.Height
        );
        Destination = new Rectangle
        (
            (int)position.X,
            (int)position.Y,
            Source.Width,
            Source.Height
        );
        Rotation = rotation;
        LayerDepth = layerDepth;
        Color = color;
        Origin = origin;
    }

    public DrawTask(
        Texture2D texture,
        Rectangle source,
        Vector2 position,
        float rotation,
        LayerDepth layerDepth)
    {
        Texture = texture;
        Source = source;
        Destination = new Rectangle
        (
            (int)position.X,
            (int)position.Y,
            source.Width,
            source.Height
        );
        Rotation = rotation;
        LayerDepth = layerDepth;
        Color = Palette.GetColor(Palette.Colors.Grey9);
        Origin = new Vector2
        (
            (float)Math.Round(Source.Width / 2D),
            (float)Math.Round(Source.Height / 2D)
        );
    }

    public DrawTask(
        Texture2D texture,
        Vector2 position,
        float rotation,
        LayerDepth layerDepth)
    {
        Texture = texture;
        Source = new Rectangle
        (
            0,
            0,
            texture.Width,
            texture.Height
        );
        Destination = new Rectangle
        (
            (int)position.X,
            (int)position.Y,
            Source.Width,
            Source.Height
        );
        Rotation = rotation;
        LayerDepth = layerDepth;
        Color = Palette.GetColor(Palette.Colors.Grey9);
        Origin = new Vector2
        (
            (float)Math.Round(Source.Width / 2D),
            (float)Math.Round(Source.Height / 2D)
        );
    }
}