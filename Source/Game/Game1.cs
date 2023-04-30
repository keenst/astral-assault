#region
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace AstralAssault;

public class Game1 : Game
{
    private enum Width { Full = 1920, Half = 960, Quarter = 480 }
    private enum Height { Full = 1080, Half = 540, Quarter = 270 }

    public const int TargetWidth = (int)Width.Quarter;
    public const int TargetHeight = (int)Height.Quarter;
    private const int StatUpdateInterval = 300;
    private static readonly Effect HighlightEffect = AssetManager.Load<Effect>("Highlight");
    private static readonly Effect ColorEffect = AssetManager.Load<Effect>("Color");

    // display
    private static readonly Color BackgroundColor = new Color(28, 23, 41);
    private readonly GraphicsDeviceManager m_graphics;

    public GameStateMachine GameStateMachine;
    public int HighScore;
    private float m_frameRate;
    private long m_lastStatUpdate;
    private KeyboardState m_prevKeyState = Keyboard.GetState();
    private RenderTarget2D m_renderTarget;
    private float m_renderTime;
    private readonly Matrix m_scale;

    // render
    public SpriteBatch m_spriteBatch;
    public float ScaleX;
    public float ScaleY;
    public int Score;

    // debug tools
    public bool ShowDebug;

    public Game1()
    {
        // set up game class
        m_graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        // set up rendering
        m_graphics.PreferredBackBufferWidth = (int)Width.Half;
        m_graphics.PreferredBackBufferHeight = (int)Height.Half;

        ScaleX = m_graphics.PreferredBackBufferWidth / (float)TargetWidth;
        ScaleY = m_graphics.PreferredBackBufferHeight / (float)TargetHeight;
        m_scale = Matrix.CreateScale(new Vector3(ScaleX, ScaleY, 1));

        m_graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;

        ShowDebug = false;
    }

    protected override void Initialize()
    {
        m_renderTarget = new RenderTarget2D
        (
            GraphicsDevice,
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24
        );

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
        m_spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (Keyboard.GetState().IsKeyDown(Keys.F2) && !m_prevKeyState.IsKeyDown(Keys.F3)) ShowDebug = !ShowDebug;

        if (Keyboard.GetState().IsKeyDown(Keys.F) && !m_prevKeyState.IsKeyDown(Keys.F))
        {
            if (m_graphics.IsFullScreen)
            {
                m_graphics.PreferredBackBufferWidth = (int)Width.Half;
                m_graphics.PreferredBackBufferHeight = (int)Height.Half;
                m_graphics.IsFullScreen = false;
                m_graphics.ApplyChanges();
            }
            else
            {
                m_graphics.PreferredBackBufferWidth = (int)Width.Full;
                m_graphics.PreferredBackBufferHeight = (int)Height.Full;
                m_graphics.IsFullScreen = true;
                m_graphics.ApplyChanges();
            }
        }

        m_prevKeyState = Keyboard.GetState();

        UpdateEventSource.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // draw sprites to render target

        List<DrawTask> drawTasks = new List<DrawTask>();

        string fullscreenText = "Press F for fullscreen";
        List<DrawTask> fullscreenTextTasks =
            fullscreenText.CreateDrawTasks(new Vector2(4, 258), Color.White, LayerDepth.Background);
        drawTasks.AddRange(fullscreenTextTasks);

        drawTasks.AddRange(GameStateMachine.GetDrawTasks());

        drawTasks = drawTasks.OrderBy(dt => (int)dt.LayerDepth).ToList();

        if (ShowDebug)
        {
            long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            if ((m_lastStatUpdate + StatUpdateInterval) < timeNow)
            {
                m_frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
                m_renderTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                m_lastStatUpdate = timeNow;
            }

            string frameRate = Math.Round(m_frameRate).ToString();
            string renderTime = m_renderTime.ToString();

            List<DrawTask> frameRateTask =
                frameRate.CreateDrawTasks(Vector2.Zero, Color.Yellow, LayerDepth.Debug);
            List<DrawTask> renderTimeTask =
                renderTime.CreateDrawTasks(new Vector2(0, 9), Color.Yellow, LayerDepth.Debug);

            drawTasks.AddRange(frameRateTask);
            drawTasks.AddRange(renderTimeTask);
        }

        GraphicsDevice.SetRenderTarget(m_renderTarget);
        GraphicsDevice.Clear(BackgroundColor);

        m_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap);

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

            m_spriteBatch.Draw
            (
                drawTask.Texture,
                drawTask.Destination,
                drawTask.Source,
                Color.White,
                drawTask.Rotation,
                drawTask.Origin,
                SpriteEffects.None,
                0
            );

            HighlightEffect.CurrentTechnique.Passes[1].Apply();
            ColorEffect.CurrentTechnique.Passes[1].Apply();
        }

        m_spriteBatch.End();

        // draw render target to screen
        GraphicsDevice.SetRenderTarget(null);

        m_spriteBatch.Begin
        (
            SpriteSortMode.Immediate, null, SamplerState.PointWrap,
            null, null, null, m_scale
        );
        m_spriteBatch.Draw
        (
            m_renderTarget,
            new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
            Color.White
        );
        m_spriteBatch.End();

        base.Draw(gameTime);
    }
}