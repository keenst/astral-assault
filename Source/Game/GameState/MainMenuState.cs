using System;
using System.Collections.Generic;
using System.IO;
using AstralAssault.Source.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class MainMenuState : GameState, IMouseMoveEventListener
{
    private readonly MenuController _menuController;
    private Texture2D _cursorTexture;
    private Point _cursorPos;
    
    public MainMenuState(Game1 root) : base(root)
    {
        string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        string json = File.ReadAllText(path + "/Content/Menus/Main.json");

        Menu menu = Menu.Parse(json);
        
        // menu.AddMenuItem(new Label(8, 8, "Astral Assault"));
        //
        // menu.AddMenuItem(new Button(8, 24, 48, 12, "Play", () =>
        // {
        //     Root.GameStateMachine.ChangeState(new GameplayState(Root));
        // }));
        //
        // menu.AddMenuItem(new Button(8, 40, 48, 12, "Exit", () =>
        // {
        //     Root.Exit();
        // }));
        
        _menuController = new MenuController(menu, this);
        
        Color[] data = { Color.White };
        _cursorTexture = new Texture2D(Root.GraphicsDevice, 1, 1);
        _cursorTexture.SetData(data);
    }

    public override List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new();
        
        drawTasks.AddRange(_menuController.GetDrawTasks());
        
        DrawTask cursor = new(
            _cursorTexture, 
            _cursorPos.ToVector2(), 
            0, 
            LayerDepth.Crosshair, 
            new List<IDrawTaskEffect> { new ColorEffect(new Vector4(0, 0, 0, 1)) });
        
        drawTasks.Add(cursor);
        
        return drawTasks;
    }

    public override void Enter()
    {
        InputEventSource.MouseMoveEvent += OnMouseMoveEvent;
    }

    public override void Exit()
    {
        InputEventSource.MouseMoveEvent -= OnMouseMoveEvent;
        _menuController.Destroy();
    }

    public void OnMouseMoveEvent(object sender, MouseMoveEventArgs e)
    {
        _cursorPos = e.Position;
    }
}