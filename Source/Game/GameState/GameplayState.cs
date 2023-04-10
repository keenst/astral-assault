#region
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AstralAssault;

public class GameplayState : GameState
{
    public readonly CollisionSystem CollisionSystem = new CollisionSystem();
    public readonly List<Entity> Entities;
    public WaveController WaveController;

    public GameplayState(Game1 root) : base(root)
    {
        Entities = new List<Entity>();
        WaveController = new WaveController(this, Root);
    }

    public Player Player
    {
        get => (Player)Entities.Find(static entity => entity is Player);
    }

    public override List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new List<DrawTask>();

        foreach (Entity entity in Entities) drawTasks.AddRange(entity.GetDrawTasks());

        drawTasks.AddRange(WaveController.GetDrawTasks());

        if (!Root.ShowDebug) return drawTasks;

        foreach (Collider collider in CollisionSystem.Colliders)
        {
            int width = collider.Rectangle.Width;
            int height = collider.Rectangle.Height;

            Texture2D rect = new Texture2D(Root.GraphicsDevice, width, height);

            Color[] data = new Color[width * height];

            Array.Fill(data, new Color(Palette.GetColor(Palette.Colors.Grey9), 0.15F));
            rect.SetData(data);

            drawTasks.Add
            (
                new DrawTask
                (
                    rect,
                    collider.Rectangle.Location.ToVector2(),
                    0,
                    LayerDepth.Debug,
                    new List<IDrawTaskEffect>(),
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
        UpdateEventSource.UpdateEvent += OnUpdate;
    }

    public override void Exit()
    {
        while (Entities.Count > 0) Entities[0].Destroy();
        UpdateEventSource.UpdateEvent -= OnUpdate;
    }

    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        CollisionSystem.OnUpdate(sender, e);
        WaveController.OnUpdate(sender, e);

        for (int i = 0; i < Entities.Count; i++) Entities[i].OnUpdate(sender, e);
    }
}