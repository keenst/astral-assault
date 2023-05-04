#region
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AstralAssault;

public class GameplayState : GameState
{
    private static readonly Vector4 MultiplierBrokenColor = new Vector4(1, 0, 0, 1);
    private static readonly Vector4 MultiplierIncreaseColor = new Vector4(1, 1, 0, 1);
    private static readonly Vector4 MultiplierDefaultColor = new Vector4(1, 1, 1, 1);
    private readonly Texture2D circle;
    public readonly CollisionSystem CollisionSystem = new CollisionSystem();
    public readonly List<Entity> Entities;
    public ItemController ItemController;
    private Vector4 m_multiplierColor = MultiplierDefaultColor;
    private float m_prevMultiplier = 1;
    public WaveController WaveController;


    public GameplayState(Game1 root) : base(root)
    {
        Entities = new List<Entity>();
        ItemController = new ItemController(this);
        WaveController = new WaveController(this, Root);

        circle = AssetManager.Load<Texture2D>("unitcircleofdoomyaaaZ");

        ItemController.StartListening();
    }

    public Player Player
    {
        get => (Player)Entities.Find(entity => entity is Player);
    }

    public override List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new List<DrawTask>();

        foreach (Entity entity in Entities) drawTasks.AddRange(entity.GetDrawTasks());

        if (!Root.ShowDebug)
        {
            drawTasks.AddRange(WaveController.GetDrawTasks());

            string scoreText = $"Score: {Root.Score}";
            Color textColor = Palette.GetColor(Palette.Colors.Grey9);
            ReadOnlySpan<DrawTask> scoreTasks = scoreText.AsSpan().CreateDrawTasks
                (new Vector2(4, 4), textColor, LayerDepth.HUD, false);
            drawTasks.AddRange(scoreTasks.ToArray());
        }


        string multiplierText =
            $"Score multi.: X{Player.Multiplier.ToString("0.0", CultureInfo.GetCultureInfo("en-US"))}";

        ReadOnlySpan<DrawTask> multiplierTasks = multiplierText.AsSpan().CreateDrawTasks
        (
            new Vector2(480 - multiplierText.Size(), 4),
            new Color(m_multiplierColor),
            LayerDepth.HUD, false
        );
        drawTasks.AddRange(multiplierTasks.ToArray());

        if (!Root.ShowDebug) return drawTasks;

        foreach (Collider collider in CollisionSystem.Colliders)
        {
            drawTasks.Add
            (
                new DrawTask
                (
                    circle,
                    collider.Parent.Position - new Vector2(collider.Radius) / 2,
                    0,
                    LayerDepth.Debug,
                    Palette.GetColor(Palette.Colors.Blue9),
                    Vector2.Zero
                )
            );
        }

        return drawTasks;
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
}