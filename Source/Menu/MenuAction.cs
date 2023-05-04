using System;
using System.Collections.Generic;

namespace AstralAssault.Source.Menu;

public struct MenuAction
{
    public MenuActionType Type { get; }
    public object[] Parameters { get; }
    
    public MenuAction(MenuActionType type, object[] parameters)
    {
        Type = type;
        Parameters = parameters;
    }
    
    private static readonly Dictionary<MenuActionType, int> ParameterCounts = new()
    {
        { MenuActionType.SetVariable, 2 },
        { MenuActionType.Exit, 0 },
        { MenuActionType.ChangeGameState, 1 }
    };

    public void Invoke(Menu menu)
    {
        if (Parameters.Length != ParameterCounts[Type])
            throw new ArgumentException($"Expected {ParameterCounts[Type]} parameters, got {Parameters.Length}");
        
        switch (Type)
        {
            case MenuActionType.SetVariable:
                menu.SetVariable((string)Parameters[0], Parameters[1]);
                break;
            case MenuActionType.Exit:
                menu.Root.Exit();
                break;
            case MenuActionType.ChangeGameState:
                
                GameState gameState = (string)Parameters[0] switch
                {
                    "Gameplay" => new GameplayState(menu.Root),
                    _ => throw new ArgumentException($"Invalid game state: {Parameters[0]}")
                };
                
                menu.Root.GameStateMachine.ChangeState(gameState);
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static MenuAction Parse(string json)
    {
        int startIndex = json.IndexOf('(');
        int endIndex = json.IndexOf(')');
        
        if (startIndex == -1 || endIndex == -1) throw new FormatException("Invalid menu action format");

        string command = json[..startIndex];

        MenuActionType type = Enum.TryParse(command, out MenuActionType result)
            ? result
            : throw new FormatException($"Unknown command: {command}");
        
        if (startIndex == endIndex - 1)
        {
            return new MenuAction(type, Array.Empty<object>());
        }
        
        string[] argumentStrings = json[(startIndex + 1)..endIndex].Split(',');
        object[] arguments = new object[argumentStrings.Length];

        for (int i = 0; i < argumentStrings.Length; i++)
        {
            string argument = argumentStrings[i].Trim();
            
            if (int.TryParse(argument, out int intResult))
            {
                arguments[i] = intResult;
                continue;
            }

            if (float.TryParse(argument, out float floatResult))
            {
                arguments[i] = floatResult;
                continue;
            }

            arguments[i] = argument;
        }

        return new MenuAction(type, arguments);
    }
}