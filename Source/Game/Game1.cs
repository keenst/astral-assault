﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace AstralAssault;

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

    public GameStateMachine GameStateMachine;

    // render
    private SpriteBatch _spriteBatch;
    private RenderTarget2D _renderTarget;
    private static readonly Effect HighlightEffect = AssetManager.Load<Effect>("Highlight");
    private static readonly Effect ColorEffect = AssetManager.Load<Effect>("Color");

    // display
    private static readonly Color BackgroundColor = new(28, 23, 41);
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

        ScaleX = graphics.PreferredBackBufferWidth / (float)Game1.TargetWidth;
        ScaleY = graphics.PreferredBackBufferHeight / (float)Game1.TargetHeight;
        _scale = Matrix.CreateScale(new Vector3(ScaleX, ScaleY, 1));

        graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;

        ShowDebug = false;
    }

    protected override void Initialize()
    {
        _renderTarget = new(
            GraphicsDevice,
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);

        AssetManager.Init(this);
        TextRenderer.Init();
        InputEventSource.Init();
        Palette.Init();

        GameStateMachine = new(new GameplayState(this));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new(GraphicsDevice);
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

        GraphicsDevice.Clear(Game1.BackgroundColor);

        List<DrawTask> drawTasks = GameStateMachine.GetDrawTasks().OrderBy(dt => (int)dt.LayerDepth).ToList();

        if (ShowDebug)
        {
            long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            if (_lastStatUpdate + Game1.StatUpdateInterval < timeNow)
            {
                _frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
                _renderTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                _lastStatUpdate = timeNow;
            }

            string frameRate = Math.Round(_frameRate).ToString();
            string renderTime = _renderTime.ToString();

            List<DrawTask> frameRateTask =
                frameRate.CreateDrawTasks(Vector2.Zero, Palette.GetColor(Palette.Colors.Yellow9), LayerDepth.Debug);
            List<DrawTask> renderTimeTask =
                renderTime.CreateDrawTasks(new(0, 9), Palette.GetColor(Palette.Colors.Yellow9), LayerDepth.Debug);

            drawTasks.AddRange(frameRateTask);
            drawTasks.AddRange(renderTimeTask);
        }

        _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap);

        foreach (DrawTask drawTask in drawTasks)
        {
            foreach (IDrawTaskEffect effect in drawTask.EffectContainer.Effects)
            {
                switch (effect)
                {
                    case HighlightEffect highlightEffect:
                        Game1.HighlightEffect.CurrentTechnique.Passes[1].Apply();
                        Game1.HighlightEffect.Parameters["blendAlpha"].SetValue(highlightEffect.Alpha);
                        Game1.HighlightEffect.CurrentTechnique.Passes[0].Apply();

                        break;

                    case ColorEffect colorEffect:
                        Game1.ColorEffect.CurrentTechnique.Passes[1].Apply();
                        Game1.ColorEffect.Parameters["newColor"].SetValue(colorEffect.Color);
                        Game1.ColorEffect.CurrentTechnique.Passes[0].Apply();

                        break;
                }
            }

            _spriteBatch.Draw(
                drawTask.Texture,
                drawTask.Destination,
                drawTask.Source,
                Color.White,
                drawTask.Rotation,
                drawTask.Origin,
                SpriteEffects.None,
                0);

            Game1.HighlightEffect.CurrentTechnique.Passes[1].Apply();
            Game1.ColorEffect.CurrentTechnique.Passes[1].Apply();
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