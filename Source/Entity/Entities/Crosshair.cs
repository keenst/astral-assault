﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault;

public class Crosshair : Entity, IMouseEventListener
{
    public Crosshair(GameplayState gameState) : base(gameState, new Vector2(0, 0))
    {
        InputEventSource.MouseButtonEvent += OnMouseButtonEvent;
        InputEventSource.MouseMoveEvent += OnMouseMoveEvent;

        Texture2D spriteSheet = AssetManager.Load<Texture2D>("Crosshair");

        Frame activeFrame   = new(new Rectangle(0,  0, 16, 16));
        Frame inactiveFrame = new(new Rectangle(64, 0, 16, 16));

        Animation deactivateAnimation = new(
            new[]
            {
                new Frame(new Rectangle(16, 0, 16, 16), 30),
                new Frame(new Rectangle(32, 0, 16, 16), 30),
                new Frame(new Rectangle(48, 0, 16, 16), 30)
            },
            false);

        Animation activateAnimation = new(
            new[]
            {
                new Frame(new Rectangle(48, 0, 16, 16), 30),
                new Frame(new Rectangle(32, 0, 16, 16), 30),
                new Frame(new Rectangle(16, 0, 16, 16), 30)
            },
            false);

        Animation activeAnimation   = new(new[] { activeFrame },   false);
        Animation inactiveAnimation = new(new[] { inactiveFrame }, false);

        Transition[] transitions = {
            new(0, 1, new[] { 2, 1 }, "IsActive", 0),
            new(1, 0, new[] { 3, 0 }, "IsActive", 1)
        };

        SpriteRenderer = new SpriteRenderer(
            spriteSheet, 
            new[] { activeAnimation, inactiveAnimation, deactivateAnimation, activateAnimation }, 
            LayerDepth.Crosshair,
            transitions,
            new[] { "IsActive" });

        OutOfBoundsBehavior = OutOfBounds.DoNothing;
    }

    public override void Destroy()
    {
        InputEventSource.MouseButtonEvent -= OnMouseButtonEvent;
        InputEventSource.MouseMoveEvent -= OnMouseMoveEvent;
        
        base.Destroy();
    }
    
    public override void OnUpdate(object sender, UpdateEventArgs e)
    {
        if (GameState.Player == null) return;
        
        Vector2 playerPosition = GameState.Player.Position;
        float distance = Vector2.Distance(playerPosition, Position);

        SpriteRenderer.SetAnimationCondition("IsActive", distance < 12 ? 0 : 1);
    }

    public void OnMouseButtonEvent(object sender, MouseButtonEventArgs e)
    {
        
    }

    public void OnMouseMoveEvent(object sender, MouseMoveEventArgs e)
    {
        Point scale = new((int)GameState.Root.ScaleX, (int)GameState.Root.ScaleY);
        Position = (e.Position / scale).ToVector2();
    }
}