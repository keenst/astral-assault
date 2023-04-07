using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public static class AssetManager
{
    private static readonly Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
    private static readonly Dictionary<string, Effect> Effects = new Dictionary<string, Effect>();
    private static Game1 _root;

    public static void Init(Game1 root)
    {
        AssetManager._root = root;
    }

    public static T Load<T>(string path)
    {
        Dictionary<string, T> activeDictionary;
        string activeDirectory;

        if (typeof(T) == typeof(Texture2D))
        {
            activeDictionary = AssetManager.Textures as Dictionary<string, T>;
            activeDirectory = "Assets";
        }
        else if (typeof(T) == typeof(Effect))
        {
            activeDictionary = AssetManager.Effects as Dictionary<string, T>;
            activeDirectory = "Shaders";
        }
        else
        {
            throw new ArgumentException("T must be either Texture2D or Effect");
        }

        if (activeDictionary.ContainsKey(path))
            return activeDictionary[path];

        T asset = AssetManager._root.Content.Load<T>($"{activeDirectory}/{path}");

        return asset;
    }
}