using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace astral_assault;

public class Player
{
    private readonly Game1 _root;
    private Texture2D _sprite;
    private Texture2D _crosshairSprite;
    private Vector2 _position;
    private Vector2 _velocity;
    private Vector2 _cursorPosition;
    private float _rotation;

    private const float MoveSpeed = 1.0F;
    private const float TiltSpeed = 0.6F;
    private const float Friction = 0.05F;
    private const float Pi = 3.14F;

    private Rectangle PositionRectangle => new(_position.ToPoint(), new Point(32, 32));
    private Rectangle CrosshairRectangle => new(_cursorPosition.ToPoint(), new Point(16, 16));

    public Player(Game1 root, Vector2 position)
    {
        _root = root;
        _position = position;
        _rotation = Pi / 2;
        
        LoadContent();
    }

    private void LoadContent()
    {
        _sprite = _root.Content.Load<Texture2D>("assets/player1");
        _crosshairSprite = _root.Content.Load<Texture2D>("assets/crosshair");
    }
    
    private bool Input(Keys key)
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();
        return currentKeyboardState.IsKeyDown(key);
    }

    private void HandleInputs(float delta)
    {
        // acceleration and deceleration
        Vector2 forward = new Vector2(
            (float)Math.Cos(_rotation), 
            (float)Math.Sin(_rotation)
            ) * MoveSpeed * delta;

        if (Input(Keys.W)) _velocity += forward;
        
        if (Input(Keys.S)) _velocity -= forward;

        // tilting
        Vector2 right = new Vector2(
            (float)Math.Cos(_rotation + Pi / 2), 
            (float)Math.Sin(_rotation + Pi / 2)
            ) * TiltSpeed * delta;
        
        if (Input(Keys.A)) _velocity -= right;

        if (Input(Keys.D)) _velocity += right;

        // move crosshair to cursor position
        MouseState mouseState = Mouse.GetState();

        Vector2 mousePos = mouseState.Position.ToVector2();
        _cursorPosition = new Vector2(mousePos.X / _root.ScaleX, mousePos.Y / _root.ScaleY);

        float xDiff = _cursorPosition.X - _position.X;
        float yDiff = _cursorPosition.Y - _position.Y;
        _rotation = (float)Math.Atan2(yDiff, xDiff);
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        HandleInputs(delta);
        
        // apply player velocity
        _position += _velocity;

        // apply friction
        float sign = Math.Sign(_velocity.Length());

        if (sign != 0)
        {
            float direction = (float)Math.Atan2(_velocity.Y, _velocity.X);
            
            _velocity -= 
                new Vector2((float)Math.Cos(direction), (float)Math.Sin(direction)) * Friction * delta * sign;
        }
        
        // wrap position
        _position.X = _position.X switch
        {
            < 0 => Game1.TargetWidth,
            > Game1.TargetWidth => 0,
            _ => _position.X
        };

        _position.Y = _position.Y switch
        {
            < 0 => Game1.TargetHeight,
            > Game1.TargetHeight => 0,
            _ => _position.Y
        };
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // draw player sprite
        spriteBatch.Draw(
            _sprite, 
            PositionRectangle, 
            null, 
            Color.White, 
            _rotation + Pi / 2, 
            new Vector2(16, 16), 
            SpriteEffects.None, 
            0);
        
        // draw crosshair sprite
        spriteBatch.Draw(
            _crosshairSprite,
            CrosshairRectangle,
            null,
            Color.White,
            0,
            new Vector2(8, 8),
            SpriteEffects.None,
            0);
    }
}