#region
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGameOfDoomHmmm.Source.Game;
#endregion

namespace TheGameOfDoomHmmm.Source.Graphics;

public static class TextureRenderer
{
    public static Game1 Root;

    public static void Init(Game1 root)
    {
        Root = root;
    }

    public static void DrawTexture2D
    (
        this Texture2D texture,
        Rectangle source,
        Rectangle destination,
        float rotation,
        Color color,
        LayerOrdering layer
    )
    {
        Root.SpriteBatch.Draw
        (
            texture,
            destination,
            source,
            color,
            rotation,
            new Vector2
            (
                (float)Math.Round(source.Width / 2D),
                (float)Math.Round(source.Height / 2D)
            ),
            SpriteEffects.None,
            layer.GetDisplayLayerValue()
        );
    }

    public static void DrawTexture2D
    (
        this Texture2D texture,
        Rectangle source,
        Vector2 position,
        float rotation,
        Color color,
        Vector2 origin,
        LayerOrdering layer
    )
    {
        Root.SpriteBatch.Draw
        (
            texture,
            new Rectangle
            (
                (int)position.X,
                (int)position.Y,
                source.Width,
                source.Height
            ),
            source,
            color,
            rotation,
            origin,
            SpriteEffects.None,
            layer.GetDisplayLayerValue()
        );
    }

    public static void DrawTexture2D
    (
        this Texture2D texture,
        Vector2 position,
        float rotation,
        Color color,
        Vector2 origin,
        LayerOrdering layer
    )
    {
        Rectangle source = new Rectangle
        (
            0,
            0,
            texture.Width,
            texture.Height
        );

        Root.SpriteBatch.Draw
        (
            texture,
            new Rectangle
            (
                (int)position.X,
                (int)position.Y,
                source.Width,
                source.Height
            ),
            source,
            color,
            rotation,
            origin,
            SpriteEffects.None,
            layer.GetDisplayLayerValue()
        );
    }

    public static void DrawTexture2D
    (
        this Texture2D texture,
        Rectangle source,
        Vector2 position,
        float rotation,
        LayerOrdering layer
    )
    {
        Root.SpriteBatch.Draw
        (
            texture,
            new Rectangle
            (
                (int)position.X,
                (int)position.Y,
                source.Width,
                source.Height
            ),
            source,
            Palette.GetColor(Palette.Colors.Grey9),
            rotation,
            new Vector2
            (
                (float)Math.Round(source.Width / 2D),
                (float)Math.Round(source.Height / 2D)
            ),
            SpriteEffects.None,
            layer.GetDisplayLayerValue()
        );
    }

    public static void DrawTexture2D
    (
        this Texture2D texture,
        Vector2 position,
        float rotation,
        LayerOrdering layer
    )
    {
        Rectangle source = new Rectangle
        (
            0,
            0,
            texture.Width,
            texture.Height
        );
        Root.SpriteBatch.Draw
        (
            texture,
            new Rectangle
            (
                (int)position.X,
                (int)position.Y,
                source.Width,
                source.Height
            ),
            source,
            Palette.GetColor(Palette.Colors.Grey9),
            rotation,
            new Vector2
            (
                (float)Math.Round(source.Width / 2D),
                (float)Math.Round(source.Height / 2D)
            ),
            SpriteEffects.None,
            layer.GetDisplayLayerValue()
        );
    }
}