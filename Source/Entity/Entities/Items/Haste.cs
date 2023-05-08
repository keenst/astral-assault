#region
using AstralAssault.Source.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AstralAssault.Items;

internal sealed class Haste : Entity
{
    public Haste(GameplayState gameState, Vector2 position) : base(gameState, position)
    {
        Collider = new Collider
            (this)
            {
                Radius = 3
            };
        GameState.CollisionSystem.AddCollider(Collider);

        Texture2D spriteSheet = AssetManager.Load<Texture2D>("Haste");

        Animation animation = AnimationCreator.CreateAnimFromSpriteSheet
        (
            16,
            16,
            0,
            18,
            18,
            new[] { 60 },
            false,
            true,
            false,
            false,
            0
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