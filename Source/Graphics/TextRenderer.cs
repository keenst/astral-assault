#region
using System;
using System.Collections.Generic;
using System.Drawing;
using AstralAssault.Source.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
#endregion

namespace AstralAssault;

public static class TextRenderer
{
    private static Game1 Root;
    private static SpriteFont f;

    public static void Init(Game1 root)
    {
        Root = root;
        f = AssetManager.Load<SpriteFont>("fc");
    }

    public static void Draw
    (
        this string input,
        Vector2 position,
        Color color,
        float rotation,
        Vector2 origin,
        float scale,
        LayerOrdering layer
    )
    {
        Root.m_spriteBatch.DrawString
        (
            f,
            input,
            position,
            color,
            rotation,
            origin,
            scale,
            SpriteEffects.None,
            layer.GetDisplayLayerValue()
        );
    }

    public static Vector2 Size(this string input) => f.MeasureString(input);
}