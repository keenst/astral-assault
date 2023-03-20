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
    
    public Player Player => (Player) Entities.Find(entity => entity is Player);

    private readonly ParticleEmitter _particleEmitter;

    public GameplayState(Game1 root) : base(root)
    {
        Entities = new List<Entity>();
        WaveController = new WaveController(this, Root);

        Texture2D spriteSheet = AssetManager.Load<Texture2D>("particle");
        
        Rectangle[] textureSources =
        {
            new(24, 0, 8, 8),
            new(16, 0, 8, 8),
            new(8, 0, 8, 8),
            new(0, 0, 8, 8)
        };

        IParticleProperty[] particleProperties =
        {
            new CauseOfDeathProperty(CauseOfDeathProperty.CausesOfDeath.LifeSpan, 1500),
            new ColorChangeProperty(
                new[]
                {
                    Palette.Colors.Blue7,
                    Palette.Colors.Blue6,
                    Palette.Colors.Blue5,
                    Palette.Colors.Blue4,
                    Palette.Colors.Blue3
                },
                200),
            new SpriteChangeProperty(0, textureSources.Length, 200),
            new VelocityProperty(0, (float)Math.PI * 2, 0.04F, 0.1F)
        };
        
        _particleEmitter = new ParticleEmitter(
            spriteSheet,
            textureSources,
            5,
            new Vector2(Game1.TargetWidth / 2F, Game1.TargetHeight / 2F),
            particleProperties
        );
        
        _particleEmitter.StartSpawning(100);
    }

    public override List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new();
        
        foreach (Entity entity in Entities)
        {
            drawTasks.AddRange(entity.GetDrawTasks());
        }

        drawTasks.AddRange(WaveController.GetDrawTasks());
        
        drawTasks.AddRange(_particleEmitter.CreateDrawTasks());

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
    }

    public override void Exit()
    {
        while (Entities.Count > 0) Entities[0].Destroy();
        _particleEmitter.StopListening();
    }
}