#region
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

        Animation nothing = new Animation
        (
            new[] { new Frame(new Rectangle(0, 0, 4, 4)) }, false, true
        );

        Animation explode = AnimationCreator.CreateAnimFromSpriteSheet
        (
            96,
            96,
            0,
            12,
            12,
            new[] { 30 },
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
            new[] { nothing, explode },
            new[]
            {
                new Transition(1, 0, new[] { 0 }, "lel", 0),
                new Transition(0, 1, new[] { 1 }, "lel", 1),
            },
            new[] { "lel" },
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

        SpriteRenderer.SetAnimationCondition("lel", 1);
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        if (SpriteRenderer.AnimDone) Destroy();
    }

    public override void OnCollision(Collider other) { }
}