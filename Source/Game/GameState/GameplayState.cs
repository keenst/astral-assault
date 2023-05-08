#region
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using TheGameOfDoomHmmm.Source.Entity.Components;
using TheGameOfDoomHmmm.Source.Entity.Entities;
using TheGameOfDoomHmmm.Source.Graphics;
#endregion

namespace TheGameOfDoomHmmm.Source.Game.GameState;

public sealed class GameplayState : GameState
{
    private static readonly Vector4 MultiplierBrokenColor = new Vector4(1, 0, 0, 1);
    private static readonly Vector4 MultiplierIncreaseColor = new Vector4(1, 1, 0, 1);
    private static readonly Vector4 MultiplierDefaultColor = new Vector4(1, 1, 1, 1);
    public readonly CollisionSystem CollisionSystem = new CollisionSystem();
    public readonly List<Entity.Entities.Entity> Entities;
    public ItemController ItemController;
    private Vector4 m_multiplierColor = MultiplierDefaultColor;
    private float m_prevMultiplier = 1;
    public WaveController WaveController;


    public GameplayState(Game1 root) : base(root)
    {
        Entities = new List<Entity.Entities.Entity>();
        ItemController = new ItemController(this);
        WaveController = new WaveController(this, Root);

        ItemController.StartListening();
    }

    public Player Player
    {
        get => (Player)Entities.Find(entity => entity is Player);
    }

    public override void Enter()
    {
        Entities.Add(new Player(this, new Vector2(Game1.TargetWidth / 2F, Game1.TargetHeight / 2F)));
        Entities.Add(new Crosshair(this));
        Root.Score = 0;

        UpdateEventSource.UpdateEvent += OnUpdate;
    }

    public override void Exit()
    {
        ItemController.StopListening();
        while (Entities.Count > 0) Entities[0].Destroy();

        UpdateEventSource.UpdateEvent -= OnUpdate;
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        if (Player == null) return;

        float multiplier = Player.Multiplier;

        if (multiplier != m_prevMultiplier)
        {
            m_multiplierColor = multiplier > m_prevMultiplier ? MultiplierIncreaseColor : MultiplierBrokenColor;
            m_prevMultiplier = multiplier;
        }
        else m_multiplierColor = Vector4.Lerp(m_multiplierColor, MultiplierDefaultColor, e.DeltaTime * 2);

        CollisionSystem.OnUpdate(sender, e);
        WaveController.OnUpdate(sender, e);

        for (int i = 0; i < Entities.Count; i++) Entities[i].OnUpdate(sender, e);
    }

    public override void Draw()
    {
        foreach (Entity.Entities.Entity entity in Entities) entity.Draw();

        if (!Root.ShowDebug)
        {
            WaveController.Draw();

            string scoreText = $"Score: {Root.Score}";
            Color textColor = Palette.GetColor(Palette.Colors.Grey9);
            scoreText.Draw(Vector2.Zero, textColor, 0f, new Vector2(0, 0), 1f, LayerOrdering.Hud);
        }
        else
        {
            foreach (Collider collider in CollisionSystem.Colliders)
            {
                Root.SpriteBatch.DrawCircle
                (
                    collider.Parent.Position, collider.Radius, 32, Palette.GetColor(Palette.Colors.Blue9), 1f,
                    LayerOrdering.Debug.GetDisplayLayerValue()
                );
            }
        }


        string multiplierText =
            $"Score multi.: X{Player.Multiplier.ToString("0.0", CultureInfo.GetCultureInfo("en-US"))}";

        multiplierText.Draw
        (
            new Vector2(480 - multiplierText.Size().X, 0),
            new Color(m_multiplierColor),
            0f,
            new Vector2(0, 0),
            1f,
            LayerOrdering.Hud
        );
    }
}