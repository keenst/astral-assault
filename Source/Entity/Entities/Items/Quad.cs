#region
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGameOfDoomHmmm.Source.Graphics;
using TheGameOfDoomHmmm.Source.Entity.Components;
using TheGameOfDoomHmmm.Source.Game;
using TheGameOfDoomHmmm.Source.Game.GameState;
#endregion

namespace TheGameOfDoomHmmm.Source.Entity.Entities.Items;

internal sealed class Quad : PowerUpBase
{
    public Quad(GameplayState gameState, Vector2 position) : base(gameState, position) { }

    protected override void InitAnimations()
    {
        Texture2D spriteSheet = AssetManager.Load<Texture2D>("Quad");

        Animation animation = AnimationCreator.CreateAnimFromSpriteSheet
        (
            16,
            16,
            0,
            6,
            10,
            new[] { 60 },
            false,
            true,
            false,
            true,
            5
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