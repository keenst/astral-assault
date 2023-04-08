using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class SpriteRenderer
{
    private readonly LayerDepth m_layerDepth;
    private readonly Animation[] m_animations;
    private readonly Texture2D m_spriteSheet;
    private Animation m_activeAnimation;

    public int ActiveAnimationIndex
    {
        get => m_animations.ToList().IndexOf(m_activeAnimation);
    }

    private int m_activeFrame;
    private long m_lastFrameUpdate;
    public readonly EffectContainer EffectContainer = new EffectContainer();

    public const float Pi = 3.14F;

    public SpriteRenderer(Texture2D spriteSheet, Animation[] animations, LayerDepth layerDepth)
    {
        m_animations = animations;
        m_spriteSheet = spriteSheet;
        m_layerDepth = layerDepth;

        UpdateEventSource.UpdateEvent += OnUpdate;
        m_activeAnimation = m_animations[0];
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        if (m_activeAnimation.Frames.Length == 1) return;

        int frameLength = m_activeAnimation.Frames[m_activeFrame].Time;
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (timeNow < (m_lastFrameUpdate + frameLength)) return;

        m_activeFrame = (m_activeFrame + 1) % m_activeAnimation.Frames.Length;
        m_lastFrameUpdate = timeNow;
    }

    public void PlayAnimation(int index)
    {
        if ((index >= m_animations.Length) || (index < 0))
            throw new ArgumentOutOfRangeException();

        m_activeAnimation = m_animations[index];
        m_activeFrame = 0;
        m_lastFrameUpdate = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    public DrawTask CreateDrawTask(Vector2 position, float rotation) =>
        m_activeAnimation.HasRotation ? DrawRotatable(position, rotation) : DrawStatic(position);

    private DrawTask DrawStatic(Vector2 position)
    {
        Rectangle source = m_activeAnimation.Frames[m_activeFrame].Source;

        return new DrawTask(m_spriteSheet, source, position, 0, m_layerDepth, EffectContainer.Effects);
    }

    private DrawTask DrawRotatable(Vector2 position, float rotation)
    {
        (float spriteRotation, Rectangle source) = GetRotation(rotation);

        return new DrawTask(m_spriteSheet, source, position, spriteRotation, m_layerDepth, EffectContainer.Effects);
    }

    private Tuple<float, Rectangle> GetRotation(float rotation)
    {
        int rot = (int)Math.Round(rotation / (Pi / 8));

        float spriteRotation;
        Rectangle source;

        if ((rot % 4) == 0)
        {
            source = m_activeAnimation.Frames[m_activeFrame].Rotations[0];
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

        source = m_activeAnimation.Frames[m_activeFrame].Rotations[rot.Mod(4)];

        return new Tuple<float, Rectangle>(spriteRotation, source);
    }
}