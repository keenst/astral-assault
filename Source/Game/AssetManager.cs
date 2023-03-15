using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public static class AssetManager
{
    private static readonly Dictionary<string, Texture2D> Textures = new();
    private static Game1 _root;
    
    public static void Init(Game1 root)
    {
        _root = root;
    }
    
    public static Texture2D LoadTexture(string path)
    {
        if (Textures.ContainsKey(path))
            return Textures[path];
        
        Texture2D texture = _root.Content.Load<Texture2D>($"assets/{path}");
        Textures.Add(path, texture);
        return texture;
    }
}