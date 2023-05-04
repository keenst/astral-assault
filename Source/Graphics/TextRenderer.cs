#region
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AstralAssault;

public static class TextRenderer
{
    private static Game1 Root;
    private static SpriteFont f;
    private static Texture2D m_font;
    private static List<Rectangle> m_letters;

    public static void Init(Game1 root)
    {
        Root = root;

        m_letters = new List<Rectangle>();

        m_font = AssetManager.Load<Texture2D>("AsepriteFont");
        Color[] fontData = new Color[177 * 793];
        m_font.GetData(fontData);

        for (int y = 1; y < 793; y += 11)
        {
            for (int x = 1; x < 177; x += 11)
            {
                int index = (y * 177) + x;
                int len = 0;

                while (fontData[index] != new Color(0, 255, 0, 255))
                {
                    len++;
                    index++;
                }

                Rectangle charBounds = new Rectangle(x, y, len, 7);

                m_letters.Add(charBounds);
            }
        }

        f = AssetManager.Load<SpriteFont>("fc");
    }

    public static int Size(this string input)
    {
        int size = 0;

        foreach (char c in input)
        {
            int index = c - ' ';
            size += m_letters[index].Width;
        }

        return size;
    }

    public static ReadOnlySpan<DrawTask> CreateDrawTasks
    (
        this ReadOnlySpan<char> input,
        Vector2 position,
        Color color,
        LayerDepth layerDepth,
        bool useSpriteFont
    )
    {
        DrawTask[] drawTasks = new DrawTask[input.Length];

        if (!useSpriteFont)
        {
            int x = 0;
            int y = 0;

            for (int i = 0; i < input.Length; i++)
            {
                int index = input[i] - ' ';
                drawTasks[i] = new DrawTask
                (
                    m_font,
                    m_letters[index],
                    new Vector2(position.X + x, position.Y + y),
                    0,
                    layerDepth,
                    color,
                    Vector2.Zero
                );

                x += m_letters[index].Width;

                if (input[i] == '\n')
                {
                    y += 7;
                }
            }
        }
        else
        {
            int x = 0;
            int y = 0;

            for (int i = 0; i < input.Length; i++)
            {
                int index = input[i] - ' ';
                drawTasks[i] = new DrawTask
                (
                    f.Texture,
                    f.GetGlyphs()[input[i]].BoundsInTexture,
                    new Vector2(position.X + x, position.Y + y),
                    0,
                    layerDepth,
                    color,
                    Vector2.Zero
                );

                x += m_letters[index].Width;

                if (input[i] == '\n')
                {
                    y += 7;
                }
            }
        }

        return drawTasks;
    }
}