using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public class GameplayState : GameState, IUpdateEventListener
{
    public readonly List<Entity> Entities;
    public readonly CollisionSystem CollisionSystem = new();
    public EnemySpawner EnemySpawner;

    public Player Player => (Player) Entities.Find(entity => entity is Player);
    public int EnemiesAlive => Entities.Count(entity => entity is not AstralAssault.Player and not Crosshair);

    private static readonly Vector4 MultiplierBrokenColor = new(1, 0, 0, 1);
    private static readonly Vector4 MultiplierIncreaseColor = new(1, 1, 0, 1);
    private static readonly Vector4 MultiplierDefaultColor = new(1, 1, 1, 1);
    private Vector4 _multiplierColor = MultiplierDefaultColor;
    private float _prevMultiplier = 1;
    
    public readonly DebrisController DebrisController;
    public readonly ExplosionController ExplosionController = new();
    
    public GameplayState(Game1 root) : base(root)
    {
        Entities = new List<Entity>();
        EnemySpawner = new EnemySpawner(this);
        DebrisController = new DebrisController(this);
    }

    public override List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new();
        
        foreach (Entity entity in Entities)
        {
            drawTasks.AddRange(entity.GetDrawTasks());
        }

        drawTasks.AddRange(EnemySpawner.GetDrawTasks());
        drawTasks.AddRange(DebrisController.GetDrawTasks());
        drawTasks.AddRange(ExplosionController.GetDrawTasks());
        drawTasks.AddRange(GetScoreDrawTasks());

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
        EnemySpawner.StopListening();
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

    private List<DrawTask> GetScoreDrawTasks()
    {
        List<DrawTask> drawTasks = new();
        
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

        return drawTasks;
    }
}