using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public static class AssetManager
{
    private static readonly Dictionary<string, Texture2D> Textures = new();
    private static readonly Dictionary<string, Effect> Effects = new();
    private static Game1 _root;
    
    public static void Init(Game1 root)
    {
        _root = root;
    }

    public static T Load<T>(string path)
    {
        Dictionary<string, T> activeDictionary;
        string activeDirectory;
        
        if (typeof(T) == typeof(Texture2D))
        {
            activeDictionary = Textures as Dictionary<string, T>;
            activeDirectory = "assets";
        }
        else if (typeof(T) == typeof(Effect))
        {
            activeDictionary = Effects as Dictionary<string, T>;
            activeDirectory = "shaders";
        }
        else
        {
            throw new ArgumentException("T must be either Texture2D or Effect");
        }

        if (activeDictionary.ContainsKey(path))
            return activeDictionary[path];
        
        T asset = _root.Content.Load<T>($"{activeDirectory}/{path}");
        return asset;
    }
}