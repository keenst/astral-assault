using System.Collections.Generic;
using System.Diagnostics;
using AstralAssault.Source.Menu;

namespace AstralAssault;

public class MainMenuState : GameState
{
    private readonly MenuController _menuController;

    public MainMenuState(Game1 root) : base(root)
    {
        Menu menu = new();
        
        menu.AddMenuItem(new Button(0, 0, 32, 32, "Test", () =>
        {
            Debug.WriteLine("Clicked!");
        }));
        
        _menuController = new MenuController(menu, this);
    }

    public override List<DrawTask> GetDrawTasks()
    {
        return _menuController.GetDrawTasks();
    }

    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }
}