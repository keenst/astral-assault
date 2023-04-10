using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class Bullet : Entity
{
    public Bullet(GameplayState gameState, Vector2 position, float rotation, float speed) : base(gameState, position)
    {
        Velocity = Vector2.UnitX.RotateVector(rotation) * speed;

        Texture2D spriteSheet = new Texture2D(GameState.Root.GraphicsDevice, 2, 2);

        Color[] data = new Color[2 * 2];
        for (int i = 0; i < data.Length; ++i) data[i] = Palette.GetColor(Palette.Colors.Grey9);
        spriteSheet.SetData(data);

        Frame frame = new Frame(new Rectangle(0, 0, 2, 2));

        SpriteRenderer = new SpriteRenderer(spriteSheet, frame, LayerDepth.Foreground);
        
        Collider = new Collider(
            this, 
            new Rectangle(
                new Point((int)Position.X - 1, (int)Position.Y - 1), 
                new Point(2, 2)));
        GameState.CollisionSystem.AddCollider(Collider);

        OutOfBoundsBehavior = OutOfBounds.Destroy;

        ContactDamage = 4;
        IsFriendly = true;
    }

    public override void OnCollision(Collider other)
    {
        if (IsFriendly == other.Parent.IsFriendly) return;

        Destroy();
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        base.OnUpdate(sender, e);

        if (Position.X is > Game1.TargetWidth or < 0 ||
            Position.Y is > Game1.TargetHeight or < 0) Destroy();
    }
}