#region
using AstralAssault.Source.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AstralAssault.Items;

internal sealed class MegaHealth : Entity
{
    public MegaHealth(GameplayState gameState, Vector2 position) : base(gameState, position)
    {
        Collider = new Collider
            (this)
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
            this,
            spriteSheet,
            new[] { animation },
            null,
            null,
            gameState.Root
        );
    }

    public override void OnCollision(Collider other)
    {
        if (other.Parent is Player) Destroy();
    }
}