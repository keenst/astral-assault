using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault.Source.Menu;

public class Menu
{
    public List<IMenuItem> MenuItems { get; }
    
    public Menu()
    {
        MenuItems = new List<IMenuItem>();
    }

    private Menu(List<IMenuItem> menuItems)
    {
        MenuItems = menuItems;
    }
    
    public void AddMenuItem(IMenuItem menuItem)
    {
        MenuItems.Add(menuItem);
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

    public static Menu Parse(string json)
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
                        Debug.WriteLine($"Unknown menu item type: {type.GetString()}");
                        break;
                }
            }
        }
        
        return new Menu(menuItems);
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
        
        string action = element.TryGetProperty("click_action", out JsonElement actionElement)
            ? actionElement.GetString()
            : throw new FormatException("Could not parse button: missing click_action property");

        return new Button(x, y, width, height, text, ParseAction(action));
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
    
    private static Action ParseAction(string lambda)
    {
        LambdaExpression yepyep = DynamicExpressionParser.ParseLambda(
            false, 
            new ParameterExpression[] {},
            typeof (void),
            lambda);
        return (Action) yepyep.Compile();
    }
}