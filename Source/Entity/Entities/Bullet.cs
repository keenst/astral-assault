﻿#region
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AstralAssault;

public class Bullet : Entity
{
    public Bullet(GameplayState gameState, Vector2 position, float rotation, float speed, bool isQuadDamage)
        : base(gameState, position)
    {
        IsQuadDamage = isQuadDamage;

        Velocity = Vector2.UnitX.RotateVector(rotation) * speed;

        Texture2D spriteSheet = AssetManager.Load<Texture2D>("Bullet");

        Frame frame = new Frame(new Rectangle(isQuadDamage ? 4 : 0, 0, 4, 4));

        SpriteRenderer = new SpriteRenderer(spriteSheet, frame, LayerDepth.Foreground);

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
        IsFriendly = true;
    }

    public bool IsQuadDamage { get; }

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