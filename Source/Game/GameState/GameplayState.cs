using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class GameplayState : GameState, IUpdateEventListener
{
    public readonly List<Entity> Entities;
    public readonly CollisionSystem CollisionSystem = new();
    public WaveController WaveController;
    public ItemController ItemController;

    public Player Player => (Player) Entities.Find(entity => entity is Player);

    private static readonly Vector4 MultiplierBrokenColor = new(1, 0, 0, 1);
    private static readonly Vector4 MultiplierIncreaseColor = new(1, 1, 0, 1);
    private static readonly Vector4 MultiplierDefaultColor = new(1, 1, 1, 1);
    private Vector4 _multiplierColor = MultiplierDefaultColor;
    private float _prevMultiplier = 1;
    
    
    public GameplayState(Game1 root) : base(root)
    {
        Entities = new List<Entity>();
        ItemController = new ItemController(this);
        WaveController = new WaveController(this, Root);
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
        
        string multiplierText = 
            $"Score multi.: X{Player.Multiplier.ToString("0.0", CultureInfo.GetCultureInfo("en-US"))}";
        
        List<DrawTask> multiplierTasks = multiplierText.CreateDrawTasks(
            new Vector2(480 - multiplierText.Length * 8 - 4, 4), 
            textColor, 
            LayerDepth.HUD,
            new List<IDrawTaskEffect> { new ColorEffect(_multiplierColor) });
        drawTasks.AddRange(multiplierTasks);

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
        Root.Score = 0;

        UpdateEventSource.UpdateEvent += OnUpdate;
    }

    public override void Exit()
    {
        WaveController.StopListening();
        ItemController.StopListening();
        while (Entities.Count > 0) Entities[0].Destroy();
        
        UpdateEventSource.UpdateEvent -= OnUpdate;
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        if (Player == null) return;
        
        float multiplier = Player.Multiplier;
        
        if (multiplier != _prevMultiplier)
        {
            _multiplierColor = multiplier > _prevMultiplier ? MultiplierIncreaseColor : MultiplierBrokenColor;
            _prevMultiplier = multiplier;
        }
        else
        {
            _multiplierColor = Vector4.Lerp(_multiplierColor, MultiplierDefaultColor, e.DeltaTime * 2);
        }
    }
}