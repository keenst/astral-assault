using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault.Items;

public class MegaHealth : Entity
{
    public MegaHealth(GameplayState gameState, Vector2 position) : base(gameState, position)
    {
        IsFriendly = true;
        
        Collider = new Collider(this, new Rectangle(new Point((int)Position.X - 8, (int)Position.Y - 8), new Point(16, 16)));
        GameState.CollisionSystem.AddCollider(Collider);
        
        Texture2D spriteSheet = AssetManager.Load<Texture2D>("MegaHealth");

        Frame[] frames = {
            new(new Rectangle(0, 0, 16, 16), 60),
            new(new Rectangle(16, 0, 16, 16), 60),
            new(new Rectangle(32, 0, 16, 16), 60),
            new(new Rectangle(48, 0, 16, 16), 60),
            new(new Rectangle(64, 0, 16, 16), 60),
            new(new Rectangle(48, 0, 16, 16), 60),
            new(new Rectangle(32, 0, 16, 16), 60),
            new(new Rectangle(16, 0, 16, 16), 60)
        };
        
        Animation animation = new(frames, false, true);

        SpriteRenderer = new SpriteRenderer(
            spriteSheet, 
            new Animation[] { animation }, 
            LayerDepth.Foreground,
            null,
            null);
    }
    
    public override void OnCollision(Collider other)
    {
        if (other.Parent is Player)
        {
            Destroy();
        }
    }
}