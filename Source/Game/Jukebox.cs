using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace AstralAssault;

public static class Jukebox
{
    private static readonly Dictionary<string, SoundEffect> SoundEffects = new();
    private static float _masterVolume = 0.5F;

    public static void Init()
    {
        SoundEffects.Add("Explosion1", AssetManager.Load<SoundEffect>("Explosion1"));
        SoundEffects.Add("Explosion2", AssetManager.Load<SoundEffect>("Explosion2"));
        SoundEffects.Add("Explosion3", AssetManager.Load<SoundEffect>("Explosion3"));
        SoundEffects.Add("GameOver", AssetManager.Load<SoundEffect>("GameOverSound"));
        SoundEffects.Add("Shoot1", AssetManager.Load<SoundEffect>("Shoot1"));
        SoundEffects.Add("Shoot2", AssetManager.Load<SoundEffect>("Shoot2"));
        SoundEffects.Add("Shoot3", AssetManager.Load<SoundEffect>("Shoot3"));
        SoundEffects.Add("QuadShoot1", AssetManager.Load<SoundEffect>("QuadShoot1"));
        SoundEffects.Add("QuadShoot2", AssetManager.Load<SoundEffect>("QuadShoot2"));
        SoundEffects.Add("QuadShoot3", AssetManager.Load<SoundEffect>("QuadShoot3"));
    }

    public static void PlaySound(string name, float volume = 1)
    {
        if (!SoundEffects.ContainsKey(name)) throw new KeyNotFoundException($"SoundEffect {name} not found");
        
        SoundEffects[name].Play(volume * _masterVolume, 0, 0);
    }
    
    public static void SetMasterVolume(float volume)
    {
        _masterVolume = volume;
    }
}