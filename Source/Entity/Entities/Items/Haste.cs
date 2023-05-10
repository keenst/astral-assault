#region
using TheGameOfDoomHmmm.Source.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGameOfDoomHmmm.Source.Entity.Components;
using TheGameOfDoomHmmm.Source.Game;
using TheGameOfDoomHmmm.Source.Game.GameState;
#endregion

namespace TheGameOfDoomHmmm.Source.Entity.Entities.Items;

internal sealed class Haste : PowerUpBase
{
    public Haste(GameplayState gameState, Vector2 position) : base(gameState, position) { }

    protected override void InitAnimations()
    {
        Texture2D spriteSheet = AssetManager.Load<Texture2D>("Haste");

        Animation animation = AnimationCreator.CreateAnimFromSpriteSheet
        (
            16,
            16,
            0,
            18,
            18,
            new[] { 60 },
            false,
            true,
            false,
            false,
            0
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