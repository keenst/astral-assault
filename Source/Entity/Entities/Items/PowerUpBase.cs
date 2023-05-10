using Microsoft.Xna.Framework;
using TheGameOfDoomHmmm.Source.Entity.Components;
using TheGameOfDoomHmmm.Source.Game.GameState;

namespace TheGameOfDoomHmmm.Source.Entity.Entities.Items;

public abstract class PowerUpBase : Entity
{
    protected PowerUpBase(GameplayState gameState, Vector2 position) : base(gameState, position)
    {
        Collider = new Collider
            (this)
            {
                Radius = 3
            };
        GameState.CollisionSystem.AddCollider(Collider);

        InitAnimations();
    }

    protected abstract void InitAnimations();

    internal override void OnCollision(Collider other)
    {
        if (other.Parent is Player) Destroy();
    }
}