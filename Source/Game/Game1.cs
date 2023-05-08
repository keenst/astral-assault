#region
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheGameOfDoomHmmm.Source.Entity.Components;
using TheGameOfDoomHmmm.Source.Entity.Entities;
using TheGameOfDoomHmmm.Source.Entity.Entities.Items;
using TheGameOfDoomHmmm.Source.Game.GameState;
using TheGameOfDoomHmmm.Source.Graphics;
using TheGameOfDoomHmmm.Source.Input;
#endregion

namespace TheGameOfDoomHmmm.Source.Game;

public sealed class Game1 : Microsoft.Xna.Framework.Game
{
    private enum Width { Full = 1920, Half = 960, Quarter = 480 }
    private enum Height { Full = 1080, Half = 540, Quarter = 270 }

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
    public float ScaleX;
    public float ScaleY;
    public int Score;

    // debug tools
    public bool ShowDebug;

    // render
    public SpriteBatch SpriteBatch;

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

    public static bool PatternThing(Entity.Entities.Entity e1, Collider c1)
    {
        if (e1 is Bullet || c1.Parent is Bullet)
        {
            Entity.Entities.Entity e;
            Bullet b;

            if (e1 is Bullet)
            {
                e = c1.Parent;
                b = (Bullet)e1;
            }
            else
            {
                e = e1;
                b = (Bullet)c1.Parent;
            }

            switch (b.m_shootBy)
            {
            case Player when e is Player or Quad or Haste or MegaHealth:
            case ShipOfDoom when e is ShipOfDoom or Quad or Haste or MegaHealth or Asteroid: return true;
            }
        }

        if (e1 is Player || c1.Parent is Player)
        {
            Player p;
            Entity.Entities.Entity otherEnt;

            if (e1 is Player)
            {
                p = (Player)e1;
                otherEnt = c1.Parent;
            }
            else
            {
                p = (Player)c1.Parent;
                otherEnt = e1;
            }


            if (otherEnt is PowerUpBase) return true;
        }

        return e1 is Asteroid && c1.Parent is Asteroid;
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
        TextureRenderer.Init(this);
        TextRenderer.Init(this);
        InputEventSource.Init();
        Palette.Init();
        Jukebox.Init();

        GameStateMachine = new GameStateMachine(new GameOverState(this));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
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
        GraphicsDevice.SetRenderTarget(m_renderTarget);
        GraphicsDevice.Clear(BackgroundColor);

        SpriteBatch.Begin
        (
            SpriteSortMode.BackToFront, samplerState: SamplerState.PointWrap,
            depthStencilState: DepthStencilState.DepthRead
        );

        GameStateMachine.Draw();

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

            frameRate.Draw(Vector2.Zero, Color.Yellow, 0f, new Vector2(0, 0), 1f, LayerOrdering.Debug);
            renderTime.Draw(new Vector2(0, frameRate.Size().Y), Color.Yellow, 0f, new Vector2(0, 0), 1f, LayerOrdering.Debug);
        }

        SpriteBatch.End();

        // draw render target to screen
        GraphicsDevice.SetRenderTarget(null);

        SpriteBatch.Begin
        (
            SpriteSortMode.Immediate, null, SamplerState.PointWrap,
            null, null, null, m_scale
        );
        SpriteBatch.Draw
        (
            m_renderTarget,
            new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
            Color.White
        );
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}