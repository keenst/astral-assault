using System;
using System.Collections.Generic;
using AstralAssault.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class GameplayState : GameState
{
    public readonly List<Entity> Entities;
    public readonly CollisionSystem CollisionSystem = new();
    public WaveController WaveController;
    public ItemController ItemController;

    public Player Player => (Player) Entities.Find(entity => entity is Player);

    public GameplayState(Game1 root) : base(root)
    {
        Entities = new List<Entity>();
        WaveController = new WaveController(this, Root);
        ItemController = new ItemController(this);
        ItemController.StartListening();
    }

    public override List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new();
        
        foreach (Entity entity in Entities)
        {
            drawTasks.AddRange(entity.GetDrawTasks());
        }

        drawTasks.AddRange(WaveController.GetDrawTasks());

        string scoreText = $"Score: {Root.Score}";
        Color textColor = Palette.GetColor(Palette.Colors.Grey9);
        List<DrawTask> scoreTasks = scoreText.CreateDrawTasks(new Vector2(4, 4), textColor, LayerDepth.HUD);
        drawTasks.AddRange(scoreTasks);
        
        if (!Root.ShowDebug) return drawTasks;
        
        foreach (Collider collider in CollisionSystem.Colliders)
        {
            int width = collider.Rectangle.Width;
            int height = collider.Rectangle.Height;
                
            Texture2D rect = new(Root.GraphicsDevice, width, height);

            Color[] data = new Color[width * height];
                
            Array.Fill(data, new Color(Color.White, 0.2F));
            rect.SetData(data);
            
            drawTasks.Add(new DrawTask(
                rect, 
                collider.Rectangle.Location.ToVector2(), 
                0, 
                LayerDepth.Debug, 
                new List<IDrawTaskEffect>(),
                Color.Blue,
                Vector2.Zero));
        }

        return drawTasks;
    }

    public override void Enter()
    {
        Entities.Add(new Player(this, new Vector2(Game1.TargetWidth / 2F, Game1.TargetHeight / 2F)));
        Entities.Add(new Crosshair(this));
        Entities.Add(new Quad(this, new Vector2(100, 100)));
        Entities.Add(new Haste(this, new Vector2(200, 200)));
        Entities.Add(new MegaHealth(this, new Vector2(380, 200)));
        Root.Score = 0;
    }

    public override void Exit()
    {
        WaveController.StopListening();
        ItemController.StopListening();
        while (Entities.Count > 0) Entities[0].Destroy();
    }
}