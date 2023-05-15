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
    internal float HighlightAlpha;
    protected float HP;
    protected float MaxHP;
    protected bool IsActor = false;

    private Texture2D m_healthBarTexture;

    private bool m_isHighlighted;
    private long m_timeStartedHighlightingMS;
    protected OutOfBounds OutOfBoundsBehavior = OutOfBounds.Wrap;
    internal Vector2 Position;
    protected float Rotation;
    protected SpriteRenderer SpriteRenderer;
    internal Vector2 Velocity;

    internal long TimeSinceSpawned
    {
        get => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - m_timeSpawned;
    }

    protected Entity(GameplayState gameState, Vector2 position)
    {
        GameState = gameState;
        Position = position;
        m_timeSpawned = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (this is not Bullet) CreateHealthBarTexture();
    }

    internal void ApplyFriction(UpdateEventArgs e)
    {
        // apply friction
        float sign = Math.Sign(Velocity.Length());

        if (sign == 0) return;

        float direction = (float)Math.Atan2(Velocity.Y, Velocity.X);

        Velocity -=
            Vector2.UnitX.RotateVector(direction) * 0.3f * e.DeltaTime * sign;
    }

    internal virtual void OnUpdate(UpdateEventArgs e)
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

        case OutOfBounds.Bounce:
        {
            Velocity = Position.X switch
            {
                < 0 => Vector2.Reflect(Velocity, Vector2.UnitX),
                > Game1.TargetWidth => Vector2.Reflect(Velocity, -Vector2.UnitX),
                var _ => Velocity
            };

            Velocity = Position.Y switch
            {
                < 0 => Vector2.Reflect(Velocity, -Vector2.UnitY),
                > Game1.TargetHeight => Vector2.Reflect(Velocity, Vector2.UnitY),
                var _ => Velocity
            };

            break;
        }

        default:
        {
            throw new ArgumentOutOfRangeException();
        }
        }
    }

    internal virtual void OnCollision(Collider other)
    {
        if (Game1.PatternThing(this, other)) return;

        if (this is Asteroid && other.Parent is MegaHealth or Haste or Quad) return;

        HP = Math.Max(0, HP - other.Parent.ContactDamage);

        m_isHighlighted = true;
        m_timeStartedHighlightingMS = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        HighlightAlpha = 0.7F;
    }

    internal virtual void Draw()
    {
        if (m_isHighlighted)
        {
            const float decayRate = 0.005F;

            long timeNowMS = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            float timeSinceStartedS = (timeNowMS - m_timeStartedHighlightingMS) / 1000F;

            HighlightAlpha = 0.7F * MathF.Pow(decayRate, timeSinceStartedS);

            if (HighlightAlpha <= 0.01)
            {
                m_isHighlighted = false;
                HighlightAlpha = 0;
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

    protected enum OutOfBounds { DoNothing, Wrap, Bounce, Destroy }
}