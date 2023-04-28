#region
using AstralAssault.Source.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AstralAssault.Items;

public class MegaHealth : Entity
{
    public MegaHealth(GameplayState gameState, Vector2 position) : base(gameState, position)
    {
        IsFriendly = true;

        Collider = new Collider
            (this, new Rectangle(new Point((int)Position.X - 8, (int)Position.Y - 8), new Point(16, 16)))
            {
                Radius = 3
            };
        GameState.CollisionSystem.AddCollider(Collider);

        Texture2D spriteSheet = AssetManager.Load<Texture2D>("MegaHealth");

        Animation animation = AnimationCreator.CreateAnimFromSpriteSheet
        (
            16,
            16,
            0,
            5,
            8,
            new[] { 60 },
            false,
            true,
            false,
            true,
            4
        );

        SpriteRenderer = new SpriteRenderer
        (
            spriteSheet,
            new[] { animation },
            LayerDepth.Foreground,
            null,
            null
        );
    }

    public override void OnCollision(Collider other)
    {
        if (other.Parent is Player) Destroy();
    }
}