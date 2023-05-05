#region
using System;
using System.Collections.Generic;
using System.Diagnostics;
using AstralAssault.Source.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AstralAssault;

public class SpriteRenderer
{
    private const float Pi = 3.14F;
    private readonly Dictionary<string, float> m_animationConditions = new Dictionary<string, float>();

    private readonly Dictionary<Tuple<int, int>, Transition> m_animationPaths = new Dictionary<Tuple<int, int>,
        Transition>();

    private readonly Animation[] m_animations;
    private readonly Texture2D m_spriteSheet;

    public bool Debugging = false;

    private int[] m_animationQueue;

    private int m_currentFrameIndex;
    private int m_indexInQueue;
    private bool m_isTransitioning;
    private long m_lastFrameUpdate;
    private int m_startAnimationIndex;
    private int m_targetAnimationIndex;

    public SpriteRenderer(
        Texture2D spriteSheet,
        Animation[] animations,
        Transition[] transitions,
        string[] animationConditions)
    {
        m_animations = animations;
        m_spriteSheet = spriteSheet;

        if (transitions != null)
        {
            foreach (Transition transition in transitions)
                m_animationPaths.Add(transition.FromTo, transition);
        }

        if (animationConditions != null) InitAnimationConditions(animationConditions);

        UpdateEventSource.UpdateEvent += OnUpdate;
        CurrentAnimationIndex = 0;
    }

    public SpriteRenderer(
        Texture2D spriteSheet,
        Frame frame)
    {
        Animation animation = new Animation(new[] { frame }, frame.HasRotations);

        m_animations = new[] { animation };
        m_spriteSheet = spriteSheet;
        CurrentAnimationIndex = 0;
    }

    private int CurrentAnimationIndex { get; set; }

    private Animation CurrentAnimation
    {
        get => m_animations[CurrentAnimationIndex];
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        if (Debugging) Debug.WriteLine(m_currentFrameIndex);

        int frameLength = CurrentAnimation.Frames[m_currentFrameIndex].Time;
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        Transition? transition = GetPossibleTransition();

        if (transition.HasValue)
        {
            if (Debugging) Debug.WriteLine("Transitioning");
            m_animationQueue = transition.Value.AnimationPath;
            CurrentAnimationIndex = m_animationQueue[0];
            m_indexInQueue = 0;
            m_currentFrameIndex = 0;
            m_lastFrameUpdate = timeNow;
            m_targetAnimationIndex = transition.Value.To;
            m_isTransitioning = true;

            return;
        }

        if (timeNow < (m_lastFrameUpdate + frameLength)) return;

        if (m_animationQueue == null)
        {
            if (!CurrentAnimation.IsLooping) return;

            m_currentFrameIndex = (m_currentFrameIndex + 1) % CurrentAnimation.Frames.Length;
            m_lastFrameUpdate = timeNow;

            return;
        }

        if (((m_currentFrameIndex + 1) == CurrentAnimation.Frames.Length) &&
            ((m_indexInQueue + 1) < m_animationQueue.Length))
        {
            CurrentAnimationIndex = m_animationQueue[++m_indexInQueue];
            m_currentFrameIndex = 0;
        }
        else m_currentFrameIndex = (m_currentFrameIndex + 1) % CurrentAnimation.Frames.Length;

        if ((CurrentAnimationIndex == m_targetAnimationIndex) &&
            m_isTransitioning)
        {
            m_isTransitioning = false;
            m_startAnimationIndex = m_targetAnimationIndex;
        }

        m_lastFrameUpdate = timeNow;
    }

    public void Draw(Vector2 position, float rotation, bool isCrosshair)
    {
        switch (CurrentAnimation.HasRotation)
        {
        case true:
            DrawRotatable(position, rotation, isCrosshair);

            break;
        case false:
            DrawStatic(position, isCrosshair);

            break;
        }
    }

    private void DrawStatic(Vector2 position, bool isCrosshair)
    {
        Rectangle source = CurrentAnimation.Frames[m_currentFrameIndex].Source;

        m_spriteSheet.DrawTexture2D(source, position, 0, isCrosshair ? LayerOrdering.Crosshair : LayerOrdering.SpriteSheet);
    }

    private void DrawRotatable(Vector2 position, float rotation, bool isCrosshair)
    {
        (float spriteRotation, Rectangle source) = GetRotation(rotation);

        m_spriteSheet.DrawTexture2D(source, position, spriteRotation, isCrosshair ? LayerOrdering.Crosshair : LayerOrdering.SpriteSheet);
    }

    private Tuple<float, Rectangle> GetRotation(float rotation)
    {
        int rot = (int)Math.Round(rotation / (Pi / 8));

        float spriteRotation;
        Rectangle source;

        if ((rot % 4) == 0)
        {
            source = CurrentAnimation.Frames[m_currentFrameIndex].Rotations[0];
            spriteRotation = Pi / 8 * rot;

            return new Tuple<float, Rectangle>(spriteRotation, source);
        }

        spriteRotation = rotation switch
        {
            >= 0 and < Pi / 2 => 0,
            >= Pi / 2 and < Pi => Pi / 2,
            <= 0 and > -Pi / 2 => -Pi / 2,
            <= -Pi / 2 and > -Pi => -Pi,
            var _ => 0
        };

        source = CurrentAnimation.Frames[m_currentFrameIndex].Rotations[rot.Mod(4)];

        return new Tuple<float, Rectangle>(spriteRotation, source);
    }

    private Transition? GetPossibleTransition()
    {
        foreach (KeyValuePair<string, float> condition in m_animationConditions)
        {
            foreach (Transition transition in m_animationPaths.Values)
            {
                if ((transition.From == m_startAnimationIndex) &&
                    (transition.To != m_targetAnimationIndex) &&
                    (transition.ConditionName == condition.Key) &&
                    (transition.ConditionThreshold == condition.Value)) return transition;
            }
        }

        return null;
    }

    public void SetAnimationCondition(string name, float value)
    {
        if (!m_animationConditions.ContainsKey(name))
            throw new ArgumentException($"Animation condition '{name}' does not exist");

        m_animationConditions[name] = value;
    }

    private void InitAnimationConditions(string[] name)
    {
        if (m_animationConditions.Count != 0)
            throw new InvalidOperationException("Animation conditions already initialized");

        foreach (string s in name) m_animationConditions.Add(s, 0);
    }
}