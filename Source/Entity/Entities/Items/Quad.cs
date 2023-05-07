#region
using AstralAssault.Source.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AstralAssault.Items;

public class Quad : Entity
{
    public Quad(GameplayState gameState, Vector2 position) : base(gameState, position)
    {
        Collider =
            new Collider(this, new Rectangle(new Point((int)Position.X - 8, (int)Position.Y - 8), new Point(16, 16)))
            {
                Radius = 3
            };
        GameState.CollisionSystem.AddCollider(Collider);

        Texture2D spriteSheet = AssetManager.Load<Texture2D>("Quad");

        Animation animation = AnimationCreator.CreateAnimFromSpriteSheet
        (
            16,
            16,
            0,
            6,
            10,
            new[] { 60 },
            false,
            true,
            false,
            true,
            5
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