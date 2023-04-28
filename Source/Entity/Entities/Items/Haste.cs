#region
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AstralAssault.Items;

public class Haste : Entity
{
    public Haste(GameplayState gameState, Vector2 position) : base(gameState, position)
    {
        IsFriendly = true;

        Collider = new Collider
            (this, new Rectangle(new Point((int)Position.X - 8, (int)Position.Y - 8), new Point(16, 16)))
            {
                Radius = 3
            };
        GameState.CollisionSystem.AddCollider(Collider);

        Texture2D spriteSheet = AssetManager.Load<Texture2D>("Haste");

        Frame[] frames =
        {
            new Frame(new Rectangle(16 * 0, 0, 16, 16), 60), new Frame(new Rectangle(16 * 1, 0, 16, 16), 60),
            new Frame(new Rectangle(16 * 2, 0, 16, 16), 60), new Frame(new Rectangle(16 * 3, 0, 16, 16), 60),
            new Frame(new Rectangle(16 * 4, 0, 16, 16), 60), new Frame(new Rectangle(16 * 5, 0, 16, 16), 60),
            new Frame(new Rectangle(16 * 6, 0, 16, 16), 60), new Frame(new Rectangle(16 * 7, 0, 16, 16), 60),
            new Frame(new Rectangle(16 * 8, 0, 16, 16), 60), new Frame(new Rectangle(16 * 9, 0, 16, 16), 60),
            new Frame(new Rectangle(16 * 10, 0, 16, 16), 60), new Frame(new Rectangle(16 * 11, 0, 16, 16), 60),
            new Frame(new Rectangle(16 * 12, 0, 16, 16), 60), new Frame(new Rectangle(16 * 13, 0, 16, 16), 60),
            new Frame(new Rectangle(16 * 14, 0, 16, 16), 60), new Frame(new Rectangle(16 * 15, 0, 16, 16), 60),
            new Frame(new Rectangle(16 * 16, 0, 16, 16), 60), new Frame(new Rectangle(16 * 17, 0, 16, 16), 60)
        };

        Animation animation = new Animation(frames, false, true);

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