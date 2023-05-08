using System;
using System.Collections.Generic;
using System.IO;
using AstralAssault.Source.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class MainMenuState : GameState
{
    private readonly MenuController _menuController;
    private readonly Texture2D _cursorTexture;
    private Point _cursorPos;
    
    public MainMenuState(Game1 root) : base(root)
    {
        string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        string json = File.ReadAllText(path + "/Content/Menus/Main.json");

        Menu menu = Menu.Parse(Root, json);
        
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
        
    }

    public override void Exit()
    {
        
    }

    public override void Update(UpdateEventArgs e)
    {
        _cursorPos = e.MousePosition;
        _menuController.Update(e);
    }
}