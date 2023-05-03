using System;

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

    public void Invoke(Menu menu)
    {
        switch (Type)
        {
            case MenuActionType.SetVariable:
            {
                if (Parameters.Length != 2) 
                    throw new ArgumentException($"Expected 2 parameters, got {Parameters.Length}");

                menu.SetVariable((string)Parameters[0], Parameters[1]);
                break;
            }
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

        MenuActionType type = Enum.TryParse(command, out MenuActionType result)
            ? result
            : throw new FormatException("Unknown command: {command}");

        return new MenuAction(type, arguments);

    }
}