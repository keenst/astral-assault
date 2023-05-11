#region
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using TheGameOfDoomHmmm.Source.Game;
#endregion

namespace TheGameOfDoomHmmm.Source.Graphics;

internal static class TextRenderer
{
    private static Game1 m_root;
    private static BitmapFont m_bitmapFont;

    public static void Init(Game1 root)
    {
        m_root = root;

        List<BitmapFontRegion> bitmapFontRegions = new List<BitmapFontRegion>();

        Texture2D fontTexture = AssetManager.Load<Texture2D>("AsepriteFont");
        Color[] fontPixelData = new Color[fontTexture.Width * fontTexture.Height];
        fontTexture.GetData(fontPixelData);

        int fontTextureWidth = fontTexture.Width;
        int fontTextureHeight = fontTexture.Height;

        int charIdx = 0;
        const int charHeight = 7;

        const int charWidthSpacing = 11;

        Color endColor = new Color(0, 255, 0, 255);

        for (int y = 1; y < fontTextureHeight; y += charWidthSpacing)
        {
            for (int x = 1; x < fontTextureWidth; x += charWidthSpacing)
            {
                int pixelIndex = y * fontTextureWidth + x;
                int charLen = 0;

                while (fontPixelData[pixelIndex] != endColor)
                {
                    charLen++;
                    pixelIndex++;
                }

                TextureRegion2D charRegion = new TextureRegion2D(fontTexture, x, y, charLen, charHeight);
                int character = ' ' + charIdx;

                BitmapFontRegion bitmapFontRegion = new BitmapFontRegion(charRegion, character, 0, 0, charLen);
                bitmapFontRegions.Add(bitmapFontRegion);

                charIdx++;
            }
        }

        m_bitmapFont = new BitmapFont("test", bitmapFontRegions, charHeight);
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