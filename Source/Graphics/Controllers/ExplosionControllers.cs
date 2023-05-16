using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class ExplosionController
{
    private readonly Texture2D _explosionTexture;
    
    private readonly List<Explosion> _explosions = new();
    
    private const int FrameDuration = 100;
    private const int NumFrames = 8;
    private const int ExplosionDuration = FrameDuration * NumFrames;
    private const int FrameWidth = 32;
    private const int FrameHeight = 32;

    public ExplosionController()
    {
        _explosionTexture = AssetManager.Load<Texture2D>("Explosion");
    }
    
    public void SpawnExplosion(Vector2 position)
    {
        _explosions.Add(new Explosion(position.ToPoint()));
    }

    public List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new();

        List<Explosion> toRemove = new();
        
        foreach (Explosion explosion in _explosions)
        {
            if (explosion.TimeSinceSpawned >= ExplosionDuration)
            {
                toRemove.Add(explosion);
                continue;   
            }
            drawTasks.Add(GetExplosionDrawTask(explosion));
        }

        foreach (Explosion explosion in toRemove)
        {
            _explosions.Remove(explosion);
        }
        
        return drawTasks;
    }

    private DrawTask GetExplosionDrawTask(Explosion explosion)
    {
        int spriteIndex = (int)(explosion.TimeSinceSpawned / FrameDuration) % NumFrames;
        Rectangle source = new(spriteIndex * FrameWidth, 0, FrameWidth, FrameHeight);
        
        return new DrawTask(
            _explosionTexture,
            source,
            explosion.Position.ToVector2(),
            0,
            LayerDepth.Explosions,
            new List<IDrawTaskEffect>());
    }
}