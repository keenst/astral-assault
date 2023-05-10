#region
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGameOfDoomHmmm.Source.Entity.Components;
using TheGameOfDoomHmmm.Source.Entity.Entities.Items;
using TheGameOfDoomHmmm.Source.Game;
using TheGameOfDoomHmmm.Source.Game.GameState;
using TheGameOfDoomHmmm.Source.Graphics;
#endregion

namespace TheGameOfDoomHmmm.Source.Entity.Entities;

public class Entity
{
    protected readonly GameplayState GameState;

    private readonly long m_timeSpawned;
    protected Collider Collider;
    protected float ContactDamage;
    protected float HP;
    protected bool IsActor = false;

    private Texture2D m_healthBarTexture;
    public float m_highlightAlpha;

    private bool m_isHighlighted;
    private long m_timeStartedHighlightingMS;
    protected float MaxHP;
    protected OutOfBounds OutOfBoundsBehavior = OutOfBounds.Wrap;
    public Vector2 Position;
    protected float Rotation;
    protected SpriteRenderer SpriteRenderer;
    public Vector2 Velocity;

    protected Entity(GameplayState gameState, Vector2 position)
    {
        GameState = gameState;
        Position = position;
        m_timeSpawned = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (this is not Bullet) CreateHealthBarTexture();
    }

    public long TimeSinceSpawned
    {
        get => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - m_timeSpawned;
    }

    public virtual void OnUpdate(object sender, UpdateEventArgs e)
    {
        if (IsActor && (HP <= 0))
        {
            OnDeath();

            return;
        }

        Position += Velocity * e.DeltaTime;

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
        if (Game1.PatternThing(this, other)) return;

        if (this is Asteroid && other.Parent is MegaHealth or Haste or Quad) return;

        HP = Math.Max(0, HP - other.Parent.ContactDamage);

        m_isHighlighted = true;
        m_timeStartedHighlightingMS = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        m_highlightAlpha = 0.7F;
    }

    public virtual void Draw()
    {
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
            }
        }

        if (IsActor) DrawHealthBar();
        SpriteRenderer.Draw(Position, Rotation, this is Crosshair);
    }

    protected virtual void OnDeath()
    {
        Destroy();
    }

    internal virtual void Destroy()
    {
        GameState.Entities.Remove(this);
        GameState.CollisionSystem.RemoveCollider(Collider);
        SpriteRenderer.Destroy();
    }

    private void CreateHealthBarTexture()
    {
        m_healthBarTexture = new Texture2D(GameState.Root.GraphicsDevice, 1, 1);
        Color[] data = { Color.White };
        m_healthBarTexture.SetData(data);
    }

    private void DrawHealthBar()
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

        m_healthBarTexture.DrawTexture2D(source, outline, 0, new Color(outlineColor), LayerOrdering.BarOutline);

        m_healthBarTexture.DrawTexture2D
        (
            source, emptyHealthBar, 0, new Color(emptyColor), LayerOrdering.BarEmpty
        );

        m_healthBarTexture.DrawTexture2D
        (
            source, fullHealthBar, 0, new Color(fullColor), LayerOrdering.BarFull
        );
    }

    protected enum OutOfBounds { DoNothing, Wrap, Destroy }
}