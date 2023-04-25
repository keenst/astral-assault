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

    Texture2D createCircleText(int radius, Color color)
    {
        Texture2D texture = new Texture2D(Root.GraphicsDevice, radius, radius);
        Color[] colorData = new Color[radius * radius];

        float diam = radius / 2f;
        float diamsq = diam * diam;

        for (int x = 0; x < radius; x++)
        {
            for (int y = 0; y < radius; y++)
            {
                int index = x * radius + y;
                Vector2 pos = new Vector2(x - diam, y - diam);

                if (pos.LengthSquared() <= diamsq)
                {
                    colorData[index] = color;
                }
                else
                {
                    colorData[index] = Color.Transparent;
                }
            }
        }

        texture.SetData(colorData);

        return texture;
    }

    public override List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new List<DrawTask>();

        foreach (Entity entity in Entities) drawTasks.AddRange(entity.GetDrawTasks());

        drawTasks.AddRange(WaveController.GetDrawTasks());

        if (!Root.ShowDebug) return drawTasks;

        foreach (Collider collider in CollisionSystem.Colliders)
        {
            Texture2D circle = createCircleText(collider.radius, new Color(Palette.GetColor(Palette.Colors.Grey9), 0.15F));

            drawTasks.Add
            (
                new DrawTask
                (
                    circle,
                    collider.Parent.Position - (new Vector2(collider.radius) / 2),
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