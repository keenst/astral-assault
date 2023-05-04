#region
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AstralAssault;

public static class TextRenderer
{
    private static Texture2D _font;

    private static Dictionary<char, int> _dict;

    public static void Init()
    {
        _font = AssetManager.Load<Texture2D>("Font");
        _dict = new Dictionary<char, int>
        {
            { 'A', 0 },
            { 'B', 1 },
            { 'C', 2 },
            { 'D', 3 },
            { 'E', 4 },
            { 'F', 5 },
            { 'G', 6 },
            { 'H', 7 },
            { 'I', 8 },
            { 'J', 9 },
            { 'K', 10 },
            { 'L', 11 },
            { 'M', 12 },
            { 'N', 13 },
            { 'O', 14 },
            { 'P', 15 },
            { 'Q', 16 },
            { 'R', 17 },
            { 'S', 18 },
            { 'T', 19 },
            { 'U', 20 },
            { 'V', 21 },
            { 'W', 22 },
            { 'X', 23 },
            { 'Y', 24 },
            { 'Z', 25 },
            { '0', 26 },
            { '1', 27 },
            { '2', 28 },
            { '3', 29 },
            { '4', 30 },
            { '5', 31 },
            { '6', 32 },
            { '7', 33 },
            { '8', 34 },
            { '9', 35 },
            { ':', 36 },
            { ';', 37 },
            { '.', 38 },
            { ',', 39 },
            { '!', 40 },
            { ' ', 41 }
        };
    }

    public static ReadOnlySpan<DrawTask> CreateDrawTasks(
        this ReadOnlySpan<char> input,
        Vector2 position,
        Color color,
        LayerDepth layerDepth)
    {
        DrawTask[] drawTasks = new DrawTask[input.Length];
        byte[] sourceOffsets = new byte[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            char c = char.ToUpper(input[i]);
            int index = c - ' ';
            int x = index % 6;
            int y = index / 6;
            sourceOffsets[i] = (byte)(y * 48 + x * 8);
            drawTasks[i] = new DrawTask
            (
                _font,
                new Rectangle(sourceOffsets[i], 0, 8, 8),
                new Vector2(position.X + i * 8, position.Y),
                0,
                layerDepth,
                color,
                Vector2.Zero
            );
        }

        return drawTasks;
    }
}