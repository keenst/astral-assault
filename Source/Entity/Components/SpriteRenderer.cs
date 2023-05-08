using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class SpriteRenderer
{
    public readonly EffectContainer EffectContainer = new();

    private int CurrentAnimationIndex { get; set; }
    private readonly LayerDepth _layerDepth;
    private readonly Animation[] _animations;
    private readonly Texture2D _spriteSheet;
    private Animation CurrentAnimation => _animations[CurrentAnimationIndex];
    private int _currentFrameIndex;
    private int _targetAnimationIndex;
    private int _startAnimationIndex;
    private bool _isTransitioning;
    private long _lastFrameUpdate;
    private readonly Dictionary<Tuple<int, int>, Transition> _animationPaths = new();
    private int[] _animationQueue;
    private int _indexInQueue;
    private readonly Dictionary<string, float> _animationConditions = new();

    private const float Pi = 3.14F;
    
    public SpriteRenderer(
        Texture2D spriteSheet, 
        Animation[] animations, 
        LayerDepth layerDepth,
        Transition[] transitions, 
        string[] animationConditions)
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
        
        if (animationConditions != null) InitAnimationConditions(animationConditions);
        
        CurrentAnimationIndex = 0;
    }

    public SpriteRenderer(
        Texture2D spriteSheet,
        Frame frame,
        LayerDepth layerDepth)
    {
        Animation animation = new(
            new Frame[] { frame }, 
            frame.HasRotations);
        
        _animations = new[] { animation };
        _spriteSheet = spriteSheet;
        _layerDepth = layerDepth;
        
        CurrentAnimationIndex = 0;
    }

    public void Update(UpdateEventArgs e)
    {
        int frameLength = CurrentAnimation.Frames[_currentFrameIndex].Time;
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        Transition? transition = GetPossibleTransition();

        if (transition.HasValue)
        {
            _animationQueue = transition.Value.AnimationPath;
            CurrentAnimationIndex = _animationQueue[0];
            _indexInQueue = 0;
            _currentFrameIndex = 0;
            _lastFrameUpdate = timeNow;
            _targetAnimationIndex = transition.Value.To;
            _isTransitioning = true;
            return;
        }

        if (_animationQueue == null) return;
        
        if (timeNow < _lastFrameUpdate + frameLength) return;

        if (_currentFrameIndex + 1 == CurrentAnimation.Frames.Length &&
            _indexInQueue + 1 < _animationQueue.Length)
        {
            CurrentAnimationIndex = _animationQueue[++_indexInQueue];
            _currentFrameIndex = 0;
        }
        else
        {
            _currentFrameIndex = (_currentFrameIndex + 1) % CurrentAnimation.Frames.Length;
        }
        
        if (CurrentAnimationIndex == _targetAnimationIndex &&
            _isTransitioning)
        {
            _isTransitioning = false;
            _startAnimationIndex = _targetAnimationIndex;
        }

        _lastFrameUpdate = timeNow;
    }

    public DrawTask CreateDrawTask(Vector2 position, float rotation)
    {
        return CurrentAnimation.HasRotation ? DrawRotatable(position, rotation) : DrawStatic(position);
    }
    
    private DrawTask DrawStatic(Vector2 position)
    {
        Rectangle source = CurrentAnimation.Frames[_currentFrameIndex].Source;
        return new DrawTask(_spriteSheet, source, position, 0, _layerDepth, EffectContainer.Effects);
    }

    private DrawTask DrawRotatable(Vector2 position, float rotation)
    {
        (float spriteRotation, Rectangle source) = GetRotation(rotation);
        return new DrawTask(_spriteSheet, source, position, spriteRotation, _layerDepth, EffectContainer.Effects);
    }

    private Tuple<float, Rectangle> GetRotation(float rotation)
    {
        int rot = (int)Math.Round(rotation / (Pi / 8));

        float spriteRotation;
        Rectangle source;

        if (rot % 4 == 0)
        {
            source = CurrentAnimation.Frames[_currentFrameIndex].Rotations[0];
            spriteRotation = Pi / 8 * rot;
            return new Tuple<float, Rectangle>(spriteRotation, source);
        }

        spriteRotation = rotation switch
        {
            >= 0 and < Pi / 2 => 0,
            >= Pi / 2 and < Pi => Pi / 2,
            <= 0 and > -Pi / 2 => -Pi / 2,
            <= -Pi / 2 and > -Pi => -Pi,
            _ => 0
        };

        source = CurrentAnimation.Frames[_currentFrameIndex].Rotations[rot.Mod(4)];

        return new Tuple<float, Rectangle>(spriteRotation, source);
    }

    private Transition? GetPossibleTransition()
    {
        foreach (KeyValuePair<string, float> condition in _animationConditions)
        {
            foreach (Transition transition in _animationPaths.Values)
            {
                if (transition.From == _startAnimationIndex &&
                    transition.To != _targetAnimationIndex &&
                    transition.ConditionName == condition.Key &&
                    transition.ConditionThreshold == condition.Value)
                {
                    return transition;
                }
            }
        }

        return null;
    }

    public void SetAnimationCondition(string name, float value)
    {
        if (!_animationConditions.ContainsKey(name))
        {
            throw new ArgumentException($"Animation condition '{name}' does not exist");
        }
        
        _animationConditions[name] = value;
    }

    private void InitAnimationConditions(string[] name)
    {
        if (_animationConditions.Count != 0)
        {
            throw new InvalidOperationException("Animation conditions already initialized");
        }

        foreach (string s in name)
        {
            _animationConditions.Add(s, 0);
        }
    } 
}