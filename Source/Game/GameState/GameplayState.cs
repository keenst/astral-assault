﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AstralAssault.Source.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AstralAssault;

public class GameplayState : GameState, IKeyboardPressedEventListener
{
    public readonly List<Entity> Entities;
    public readonly CollisionSystem CollisionSystem = new();
    public WaveController WaveController;
    
    public Player Player => (Player) Entities.Find(entity => entity is Player);
    
    private readonly MenuController _pauseMenuController;
    private bool _isPaused;

    public GameplayState(Game1 root) : base(root)
    {
        Entities = new List<Entity>();
        WaveController = new WaveController(this, Root);
        
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

        drawTasks.AddRange(WaveController.GetDrawTasks());
        drawTasks.AddRange(_pauseMenuController.GetDrawTasks());
        
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
        InputEventSource.KeyboardPressedEvent += OnKeyboardPressedEvent;
        
        Entities.Add(new Player(this, new Vector2(Game1.TargetWidth / 2F, Game1.TargetHeight / 2F)));
        Entities.Add(new Crosshair(this));
    }

    public override void Exit()
    {
        InputEventSource.KeyboardPressedEvent -= OnKeyboardPressedEvent;
        
        WaveController.StopListening();
        while (Entities.Count > 0) Entities[0].Destroy();
    }

    public void OnKeyboardPressedEvent(object sender, KeyboardEventArgs e)
    {
        if (e.Keys.All(key => key != Keys.Escape)) return;
        
        _isPaused = !_isPaused;
        
        if (_isPaused) _pauseMenuController.Open();
        else _pauseMenuController.Close();
    }
}