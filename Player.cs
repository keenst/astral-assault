using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace astral_assault;

public class Player
{
    private readonly Game1 _root;
    private Texture2D _sprite;
    private Vector2 _position;
    private Vector2 _velocity;
    private float _rotation;

    private const float MoveSpeed = 1.0F;
    private const float TiltSpeed = 0.6F;
    private const float Friction = 0.05F;
    private const float Pi = 3.14F;

    public Rectangle PositionRectangle => new((int)_position.X, (int)_position.Y, 32, 32);

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
    }
    
    private bool Input(Keys key)
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();
        return currentKeyboardState.IsKeyDown(key);
    }

    private void HandleInputs(float delta)
    {
        // acceleration and deceleration
        if (Input(Keys.W))
        {
            _velocity -= new Vector2((float)Math.Cos(_rotation), (float)Math.Sin(_rotation)) * MoveSpeed * delta;
        }

        if (Input(Keys.S))
        {
            _velocity += new Vector2((float)Math.Cos(_rotation), (float)Math.Sin(_rotation)) * MoveSpeed * delta;
        }

        // tilting
        if (Input(Keys.A))
        {
            _velocity -= new Vector2((float)Math.Sin(_rotation), (float)Math.Cos(_rotation)) * TiltSpeed * delta;
        }
        
        if (Input(Keys.D))
        {
            _velocity += new Vector2((float)Math.Sin(_rotation), (float)Math.Cos(_rotation)) * TiltSpeed * delta;
        }
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
        spriteBatch.Draw(
            _sprite, 
            PositionRectangle, 
            null, 
            Color.White, 
            0, 
            new Vector2(16, 16), 
            SpriteEffects.None, 
            0);
    }
}