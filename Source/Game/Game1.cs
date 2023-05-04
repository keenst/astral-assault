#region
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace AstralAssault;

public class Game1 : Game
{
    public const int TargetWidth = (int)Width.Quarter;
    public const int TargetHeight = (int)Height.Quarter;
    private const int StatUpdateInterval = 300;

    // display
    private static readonly Color BackgroundColor = new Color(28, 23, 41);
    private readonly GraphicsDeviceManager m_graphics;
    private readonly Matrix m_scale;

    public GameStateMachine GameStateMachine;
    public int HighScore;
    private float m_frameRate;
    private long m_lastStatUpdate;
    private KeyboardState m_prevKeyState = Keyboard.GetState();
    private RenderTarget2D m_renderTarget;
    private float m_renderTime;

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
        ReadOnlySpan<DrawTask> fullscreenTextTasks =
            fullscreenText.AsSpan().CreateDrawTasks(new Vector2(4, 258), Color.White, LayerDepth.HUD);
        drawTasks.AddRange(fullscreenTextTasks.ToArray());

        drawTasks.AddRange(GameStateMachine.GetDrawTasks());

        for (int i = 0; i < (drawTasks.Count - 1); i++)
        {
            int minIndex = i;

            for (int j = i + 1; j < drawTasks.Count; j++)
            {
                if ((int)drawTasks[j].LayerDepth < (int)drawTasks[minIndex].LayerDepth) minIndex = j;
            }

            if (minIndex != i)
            {
                DrawTask temp = drawTasks[i];
                drawTasks[i] = drawTasks[minIndex];
                drawTasks[minIndex] = temp;
            }
        }

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

            ReadOnlySpan<DrawTask> frameRateTask =
                frameRate.AsSpan().CreateDrawTasks(Vector2.Zero, Color.Yellow, LayerDepth.Debug);
            ReadOnlySpan<DrawTask> renderTimeTask =
                renderTime.AsSpan().CreateDrawTasks(new Vector2(0, 9), Color.Yellow, LayerDepth.Debug);

            drawTasks.AddRange(frameRateTask.ToArray());
            drawTasks.AddRange(renderTimeTask.ToArray());
        }

        GraphicsDevice.SetRenderTarget(m_renderTarget);
        GraphicsDevice.Clear(BackgroundColor);

        m_spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointWrap);

        foreach (DrawTask drawTask in drawTasks)
        {
            m_spriteBatch.Draw
            (
                drawTask.Texture,
                drawTask.Destination,
                drawTask.Source,
                drawTask.Color,
                drawTask.Rotation,
                drawTask.Origin,
                SpriteEffects.None,
                0
            );
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

    private enum Width { Full = 1920, Half = 960, Quarter = 480 }
    private enum Height { Full = 1080, Half = 540, Quarter = 270 }
}