#region
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGameOfDoomHmmm.Source.Entity.Components;
using TheGameOfDoomHmmm.Source.Game;
using TheGameOfDoomHmmm.Source.Game.GameState;
using TheGameOfDoomHmmm.Source.Graphics;
#endregion

namespace TheGameOfDoomHmmm.Source.Entity.Entities;

public sealed class Explosion : Entity
{
    public Explosion(GameplayState gameState, Vector2 position, float rotation, float speed)
        : base(gameState, position)
    {
        Velocity = Vector2.UnitX.RotateVector(rotation) * speed;

        Texture2D spriteSheet = AssetManager.Load<Texture2D>("Explosion");

        Animation explode = AnimationCreator.CreateAnimFromSpriteSheet
        (
            96,
            96,
            0,
            12,
            12,
            new[] { 60 },
            false,
            false,
            false,
            false,
            0
        );

        SpriteRenderer = new SpriteRenderer
        (
            this,
            spriteSheet,
            new[] { explode },
            null,
            null,
            GameState.Root
        );

        Collider = new Collider
        (
            this
        )
        {
            Radius = 2
        };

        GameState.CollisionSystem.AddCollider(Collider);

        OutOfBoundsBehavior = OutOfBounds.Destroy;
    }

    internal override void OnUpdate(UpdateEventArgs e)
    {
        if (SpriteRenderer.AnimDone) Destroy();
    }

    internal override void OnCollision(Collider other) { }
}