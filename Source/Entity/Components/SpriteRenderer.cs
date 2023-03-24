using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class SpriteRenderer : IUpdateEventListener
{
    public int ActiveAnimation { get; private set; }

    private readonly LayerDepth _layerDepth;
    private readonly Animation[] _animations;
    private readonly Texture2D _spriteSheet;
    private Animation CurrentAnimation => _animations[ActiveAnimation];
    private int _activeFrame;
    private long _lastFrameUpdate;
    private readonly List<IDrawTaskEffect> _drawTaskEffects = new();
    private readonly Dictionary<Tuple<int, int>, Transition> _animationPaths = new();
    private int[] _animationQueue;
    private int _indexInQueue;
    
    private const float Pi = 3.14F;

    public SpriteRenderer(Texture2D spriteSheet, Animation[] animations, LayerDepth layerDepth, 
        Transition[] transitions = null)
    {
        _animations = animations;
        _spriteSheet = spriteSheet;
        _layerDepth = layerDepth;

        if (transitions != null)
        {
            foreach (Transition transition in transitions)
            {
                _animationPaths.Add(transition.FromTo, transition);
            }
        }
        
        UpdateEventSource.UpdateEvent += OnUpdate;
        ActiveAnimation = 0;
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        if (_animationQueue == null) return;
        
        int frameLength = CurrentAnimation.Frames[_activeFrame].Time;
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (timeNow < _lastFrameUpdate + frameLength) return;

        if (_activeFrame + 1 == CurrentAnimation.Frames.Length &&
            _indexInQueue + 1 == _animationQueue.Length - 1)
        {
            ActiveAnimation = _animationQueue[++_indexInQueue];
            _activeFrame = 0;
            Debug.WriteLine("end of animation");
        }
        else
        {
            _activeFrame = (_activeFrame + 1) % CurrentAnimation.Frames.Length;
            //Debug.WriteLine($"next frame: {_activeFrame}");
        }
        
        _lastFrameUpdate = timeNow;
    }

    public void PlayAnimation(int index)
    {
        if (index >= _animations.Length || index < 0) 
            throw new ArgumentOutOfRangeException();

        if (index == ActiveAnimation) return;
        
        Debug.WriteLine($"playing animation: {index}, from animation: {ActiveAnimation}");

        _animationQueue = GetTransition(ActiveAnimation, index).AnimationPath;
    }
    
    public DrawTask CreateDrawTask(Vector2 position, float rotation)
    {
        return CurrentAnimation.HasRotation ? DrawRotatable(position, rotation) : DrawStatic(position);
    }

    public void SetEffect<TEffect, TParameter>(TParameter parameter)
    {
        if (!_drawTaskEffects.OfType<TEffect>().Any())
        {
            _drawTaskEffects.Add((IDrawTaskEffect)Activator.CreateInstance(typeof(TEffect), parameter));
        }
        else
        {
            int index = _drawTaskEffects.IndexOf((IDrawTaskEffect)_drawTaskEffects.OfType<TEffect>().First());
            _drawTaskEffects[index] = (IDrawTaskEffect)Activator.CreateInstance(typeof(TEffect), parameter);
        }
    }
    
    public void RemoveEffect<TEffect>()
    {
        if (!_drawTaskEffects.OfType<TEffect>().Any()) return;
        
        int index = _drawTaskEffects.IndexOf((IDrawTaskEffect)_drawTaskEffects.OfType<TEffect>().First());
        _drawTaskEffects.RemoveAt(index);
    }

    private DrawTask DrawStatic(Vector2 position)
    {
        Rectangle source = CurrentAnimation.Frames[_activeFrame].Source;
        return new DrawTask(_spriteSheet, source, position, 0, _layerDepth, _drawTaskEffects);
    }

    private DrawTask DrawRotatable(Vector2 position, float rotation)
    {
        (float spriteRotation, Rectangle source) = GetRotation(rotation);
        return new DrawTask(_spriteSheet, source, position, spriteRotation, _layerDepth, _drawTaskEffects);
    }

    private Tuple<float, Rectangle> GetRotation(float rotation)
    {
        int rot = (int)Math.Round(rotation / (Pi / 8));

        float spriteRotation;
        Rectangle source;
        
        if (rot % 4 == 0)
        {
            source = CurrentAnimation.Frames[_activeFrame].Rotations[0];
            spriteRotation = Pi / 8 * rot;
            return new Tuple<float, Rectangle>(spriteRotation, source);
        }

        spriteRotation = rotation switch
        {
            >= 0         and < Pi / 2    => 0,
            >= Pi / 2    and < Pi        => Pi / 2,
            <= 0         and > -Pi / 2   => -Pi / 2,
            <= -Pi / 2   and > -Pi       => -Pi,
            _ => 0
        };

        source = CurrentAnimation.Frames[_activeFrame].Rotations[rot.Mod(4)];

        return new Tuple<float, Rectangle>(spriteRotation, source);
    }

    private Transition GetTransition(int from, int to)
    {
        return _animationPaths[new Tuple<int, int>(from, to)];
    }
}