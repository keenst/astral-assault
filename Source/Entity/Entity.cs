using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector4 = Microsoft.Xna.Framework.Vector4;

namespace AstralAssault;

public class Entity
{
    public Vector2 Position;
    public Vector2 Velocity;
    protected float Rotation;
    protected Collider Collider;
    protected SpriteRenderer SpriteRenderer;
    protected readonly GameplayState GameState;
    protected OutOfBounds OutOfBoundsBehavior = OutOfBounds.Wrap;
    protected bool IsActor = false;
    protected float MaxHP;
    protected float HP;
    protected float ContactDamage;

    private bool m_isHighlighted;
    private long m_timeStartedHighlightingMS;
    private float m_highlightAlpha;

    public bool IsFriendly;

    private readonly long m_timeSpawned;

    public long TimeSinceSpawned
    {
        get => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - m_timeSpawned;
    }

    private Texture2D m_healthBarTexture;

    protected enum OutOfBounds { DoNothing, Wrap, Destroy }

    protected Entity(GameplayState gameState, Vector2 position)
    {
        GameState = gameState;
        Position = position;
        m_timeSpawned = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        CreateHealthBarTexture();
    }

    public virtual void OnUpdate(object sender, UpdateEventArgs e)
    {
        SpriteRenderer.OnUpdate(sender, e);

        if (IsActor && (HP <= 0))
        {
            OnDeath();

            return;
        }

        Position += Velocity * e.DeltaTime;
        Collider?.SetPosition(Position.ToPoint());

        switch (OutOfBoundsBehavior)
        {
        case OutOfBounds.DoNothing:
        {
            break;
        }

        case OutOfBounds.Destroy:
        {
            if (Position.X is < 0 or > Game1.TargetWidth ||
                Position.Y is < 0 or > Game1.TargetHeight) Destroy();

            break;
        }

        case OutOfBounds.Wrap:
        {
            Position.X = Position.X switch
            {
                < 0 => Game1.TargetWidth,
                > Game1.TargetWidth => 0,
                var _ => Position.X
            };

            Position.Y = Position.Y switch
            {
                < 0 => Game1.TargetHeight,
                > Game1.TargetHeight => 0,
                var _ => Position.Y
            };

            break;
        }

        default:
        {
            throw new ArgumentOutOfRangeException();
        }
        }
    }

    public virtual void OnCollision(Collider other)
    {
        if (!IsActor || (other.Parent.IsFriendly == IsFriendly)) return;

        HP = Math.Max(0, HP - other.Parent.ContactDamage);

        m_isHighlighted = true;
        m_timeStartedHighlightingMS = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        m_highlightAlpha = 0.7F;

        SpriteRenderer.EffectContainer.SetEffect<HighlightEffect, float>(m_highlightAlpha);
    }

    public virtual List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new List<DrawTask>();

        if (m_isHighlighted)
        {
            const float decayRate = 0.005F;

            long timeNowMS = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            float timeSinceStartedS = (timeNowMS - m_timeStartedHighlightingMS) / 1000F;

            m_highlightAlpha = 0.7F * MathF.Pow(decayRate, timeSinceStartedS);

            if (m_highlightAlpha <= 0.01)
            {
                m_isHighlighted = false;
                m_highlightAlpha = 0;
                SpriteRenderer.EffectContainer.RemoveEffect<HighlightEffect>();
            }
            else SpriteRenderer.EffectContainer.SetEffect<HighlightEffect, float>(m_highlightAlpha);
        }

        if (IsActor) drawTasks.AddRange(CreateHealthBarDrawTasks());
        drawTasks.Add(SpriteRenderer.CreateDrawTask(Position, Rotation));

        return drawTasks;
    }

    public virtual void Destroy()
    {
        GameState.Entities.Remove(this);
        GameState.CollisionSystem.RemoveCollider(Collider);
    }

    protected virtual void OnDeath()
    {
        Destroy();
    }

    private void CreateHealthBarTexture()
    {
        m_healthBarTexture = new Texture2D(GameState.Root.GraphicsDevice, 1, 1);
        Color[] data = { Palette.GetColor(Palette.Colors.Grey9) };
        m_healthBarTexture.SetData(data);
    }

    private List<DrawTask> CreateHealthBarDrawTasks()
    {
        const int width = 20;
        const int height = 3;

        int filled = (int)Math.Ceiling(HP / MaxHP * width);

        int x = (int)Position.X - width / 2;
        int y = (int)Position.Y - 20;

        Rectangle outline = new Rectangle(x - 1, y - 1, width + 2, height + 2);
        Rectangle emptyHealthBar = new Rectangle(x, y, width, height);
        Rectangle fullHealthBar = new Rectangle(x, y, filled, height);

        Vector4 outlineColor = Palette.GetColorVector(Palette.Colors.Black);
        Vector4 emptyColor = Palette.GetColorVector(Palette.Colors.Red6);
        Vector4 fullColor = Palette.GetColorVector(Palette.Colors.Green7);

        Rectangle source = new Rectangle(0, 0, 1, 1);

        DrawTask background = new DrawTask
        (
            m_healthBarTexture, source, outline, 0, LayerDepth.HUD,
            new List<IDrawTaskEffect> { new ColorEffect(outlineColor) }, Palette.GetColor(Palette.Colors.Black)
        );

        DrawTask empty = new DrawTask
        (
            m_healthBarTexture, source, emptyHealthBar, 0, LayerDepth.HUD,
            new List<IDrawTaskEffect> { new ColorEffect(emptyColor) }, Palette.GetColor(Palette.Colors.Red9)
        );

        DrawTask full = new DrawTask
        (
            m_healthBarTexture, source, fullHealthBar, 0, LayerDepth.HUD,
            new List<IDrawTaskEffect> { new ColorEffect(fullColor) }, Palette.GetColor(Palette.Colors.Green9)
        );

        return new List<DrawTask> { background, empty, full };
    }
}