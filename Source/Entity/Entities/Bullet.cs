#region
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGameOfDoomHmmm.Source.Entity.Components;
using TheGameOfDoomHmmm.Source.Game;
using TheGameOfDoomHmmm.Source.Game.GameState;
#endregion

namespace TheGameOfDoomHmmm.Source.Entity.Entities;

public sealed class Bullet : Entity
{
    public Entity m_shootBy;

    public Bullet(GameplayState gameState, Vector2 position, float rotation, float speed, bool isQuadDamage,
        Entity shootBy)
        : base(gameState, position)
    {
        IsQuadDamage = isQuadDamage;
        m_shootBy = shootBy;

        Velocity = Vector2.UnitX.RotateVector(rotation) * speed;

        Texture2D spriteSheet = AssetManager.Load<Texture2D>("Bullet");

        Frame frame = new Frame(new Rectangle(isQuadDamage ? 4 : 0, 0, 4, 4));

        SpriteRenderer = new SpriteRenderer(this, spriteSheet, frame, GameState.Root);

        Collider = new Collider
        (
            this
        )
        {
            Radius = 3
        };

        GameState.CollisionSystem.AddCollider(Collider);

        OutOfBoundsBehavior = OutOfBounds.Destroy;

        ContactDamage = isQuadDamage ? 16 : 4;
    }

    public bool IsQuadDamage { get; }

    public override void OnCollision(Collider other)
    {
        if (Game1.PatternThing(this, other)) return;

        Destroy();
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        base.OnUpdate(sender, e);

        if (Position.X is > Game1.TargetWidth or < 0 ||
            Position.Y is > Game1.TargetHeight or < 0) Destroy();
    }
}