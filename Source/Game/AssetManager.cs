#region
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AstralAssault;

public static class AssetManager
{
    private static readonly Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
    private static readonly Dictionary<string, Effect> Effects = new Dictionary<string, Effect>();
    private static readonly Dictionary<string, SoundEffect> SoundEffects = new();
    private static Game1 m_root;
    
    public static void Init(Game1 root)
    {
        m_root = root;
    }

    public static T Load<T>(string path)
    {
        Dictionary<string, T> activeDictionary;
        string activeDirectory;

        if (typeof(T) == typeof(Texture2D))
        {
            activeDictionary = Textures as Dictionary<string, T>;
            activeDirectory = "Assets";
        }
        else if (typeof(T) == typeof(SoundEffect))
        {
            activeDictionary = SoundEffects as Dictionary<string, T>;
            activeDirectory = "Assets";
        }
        else if (typeof(T) == typeof(Effect))
        {
            activeDictionary = Effects as Dictionary<string, T>;
            activeDirectory = "Shaders";
        }
        else
        {
            throw new ArgumentException("T must be Texture2D, SoundEffect or Effect");
        }

        if (activeDictionary.ContainsKey(path))
            return activeDictionary[path];

        T asset = m_root.Content.Load<T>($"{activeDirectory}/{path}");

        return asset;
    }
}