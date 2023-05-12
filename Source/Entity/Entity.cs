﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector4 = Microsoft.Xna.Framework.Vector4;

namespace AstralAssault;

public class Entity : IUpdateEventListener
{
    public Vector2 Position;
    public Vector2 Velocity;
    public float ContactDamage;
    protected float Rotation;
    protected Collider Collider;
    protected SpriteRenderer SpriteRenderer;
    protected readonly GameplayState GameState;
    protected OutOfBounds OutOfBoundsBehavior = OutOfBounds.Wrap;
    protected bool IsActor = false;
    protected float MaxHP;
    protected float HP;
    protected int HealthBarYOffset = 20;
    
    private bool _isHighlighted;
    private long _timeStartedHighlightingMS;
    private float _highlightAlpha;

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

        if (this is Player)
        {
            Player player = (Player)this;
            player.HandleDamage(other.Parent.ContactDamage);
        }
        else
        {
            HP = Math.Max(0, HP - other.Parent.ContactDamage);
        }

        _isHighlighted = true;
        _timeStartedHighlightingMS = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        _highlightAlpha = 0.7F;
        
        SpriteRenderer.EffectContainer.SetEffect<HighlightEffect, float>(_highlightAlpha);
    }
    
    public virtual List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new();
        
        if (_isHighlighted)
        {
            const float decayRate = 0.005F;
            
            long timeNowMS = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            float timeSinceStartedS = (timeNowMS - _timeStartedHighlightingMS) / 1000F;
            
            _highlightAlpha = 0.7F * MathF.Pow(decayRate, timeSinceStartedS);
            
            if (_highlightAlpha <= 0.01)
            {
                _isHighlighted = false;
                _highlightAlpha = 0;
                SpriteRenderer.EffectContainer.RemoveEffect<HighlightEffect>();
            }
            else
            {
                SpriteRenderer.EffectContainer.SetEffect<HighlightEffect, float>(_highlightAlpha);
            }
        }
        
        Vector4 fullColor = Palette.GetColorVector(Palette.Colors.Green7);
        if (IsActor) drawTasks.AddRange(CreateBarDrawTasks(HP, MaxHP, fullColor, HealthBarYOffset));
        drawTasks.Add(SpriteRenderer.CreateDrawTask(Position, Rotation));

        return drawTasks;
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

    protected List<DrawTask> CreateBarDrawTasks(float value, float maxValue, Vector4 fillColor, int yOffset)
    {
        const int width = 20;
        const int height = 3;

        int filled = (int)Math.Ceiling(value / maxValue * width);
        
        int x = (int)Position.X - width / 2;
        int y = (int)Position.Y - yOffset;
        
        Rectangle outline = new(x - 1, y - 1, width + 2, height + 2);
        Rectangle emptyHealthBar = new(x, y, width, height);
        Rectangle fullHealthBar = new(x, y, filled, height);

        Vector4 outlineColor = Palette.GetColorVector(Palette.Colors.Black);
        Vector4 emptyColor = Palette.GetColorVector(Palette.Colors.Red6);

        Rectangle source = new(0, 0, 1, 1);
        
        DrawTask background = new(
            _healthBarTexture, 
            source, 
            outline,
            0, 
            LayerDepth.HUD, 
            new List<IDrawTaskEffect> { new ColorEffect(outlineColor) },
            Color.Black);
        
        DrawTask empty = new(
            _healthBarTexture, 
            source, 
            emptyHealthBar,
            0, 
            LayerDepth.HUD, 
            new List<IDrawTaskEffect> { new ColorEffect(emptyColor) },
            Color.Red);
        
        DrawTask full = new(
            _healthBarTexture, 
            source, 
            fullHealthBar,
            0, 
            LayerDepth.HUD, 
            new List<IDrawTaskEffect> { new ColorEffect(fillColor) }, 
            Color.LimeGreen);
        
        return new List<DrawTask> { background, empty, full };
    }
}