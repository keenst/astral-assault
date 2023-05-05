#region
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using AstralAssault.Source.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
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

    [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: Microsoft.Xna.Framework.Vector2[]; size: 111MB")]
    public override void Draw()
    {
        foreach (Entity entity in Entities) entity.Draw();

        if (!Root.ShowDebug)
        {
            WaveController.Draw();

            string scoreText = $"Score: {Root.Score}";
            Color textColor = Palette.GetColor(Palette.Colors.Grey9);
            scoreText.Draw
                (new Vector2(4, 4), textColor, 0f, new Vector2(0, 0), 1f, LayerOrdering.Hud);
        }
        else
        {
            foreach (Collider collider in CollisionSystem.Colliders)
            {
                Root.m_spriteBatch.DrawCircle
                (
                    collider.Parent.Position, collider.Radius, 32, Palette.GetColor(Palette.Colors.Blue9), 1f, LayerOrdering.Debug.GetDisplayLayerValue()
                );
            }
        }


        string multiplierText =
            $"Score multi.: X{Player.Multiplier.ToString("0.0", CultureInfo.GetCultureInfo("en-US"))}";

        multiplierText.Draw
        (
            new Vector2(480 - multiplierText.Size().X, 4),
            new Color(m_multiplierColor),
            0f,
            new Vector2(0, 0),
            1f,
            LayerOrdering.Hud
        );
    }
}