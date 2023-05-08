#region
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using TheGameOfDoomHmmm.Source.Game;
#endregion

namespace TheGameOfDoomHmmm.Source.Graphics;

public static class TextRenderer
{
    private static Game1 m_root;
    private static BitmapFont m_bitmapFont;

    public static void Init(Game1 root)
    {
        m_root = root;

        List<BitmapFontRegion> bitmapFontRegions = new List<BitmapFontRegion>();

        var pixelFont = new Color[177 * 793];
        var fontThing = AssetManager.Load<Texture2D>("AsepriteFont");
        fontThing.GetData(pixelFont);

        int charIdx = 0;

        for (int y = 1; y < 793; y += 11)
        {
            for (int x = 1; x < 177; x += 11)
            {
                int index = (y * 177) + x;
                int len = 0;

                while (pixelFont[index] != new Color(0, 255, 0, 255))
                {
                    len++;
                    index++;
                }

                bitmapFontRegions.Add
                (
                    new BitmapFontRegion
                    (
                        new TextureRegion2D(fontThing, x, y, len, 7),
                        ' ' + charIdx,
                        0,
                        0,
                        len
                    )
                );

                charIdx++;
            }
        }

        m_bitmapFont = new BitmapFont("teeest", bitmapFontRegions, 7);
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
        m_root.SpriteBatch.DrawString
        (
            m_bitmapFont,
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

    public static Vector2 Size(this string input) => m_bitmapFont.MeasureString(input);
}