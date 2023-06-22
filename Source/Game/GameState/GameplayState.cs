using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AstralAssault.Background;
using AstralAssault.Source.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AstralAssault;

public class GameplayState : GameState
{
    public readonly List<Entity> Entities;
    public readonly CollisionSystem CollisionSystem = new();
    private readonly WaveController _waveController;
    private readonly BackgroundRenderer _backgroundRenderer;
    
    public Player Player => (Player) Entities.Find(entity => entity is Player);
    
    private readonly MenuController _pauseMenuController;
    public bool IsPaused { get; private set; }

    public GameplayState(Game1 root) : base(root)
    {
        _backgroundRenderer = new BackgroundRenderer(root);
        
        Entities = new List<Entity>();
        
        _waveController = new WaveController(this);
        
        string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        string json = File.ReadAllText(path + "/Content/Menus/Pause.json");

        Menu pauseMenu = Menu.Parse(Root, json);
        _pauseMenuController = new MenuController(pauseMenu, this);
        _pauseMenuController.Close();
    }

    public override List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new();
        
        foreach (Entity entity in Entities)
        {
            drawTasks.AddRange(entity.GetDrawTasks());
        }

        drawTasks.AddRange(_waveController.GetDrawTasks());
        drawTasks.AddRange(_pauseMenuController.GetDrawTasks());
        drawTasks.AddRange(_backgroundRenderer.GetDrawTasks());
        
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

    public override void Update(UpdateEventArgs e)
    {
        if (!IsPaused)
        {
            List<Entity> entitiesToUpdate = new(Entities);
            while (entitiesToUpdate.Count > 0)
            {
                entitiesToUpdate[0].Update(e);
                entitiesToUpdate.RemoveAt(0);
            }
            
            CollisionSystem.Update(e);
            _waveController.Update(e.DeltaTime);
            _backgroundRenderer.Update(e.DeltaTime);
        }

        HandleKeyboardInputs(e.KeysPressed);
    }

    public override void Enter()
    {
        Entities.Add(new Player(this, new Vector2(Game1.TargetWidth / 2F, Game1.TargetHeight / 2F)));
        Entities.Add(new Crosshair(this));
    }

    public override void Exit()
    {
        while (Entities.Count > 0) Entities[0].Destroy();
    }

    private void HandleKeyboardInputs(Keys[] keys)
    {
        if (!keys.Contains(Keys.Escape)) return;
        
        IsPaused = !IsPaused;
        
        if (IsPaused) _pauseMenuController.Open();
        else _pauseMenuController.Close();
    }
}