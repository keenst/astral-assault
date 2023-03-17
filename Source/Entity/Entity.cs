using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class Entity : IUpdateEventListener
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
    protected float HighlightAlpha;

    public bool IsFriendly;

    private readonly long _timeSpawned;
    public long TimeSinceSpawned => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - _timeSpawned;

    private Texture2D _healthBarTexture;

    protected enum OutOfBounds
    {
        DoNothing,
        Wrap,
        Destroy
    }

    protected Entity(GameplayState gameState, Vector2 position)
    {
        GameState = gameState;
        Position = position;
        _timeSpawned = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        
        UpdateEventSource.UpdateEvent += OnUpdate;
        
        CreateHealthBarTexture();
    }
    
    public virtual void OnUpdate(object sender, UpdateEventArgs e)
    {
        if (IsActor && HP <= 0)
        {
            OnDeath();
            return;
        }

        if (HighlightAlpha > 0)
        {
            HighlightAlpha -= e.DeltaTime * 7;
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
                    Position.Y is < 0 or > Game1.TargetHeight)
                {
                    Destroy();
                }

                break;
            }
            
            case OutOfBounds.Wrap:
            {
                Position.X = Position.X switch
                {
                    < 0 => Game1.TargetWidth,
                    > Game1.TargetWidth => 0,
                    _ => Position.X
                };

                Position.Y = Position.Y switch
                {
                    < 0 => Game1.TargetHeight,
                    > Game1.TargetHeight => 0,
                    _ => Position.Y
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
        if (!IsActor || other.Parent.IsFriendly == IsFriendly) return;

        HP = Math.Max(0, HP - other.Parent.ContactDamage);
        
        HighlightAlpha = 0.5F;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsActor) DrawHealthBar(spriteBatch);
        SpriteRenderer.Draw(spriteBatch, Position, Rotation, HighlightAlpha);
    }

    public virtual void Destroy()
    {
        GameState.Entities.Remove(this);
        GameState.CollisionSystem.RemoveCollider(Collider);
        
        UpdateEventSource.UpdateEvent -= OnUpdate;
    }

    protected virtual void OnDeath()
    {
        Destroy();
    }

    private void CreateHealthBarTexture()
    {
        _healthBarTexture = new Texture2D(GameState.Root.GraphicsDevice, 1, 1);
        Color[] data = { Color.White };
        _healthBarTexture.SetData(data);
    }

    private void DrawHealthBar(SpriteBatch spriteBatch)
    {
        const int width = 20;
        const int height = 3;

        int filled = (int)Math.Ceiling(HP / MaxHP * width);

        int x = (int)Position.X - width / 2;
        int y = (int)Position.Y - 20;
        
        Rectangle outline = new(x - 1, y - 1, width + 2, height + 2);
        Rectangle emptyHealthBar = new(x, y, width, height);
        Rectangle fullHealthBar = new(x, y, filled, height);
        
        Rectangle source = new(0, 0, 1, 1);
        
        spriteBatch.Draw(
            _healthBarTexture, 
            outline, 
            source, 
            Color.Black, 
            0, 
            Vector2.Zero, 
            SpriteEffects.None, 
            0.03F);
        
        spriteBatch.Draw(
            _healthBarTexture, 
            emptyHealthBar, 
            source, 
            Color.Red, 
            0, 
            Vector2.Zero, 
            SpriteEffects.None, 
            0.02F);
        
        spriteBatch.Draw(
            _healthBarTexture, 
            fullHealthBar, 
            source, 
            Color.LimeGreen, 
            0, 
            Vector2.Zero, 
            SpriteEffects.None, 
            0.01F);
    }
}