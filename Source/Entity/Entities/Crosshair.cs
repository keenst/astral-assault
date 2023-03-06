using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class Crosshair : Entity, IMouseEventListener
{
    public Crosshair(MainGameState gameState) : base(gameState, new Vector2(0, 0))
    {
        InputEventSource.MouseButtonEvent += OnMouseButtonEvent;
        InputEventSource.MouseMoveEvent += OnMouseMoveEvent;

        Texture2D spriteSheet = _gameState.Root.Content.Load<Texture2D>("assets/crosshair");

        Frame frame = new(new Rectangle(0, 0, 16, 16));

        Animation animation = new(new[] { frame }, false);
        
        SpriteRenderer = new SpriteRenderer(spriteSheet, new[] { animation });

        OutOfBoundsBehavior = OutOfBounds.DoNothing;
    }

    public void OnMouseButtonEvent(object sender, MouseButtonEventArgs e)
    {
        
    }

    public void OnMouseMoveEvent(object sender, MouseMoveEventArgs e)
    {
        Point scale = new((int)_gameState.Root.ScaleX, (int)_gameState.Root.ScaleY);
        Position = (e.Position / scale).ToVector2();
    }
}