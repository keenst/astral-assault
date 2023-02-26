using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class Crosshair : Entity, IMouseEventListener
{
    private Game1 _root;
    
    public Crosshair(Game1 root, Vector2 position) : base(position)
    {
        _root = root;

        InputEventSource.MouseButtonEvent += OnMouseButtonEvent;
        InputEventSource.MouseMoveEvent += OnMouseMoveEvent;

        Texture2D spriteSheet = _root.Content.Load<Texture2D>("assets/crosshair");

        Frame frame = new(new Rectangle(0, 0, 16, 16));

        Animation animation = new(new[] { frame }, false);
        
        _spriteRenderer = new SpriteRenderer(spriteSheet, new[] { animation });
    }

    public void OnMouseButtonEvent(object sender, MouseButtonEventArgs e)
    {
        
    }

    public void OnMouseMoveEvent(object sender, MouseMoveEventArgs e)
    {
        Point scale = new((int)_root.ScaleX, (int)_root.ScaleY);
        _position = (e.Position / scale).ToVector2();
    }
}