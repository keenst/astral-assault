using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class Crosshair : Entity, IMouseEventListener
{
    public Crosshair(Game1 root, Vector2 position) : base(root, position)
    {
        Root = root;

        InputEventSource.MouseButtonEvent += OnMouseButtonEvent;
        InputEventSource.MouseMoveEvent += OnMouseMoveEvent;

        Texture2D spriteSheet = Root.Content.Load<Texture2D>("assets/crosshair");

        Frame frame = new(new Rectangle(0, 0, 16, 16));

        Animation animation = new(new[] { frame }, false);
        
        SpriteRenderer = new SpriteRenderer(spriteSheet, new[] { animation });
    }

    public void OnMouseButtonEvent(object sender, MouseButtonEventArgs e)
    {
        
    }

    public void OnMouseMoveEvent(object sender, MouseMoveEventArgs e)
    {
        Point scale = new((int)Root.ScaleX, (int)Root.ScaleY);
        Position = (e.Position / scale).ToVector2();
    }
}