using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class Crosshair : Entity
{
    public Crosshair(GameplayState gameState) : base(gameState, new Vector2(0, 0))
    {
        Texture2D spriteSheet = AssetManager.Load<Texture2D>("Crosshair");

        Frame activeFrame   = new(new Rectangle(0,  0, 16, 16));
        Frame inactiveFrame = new(new Rectangle(16, 0, 16, 16));
        
        Animation activeAnimation   = new(new[] { activeFrame },   false);
        Animation inactiveAnimation = new(new[] { inactiveFrame }, false);

        SpriteRenderer = new SpriteRenderer(
            spriteSheet, 
            new[] { activeAnimation, inactiveAnimation }, 
            LayerDepth.Crosshair);

        OutOfBoundsBehavior = OutOfBounds.DoNothing;
    }

    public override void Update(float deltaTime)
    {
        if (GameState.Player == null) return;
        
        HandleMouseMove();
        
        Vector2 playerPosition = GameState.Player.Position;
        float distance = Vector2.Distance(playerPosition, Position);
        if (distance < 12) SpriteRenderer.PlayAnimation(1);
        else if (SpriteRenderer.ActiveAnimationIndex != 0) SpriteRenderer.PlayAnimation(0);
    }

    public void HandleMouseMove()
    {
        Point scale = new((int)GameState.Root.ScaleX, (int)GameState.Root.ScaleY);
        Position = (GameState.Root.InputCollector._mousePos / scale).ToVector2();
    }
}