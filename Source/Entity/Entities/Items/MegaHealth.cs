#region
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGameOfDoomHmmm.Source.Entity.Components;
using TheGameOfDoomHmmm.Source.Game;
using TheGameOfDoomHmmm.Source.Game.GameState;
using TheGameOfDoomHmmm.Source.Graphics;
#endregion

namespace TheGameOfDoomHmmm.Source.Entity.Entities.Items;

internal sealed class MegaHealth : PowerUpBase
{
    public MegaHealth(GameplayState gameState, Vector2 position) : base(gameState, position) { }

    protected override void InitAnimations()
    {
        Texture2D spriteSheet = AssetManager.Load<Texture2D>("MegaHealth");

        Animation animation = AnimationCreator.CreateAnimFromSpriteSheet
        (
            16,
            16,
            0,
            5,
            8,
            new[] { 60 },
            false,
            true,
            false,
            true,
            4
        );

        SpriteRenderer = new SpriteRenderer
        (
            this,
            spriteSheet,
            new[] { animation },
            null,
            null,
            GameState.Root
        );
    }
}