#region
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AstralAssault;

public static class TextRenderer
{
    private static Game1 m_root;
    private static SpriteFont m_f;

    public static void Init(Game1 root)
    {
        m_root = root;
        m_f = AssetManager.Load<SpriteFont>("fc");
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
            m_f,
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

    public static Vector2 Size(this string input) => m_f.MeasureString(input);
}