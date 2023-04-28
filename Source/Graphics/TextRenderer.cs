#region
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
#endregion

namespace AstralAssault;

public static class TextRenderer
{
    private static BitmapFont m_font;
    private static Game1 Root;

    public static void Init(Game1 root)
    {
        Root = root;

        m_font = AssetManager.Load<BitmapFont>("unifont");
    }

    public static List<DrawTask> CreateDrawTasks(
        this string input,
        Vector2 position,
        Color color,
        LayerDepth layerDepth,
        List<IDrawTaskEffect> effects)
    {
        List<DrawTask> drawTasks = new List<DrawTask>();

        Rectangle stringSize = ((Rectangle)m_font.GetStringRectangle(input));
        Root.m_spriteBatch.Begin();
        Root.m_spriteBatch.DrawString(m_font, input, position, color);
        Root.m_spriteBatch.End();

        return drawTasks;
    }

    public static List<DrawTask> CreateDrawTasks(
        this string input,
        Vector2 position,
        Color color,
        LayerDepth layerDepth) => input.CreateDrawTasks(position, color, layerDepth, new List<IDrawTaskEffect>());
}