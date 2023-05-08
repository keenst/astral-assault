using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGameOfDoomHmmm.Source.Entity.Components;
using TheGameOfDoomHmmm.Source.Game;
using TheGameOfDoomHmmm.Source.Game.GameState;
using TheGameOfDoomHmmm.Source.Graphics;

namespace TheGameOfDoomHmmm.Source.Entity.Entities.Items;

public abstract class PowerUpBase : Entity
{
    public PowerUpBase(GameplayState gameState, Vector2 position) : base(gameState, position)
    {
        Collider = new Collider
            (this)
            {
                Radius = 3
            };
        GameState.CollisionSystem.AddCollider(Collider);

        InitAnimations();
    }

    public abstract void InitAnimations();

    public override void OnCollision(Collider other)
    {
        if (other.Parent is Player) Destroy();
    }
}