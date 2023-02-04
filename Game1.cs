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
    
    private SpriteBatch _spriteBatch;

    private Player _player;
    public readonly List<Bullet> Bullets = new();

    public const int TargetWidth = (int)Width.Quarter;
    public const int TargetHeight = (int)Height.Quarter;
    private readonly Matrix _scale;
    public readonly float ScaleX;
    public readonly float ScaleY;

    public bool Debug;

    private KeyboardState _prevKeyState = Keyboard.GetState();

    private RenderTarget2D _renderTarget;
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

        Debug = false;
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
        
        // initialize objects
        _player = new Player(this, new Vector2(TargetWidth / 2F, TargetHeight / 2F));
        
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
            Debug = !Debug;

        _prevKeyState = Keyboard.GetState();

        _player.Update(gameTime);
        
        for (int i = 0; i < Bullets.Count; i++)
        {
            if (Bullets[i]._position.X is > TargetWidth or < 0 ||
                Bullets[i]._position.Y is > TargetHeight or < 0)
            {
                Bullets.RemoveAt(i);
                return;
            }
            
            Bullets[i].Update(gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // draw sprites to render target
        GraphicsDevice.SetRenderTarget(_renderTarget);
        
        GraphicsDevice.Clear(Color.DarkGray);
        
        _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap);
        _player.Draw(_spriteBatch);
        foreach (Bullet bullet in Bullets) bullet.Draw(_spriteBatch);
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
