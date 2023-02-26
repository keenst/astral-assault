﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace astral_assault;

public class Game1 : Game
{
    private enum Height
    {
        Full = 1080,
        Half = 540,
        Quarter = 270
    }

    private enum Width
    {
        Full = 1920,
        Half = 960,
        Quarter = 480
    }
    
    // render
    private SpriteBatch _spriteBatch;
    private RenderTarget2D _renderTarget;

    // entities
    public readonly List<Entity> Entities = new();

    // display
    public const int TargetWidth = (int)Width.Quarter;
    public const int TargetHeight = (int)Height.Quarter;
    private readonly Matrix _scale;
    public readonly float ScaleX;
    public readonly float ScaleY;

    // debug tools
    public bool ShowDebug;
    private float _frameRate;
    private float _renderTime;
    private long _lastStatUpdate;
    private const int StatUpdateInterval = 300;
    private KeyboardState _prevKeyState = Keyboard.GetState();

    public Game1()
    {
        // set up game class
        GraphicsDeviceManager graphics = new(this);
        Content.RootDirectory = "Content";

        // set up rendering
        graphics.PreferredBackBufferWidth = (int)Width.Half;
        graphics.PreferredBackBufferHeight = (int)Height.Half;

        ScaleX = graphics.PreferredBackBufferWidth / (float)TargetWidth;
        ScaleY = graphics.PreferredBackBufferHeight / (float)TargetHeight;
        _scale = Matrix.CreateScale(new Vector3(ScaleX, ScaleY, 1));

        graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;

        // set up debug tools
        ShowDebug = false;
    }

    protected override void Initialize()
    {
        _renderTarget = new RenderTarget2D(
            GraphicsDevice,
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);
        
        Entities.Add(new Player(this, new Vector2(TargetWidth / 2F, TargetHeight / 2F)));
        Entities.Add(new Asteroid(this, new Vector2(TargetWidth / 3F, TargetHeight / 3F)));
        Entities.Add(new Crosshair(this, new Vector2(0, 0)));
        
        Text.Initialize(this);
        InputEventSource.Initialize();
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (Keyboard.GetState().IsKeyDown(Keys.F3) && !_prevKeyState.IsKeyDown(Keys.F3))
            ShowDebug = !ShowDebug;

        _prevKeyState = Keyboard.GetState();

        UpdateEventSource.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // draw sprites to render target
        GraphicsDevice.SetRenderTarget(_renderTarget);
        
        GraphicsDevice.Clear(Color.DarkGray);
        
        _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap);
        foreach (Entity e in Entities) e.Draw(_spriteBatch);

        if (ShowDebug)
        {
            long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            
            if (_lastStatUpdate + StatUpdateInterval < timeNow)
            {
                _frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
                _renderTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                _lastStatUpdate = timeNow;
            }
            
            _spriteBatch.Write(
                Math.Round(_frameRate).ToString(), 
                new Vector2(0, 0), 
                Color.Yellow);
            
            _spriteBatch.Write(
                _renderTime.ToString(), 
                new Vector2(0, 9), 
                Color.Yellow);
        }
        
        _spriteBatch.End();

        // draw render target to screen
        GraphicsDevice.SetRenderTarget(null);

        _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap,
            null, null, null, _scale);
        _spriteBatch.Draw(
            _renderTarget,
            new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
            Color.White);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}
