using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class GameplayState : GameState
{
    public readonly List<Entity> Entities;
    public readonly CollisionSystem CollisionSystem = new();
    public WaveController WaveController;

    public Player Player => (Player)Entities.Find(entity => entity is Player);

    public GameplayState(Game1 root) : base(root)
    {
        Entities = new();
        WaveController = new(this, Root);
    }

    public override List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new();

        foreach (Entity entity in Entities)
        {
            drawTasks.AddRange(entity.GetDrawTasks());
        }

        drawTasks.AddRange(WaveController.GetDrawTasks());

        if (!Root.ShowDebug) return drawTasks;

        foreach (Collider collider in CollisionSystem.Colliders)
        {
            int width = collider.Rectangle.Width;
            int height = collider.Rectangle.Height;

            Texture2D rect = new(Root.GraphicsDevice, width, height);

            Color[] data = new Color[width * height];

            Array.Fill(data, new(Color.White, 0.2F));
            rect.SetData(data);

            drawTasks.Add(new(
                rect,
                collider.Rectangle.Location.ToVector2(),
                0,
                LayerDepth.Debug,
                new(),
                Color.Blue,
                Vector2.Zero));
        }

        return drawTasks;
    }

    public override void Enter()
    {
        Entities.Add(new Player(this, new(Game1.TargetWidth / 2F, Game1.TargetHeight / 2F)));
        Entities.Add(new Crosshair(this));
    }

    public override void Exit()
    {
        WaveController.StopListening();
        while (Entities.Count > 0) Entities[0].Destroy();
    }
}