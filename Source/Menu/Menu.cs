using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault.Source.Menu;

public class Menu
{
    public List<IMenuItem> MenuItems { get; }
    public Game1 Root { get; }
    
    private readonly Dictionary<string, object> _variables = new();
    
    private Menu(Game1 root, List<IMenuItem> menuItems)
    {
        Root = root;
        MenuItems = menuItems;
    }

    public List<DrawTask> GetDrawTasks(Texture2D texture)
    {
        List<DrawTask> drawTasks = new();
        
        foreach (IMenuItem menuItem in MenuItems)
        {
            drawTasks.AddRange(menuItem.GetDrawTasks());
        }
        
        return drawTasks;
    }
    
    public void SetVariable(string name, object value)
    {
        _variables[name] = value;
        using Dictionary<string, object>.Enumerator enumerator = _variables.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Debug.WriteLine($"{enumerator.Current.Key} = {enumerator.Current.Value}");
        }
    }

    public static Menu Parse(Game1 root, string json)
    {
        JsonDocument doc = JsonDocument.Parse(json);
        
        List<IMenuItem> menuItems = new();
        
        foreach (JsonElement element in doc.RootElement.EnumerateArray())
        {
            if (element.TryGetProperty("type", out JsonElement type))
            {
                switch (type.GetString())
                {
                    case "button":
                        menuItems.Add(ParseButton(element));
                        break;
                    case "label":
                        menuItems.Add(ParseLabel(element));
                        break;
                    default:
                        throw new FormatException($"Unknown menu item type: {type.GetString()}");
                }
            }
        }
        
        return new Menu(root, menuItems);
    }

    private static Button ParseButton(JsonElement element)
    {
        int x = element.TryGetProperty("x", out JsonElement xElement)
            ? xElement.GetInt32()
            : throw new FormatException("Could not parse button: missing x property");

        int y = element.TryGetProperty("y", out JsonElement yElement)
            ? yElement.GetInt32()
            : throw new FormatException("Could not parse button: missing y property");
        
        int width = element.TryGetProperty("width", out JsonElement widthElement)
            ? widthElement.GetInt32()
            : throw new FormatException("Could not parse button: missing width property");
        
        int height = element.TryGetProperty("height", out JsonElement heightElement)
            ? heightElement.GetInt32()
            : throw new FormatException("Could not parse button: missing height property");
        
        string text = element.TryGetProperty("text", out JsonElement textElement)
            ? textElement.GetString()
            : throw new FormatException("Could not parse button: missing text property");
        
        string action = element.TryGetProperty("clickAction", out JsonElement actionElement)
            ? actionElement.GetString()
            : throw new FormatException("Could not parse button: missing clickAction property");
        
        MenuAction menuAction = MenuAction.Parse(action);
        
        return new Button(x, y, width, height, text, menuAction);
    }

    private static Label ParseLabel(JsonElement element)
    {
        int x = element.TryGetProperty("x", out JsonElement xElement)
            ? xElement.GetInt32()
            : throw new FormatException("Could not parse label: missing x property");
        
        int y = element.TryGetProperty("y", out JsonElement yElement)
            ? yElement.GetInt32()
            : throw new FormatException("Could not parse label: missing y property");
        
        string text = element.TryGetProperty("text", out JsonElement textElement)
            ? textElement.GetString()
            : throw new FormatException("Could not parse label: missing text property");
        
        return new Label(x, y, text);
    }
}