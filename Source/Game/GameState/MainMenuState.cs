using System.Collections.Generic;
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
        Menu menu = new();
        
        menu.AddMenuItem(new Button(8, 8, 48, 12, "Play", () =>
        {
            Root.GameStateMachine.ChangeState(new GameplayState(Root));
        }));
        
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