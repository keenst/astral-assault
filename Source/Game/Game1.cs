using System;
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
    public int Score;
    public int HighScore;
    
    // render
    private SpriteBatch _spriteBatch;
    private RenderTarget2D _renderTarget;
    private static readonly Effect HighlightEffect = AssetManager.Load<Effect>("Highlight");
    private static readonly Effect ColorEffect = AssetManager.Load<Effect>("Color");

    // display
    private static readonly Color BackgroundColor = new(28, 23, 41);
    public const int TargetWidth = (int)Width.Quarter;
    public const int TargetHeight = (int)Height.Quarter;
    private Matrix _scale;
    public float ScaleX;
    public float ScaleY;
    private readonly GraphicsDeviceManager _graphics;

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
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        // set up rendering
        _graphics.PreferredBackBufferWidth = (int)Width.Half;
        _graphics.PreferredBackBufferHeight = (int)Height.Half;

        ScaleX = _graphics.PreferredBackBufferWidth / (float)TargetWidth;
        ScaleY = _graphics.PreferredBackBufferHeight / (float)TargetHeight;
        _scale = Matrix.CreateScale(new Vector3(ScaleX, ScaleY, 1));

        _graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;
        
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

        AssetManager.Init(this);
        TextRenderer.Init();
        InputEventSource.Init();
        Palette.Init();
        Jukebox.Init();
        
        GameStateMachine = new GameStateMachine(new GameplayState(this));

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

        if (Keyboard.GetState().IsKeyDown(Keys.F) && !_prevKeyState.IsKeyDown(Keys.F))
        {
            if (_graphics.IsFullScreen)
            {
                _graphics.PreferredBackBufferWidth = (int)Width.Half;
                _graphics.PreferredBackBufferHeight = (int)Height.Half;
                _graphics.IsFullScreen = false;
                _graphics.ApplyChanges();
            }
            else
            {
                _graphics.PreferredBackBufferWidth = (int)Width.Full;
                _graphics.PreferredBackBufferHeight = (int)Height.Full;
                _graphics.IsFullScreen = true;
                _graphics.ApplyChanges();
            }
        }

        _prevKeyState = Keyboard.GetState();

        UpdateEventSource.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // draw sprites to render target
        GraphicsDevice.SetRenderTarget(_renderTarget);
        
        GraphicsDevice.Clear(BackgroundColor);

        List<DrawTask> drawTasks = new();

        drawTasks.AddRange(GameStateMachine.GetDrawTasks());
        
        drawTasks = drawTasks.OrderBy(dt => (int)dt.LayerDepth).ToList();

        _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap);
        
        foreach (DrawTask drawTask in drawTasks)
        {
            foreach (IDrawTaskEffect effect in drawTask.EffectContainer.Effects)
            {
                switch (effect)
                {
                    case HighlightEffect highlightEffect:
                        HighlightEffect.CurrentTechnique.Passes[1].Apply();
                        HighlightEffect.Parameters["blendAlpha"].SetValue(highlightEffect.Alpha);
                        HighlightEffect.CurrentTechnique.Passes[0].Apply();
                        break;
                    
                    case ColorEffect colorEffect:
                        ColorEffect.CurrentTechnique.Passes[1].Apply();
                        ColorEffect.Parameters["newColor"].SetValue(colorEffect.Color);
                        ColorEffect.CurrentTechnique.Passes[0].Apply();
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
            
            HighlightEffect.CurrentTechnique.Passes[1].Apply();
            ColorEffect.CurrentTechnique.Passes[1].Apply();
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
