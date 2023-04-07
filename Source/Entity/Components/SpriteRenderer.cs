using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class SpriteRenderer
{
    private readonly LayerDepth _layerDepth;
    private readonly Animation[] _animations;
    private readonly Texture2D _spriteSheet;
    private Animation _activeAnimation;
    public int ActiveAnimationIndex => _animations.ToList().IndexOf(_activeAnimation);
    private int _activeFrame;
    private long _lastFrameUpdate;
    public readonly EffectContainer EffectContainer = new();

    private const float Pi = 3.14F;

    public SpriteRenderer(Texture2D spriteSheet, Animation[] animations, LayerDepth layerDepth)
    {
        _animations = animations;
        _spriteSheet = spriteSheet;
        _layerDepth = layerDepth;

        UpdateEventSource.UpdateEvent += OnUpdate;
        _activeAnimation = _animations[0];
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        if (_activeAnimation.Frames.Length == 1) return;

        int frameLength = _activeAnimation.Frames[_activeFrame].Time;
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (timeNow < _lastFrameUpdate + frameLength) return;

        _activeFrame = (_activeFrame + 1) % _activeAnimation.Frames.Length;
        _lastFrameUpdate = timeNow;
    }

    public void PlayAnimation(int index)
    {
        if (index >= _animations.Length || index < 0)
            throw new ArgumentOutOfRangeException();

        _activeAnimation = _animations[index];
        _activeFrame = 0;
        _lastFrameUpdate = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    public DrawTask CreateDrawTask(Vector2 position, float rotation)
    {
        return _activeAnimation.HasRotation ? DrawRotatable(position, rotation) : DrawStatic(position);
    }

    private DrawTask DrawStatic(Vector2 position)
    {
        Rectangle source = _activeAnimation.Frames[_activeFrame].Source;

        return new(_spriteSheet, source, position, 0, _layerDepth, EffectContainer.Effects);
    }

    private DrawTask DrawRotatable(Vector2 position, float rotation)
    {
        (float spriteRotation, Rectangle source) = GetRotation(rotation);

        return new(_spriteSheet, source, position, spriteRotation, _layerDepth, EffectContainer.Effects);
    }

    private Tuple<float, Rectangle> GetRotation(float rotation)
    {
        int rot = (int)Math.Round(rotation / (SpriteRenderer.Pi / 8));

        float spriteRotation;
        Rectangle source;

        if (rot % 4 == 0)
        {
            source = _activeAnimation.Frames[_activeFrame].Rotations[0];
            spriteRotation = SpriteRenderer.Pi / 8 * rot;

            return new(spriteRotation, source);
        }

        spriteRotation = rotation switch
        {
            >= 0 and < SpriteRenderer.Pi / 2 => 0,
            >= SpriteRenderer.Pi / 2 and < SpriteRenderer.Pi => SpriteRenderer.Pi / 2,
            <= 0 and > -SpriteRenderer.Pi / 2 => -SpriteRenderer.Pi / 2,
            <= -SpriteRenderer.Pi / 2 and > -SpriteRenderer.Pi => -SpriteRenderer.Pi,
            _ => 0
        };

        source = _activeAnimation.Frames[_activeFrame].Rotations[rot.Mod(4)];

        return new(spriteRotation, source);
    }
}