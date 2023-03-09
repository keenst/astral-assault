using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class SpriteRenderer : IUpdateEventListener
{
    public Animation[] Animations { get; }
    private readonly Texture2D _spriteSheet;

    public Animation ActiveAnimation { get; private set; }
    public int ActiveAnimationIndex => Animations.ToList().IndexOf(ActiveAnimation);
    private int _activeFrame;

    private long _lastFrameUpdate;

    private const float Pi = 3.14F;

    public SpriteRenderer(Texture2D spriteSheet, Animation[] frames)
    {
        Animations = frames;
        _spriteSheet = spriteSheet;
        
        UpdateEventSource.UpdateEvent += OnUpdate;
        ActiveAnimation = Animations[0];
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        if (ActiveAnimation.Frames.Length == 1) return;
        
        int frameLength = ActiveAnimation.Frames[_activeFrame].Time;
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (timeNow < _lastFrameUpdate + frameLength) return;

        _activeFrame = (_activeFrame + 1) % ActiveAnimation.Frames.Length;
        _lastFrameUpdate = timeNow;
    }

    public void PlayAnimation(int index)
    {
        if (index >= Animations.Length || index < 0) 
            throw new ArgumentOutOfRangeException();

        ActiveAnimation = Animations[index];
        _activeFrame = 0;
        _lastFrameUpdate = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation)
    {
        if (ActiveAnimation.HasRotation)
            DrawRotatable(spriteBatch, position, rotation);
        else
            DrawStatic(spriteBatch, position);
    }

    private void DrawStatic(SpriteBatch spriteBatch, Vector2 position)
    {
        Rectangle source = ActiveAnimation.Frames[_activeFrame].Source;
        
        spriteBatch.Draw(
            _spriteSheet, 
            position, 
            source, 
            Color.White, 
            0, 
            new Vector2(source.Height / 2F, source.Width / 2F),
            new Vector2(1, 1),
            SpriteEffects.None,
            0);
    }

    private void DrawRotatable(SpriteBatch spriteBatch, Vector2 position, float rotation)
    {
        (float spriteRotation, Rectangle source) = GetRotation(rotation);

        spriteBatch.Draw(
            _spriteSheet, 
            position, 
            source, 
            Color.White, 
            spriteRotation, 
            new Vector2(source.Height / 2F, source.Width / 2F),
            new Vector2(1, 1),
            SpriteEffects.None,
            0);
    }

    private Tuple<float, Rectangle> GetRotation(float rotation)
    {
        int rot = (int)Math.Round(rotation / (Pi / 8));

        float spriteRotation;
        Rectangle source;
        
        if (rot % 4 == 0)
        {
            source = ActiveAnimation.Frames[_activeFrame].Rotations[0];
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

        source = ActiveAnimation.Frames[_activeFrame].Rotations[rot.Mod(4)];

        return new Tuple<float, Rectangle>(spriteRotation, source);
    }
}