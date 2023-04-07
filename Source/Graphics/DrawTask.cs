using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public struct DrawTask
{
    public Texture2D Texture { get; }
    public Rectangle Source { get; }
    public Rectangle Destination { get; }
    public float Rotation { get; }
    public LayerDepth LayerDepth { get; }
    public EffectContainer EffectContainer = new();
    public Color Color { get; }
    public Vector2 Origin { get; }

    public DrawTask(
        Texture2D texture,
        Rectangle source,
        Rectangle destination,
        float rotation,
        LayerDepth layerDepth,
        List<IDrawTaskEffect> effects,
        Color color)
    {
        Texture = texture;
        Source = source;
        Destination = destination;
        Rotation = rotation;
        LayerDepth = layerDepth;
        EffectContainer = new(effects);
        Color = color;
        Origin = new(
            (float)Math.Round(Source.Width / 2D),
            (float)Math.Round(Source.Height / 2D));
    }

    public DrawTask(
        Texture2D texture,
        Rectangle source,
        Vector2 position,
        float rotation,
        LayerDepth layerDepth,
        List<IDrawTaskEffect> effects,
        Color color,
        Vector2 origin)
    {
        Texture = texture;
        Source = source;
        Destination = new(
            (int)position.X,
            (int)position.Y,
            source.Width,
            source.Height);
        Rotation = rotation;
        LayerDepth = layerDepth;
        EffectContainer = new(effects);
        Color = color;
        Origin = origin;
    }

    public DrawTask(
        Texture2D texture,
        Vector2 position,
        float rotation,
        LayerDepth layerDepth,
        List<IDrawTaskEffect> effects,
        Color color,
        Vector2 origin)
    {
        Texture = texture;
        Source = new(
            0,
            0,
            texture.Width,
            texture.Height);
        Destination = new(
            (int)position.X,
            (int)position.Y,
            Source.Width,
            Source.Height);
        Rotation = rotation;
        LayerDepth = layerDepth;
        EffectContainer = new(effects);
        Color = color;
        Origin = origin;
    }

    public DrawTask(
        Texture2D texture,
        Rectangle source,
        Vector2 position,
        float rotation,
        LayerDepth layerDepth,
        List<IDrawTaskEffect> effects)
    {
        Texture = texture;
        Source = source;
        Destination = new(
            (int)position.X,
            (int)position.Y,
            source.Width,
            source.Height);
        Rotation = rotation;
        LayerDepth = layerDepth;
        EffectContainer = new(effects);
        Color = Palette.GetColor(Palette.Colors.Grey9);
        Origin = new(
            (float)Math.Round(Source.Width / 2D),
            (float)Math.Round(Source.Height / 2D));
    }

    public DrawTask(
        Texture2D texture,
        Vector2 position,
        float rotation,
        LayerDepth layerDepth,
        List<IDrawTaskEffect> effects)
    {
        Texture = texture;
        Source = new(
            0,
            0,
            texture.Width,
            texture.Height);
        Destination = new(
            (int)position.X,
            (int)position.Y,
            Source.Width,
            Source.Height);
        Rotation = rotation;
        LayerDepth = layerDepth;
        EffectContainer = new(effects);
        Color = Palette.GetColor(Palette.Colors.Grey9);
        Origin = new(
            (float)Math.Round(Source.Width / 2D),
            (float)Math.Round(Source.Height / 2D));
    }
}