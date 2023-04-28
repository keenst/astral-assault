#region
using AstralAssault.Source.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AstralAssault;

public class Crosshair : Entity, IMouseEventListener
{
    public Crosshair(GameplayState gameState) : base(gameState, new Vector2(0, 0))
    {
        InputEventSource.MouseButtonEvent += OnMouseButtonEvent;
        InputEventSource.MouseMoveEvent += OnMouseMoveEvent;

        Texture2D spriteSheet = AssetManager.Load<Texture2D>("Crosshair");

        Animation activateAnimation = AnimationCreator.CreateAnimFromSpriteSheet
        (
            16,
            16,
            3,
            5,
            3,
            new[] { 30, 20, 10 },
            false,
            false,
            true,
            false,
            0
        );

        Animation deactivateAnimation = AnimationCreator.CreateAnimFromSpriteSheet
        (
            16,
            16,
            1,
            5,
            3,
            new[] { 30, 20, 10 },
            false,
            false,
            false,
            false,
            0
        );

        Animation activeAnimation = AnimationCreator.CreateAnimFromSpriteSheet
        (
            16,
            16,
            0,
            5,
            1,
            new[] { 0 },
            false,
            false,
            false,
            false,
            0
        );

        Animation inactiveAnimation = AnimationCreator.CreateAnimFromSpriteSheet
        (
            16,
            16,
            4,
            5,
            1,
            new[] { 0 },
            false,
            false,
            false,
            false,
            0
        );

        Transition[] transitions =
        {
            new Transition(0, 1, new[] { 2, 1 }, "IsActive", 0),
            new Transition(1, 0, new[] { 3, 0 }, "IsActive", 1)
        };

        SpriteRenderer = new SpriteRenderer
        (
            spriteSheet,
            new[] { activeAnimation, inactiveAnimation, deactivateAnimation, activateAnimation },
            LayerDepth.Crosshair,
            transitions,
            new[] { "IsActive" }
        );

        OutOfBoundsBehavior = OutOfBounds.DoNothing;
    }

    public void OnMouseButtonEvent(object sender, MouseButtonEventArgs e) { }

    public void OnMouseMoveEvent(object sender, MouseMoveEventArgs e)
    {
        Point scale = new Point((int)GameState.Root.ScaleX, (int)GameState.Root.ScaleY);
        Position = (e.Position / scale).ToVector2();
    }

    public override void Destroy()
    {
        InputEventSource.MouseButtonEvent -= OnMouseButtonEvent;
        InputEventSource.MouseMoveEvent -= OnMouseMoveEvent;

        base.Destroy();
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        if (GameState.Player == null) return;

        Vector2 playerPosition = GameState.Player.Position;
        float distance = Vector2.Distance(playerPosition, Position);

        SpriteRenderer.SetAnimationCondition("IsActive", distance < 12 ? 0 : 1);
    }
}