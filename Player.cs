using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace astral_assault;

public class Player
{
    private readonly Game1 _root;
    private Sprite _sprite;
    private Sprite _crosshairSprite;
    private Vector2 _position;
    private Vector2 _velocity;
    private Vector2 _cursorPosition;
    private float _rotation;
    private float _spriteRot;
    private Tuple<Vector2, Vector2> _muzzle;

    private readonly Texture2D[] _playerSprites = new Texture2D[4];

    private const float MoveSpeed = 1.0F;
    private const float TiltSpeed = 0.6F;
    private const float Friction = 0.05F;
    private const float Pi = 3.14F;

    public Player(Game1 root, Vector2 position)
    {
        _root = root;
        _position = position;
        _rotation = Pi / 2;
        
        LoadContent();
    }

    private void LoadContent()
    {
        _playerSprites[0] = _root.Content.Load<Texture2D>("assets/player1");
        _playerSprites[1] = _root.Content.Load<Texture2D>("assets/player2");
        _playerSprites[2] = _root.Content.Load<Texture2D>("assets/player3");
        _playerSprites[3] = _root.Content.Load<Texture2D>("assets/player4");

        Texture2D crosshairSprite = _root.Content.Load<Texture2D>("assets/crosshair");

        _sprite = new Sprite(_playerSprites[0]);
        _crosshairSprite = new Sprite(crosshairSprite);
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
            < -16 => Game1.TargetWidth - 16,
            > Game1.TargetWidth + 16 => 16,
            _ => _position.X
        };

        _position.Y = _position.Y switch
        {
            < -16 => Game1.TargetHeight - 16,
            > Game1.TargetHeight + 16 => 16,
            _ => _position.Y
        };
        
        // rotate the points for the cannon muzzles
        Vector2 muzzle1;
        Vector2 muzzle2;
        
        const float x =  8;
        const float y = 10;

        {
            float rot = Pi / 8 * (float)Math.Round(_rotation / (Pi / 8));

            float x2 = (float)(x * Math.Cos(rot) - y * Math.Sin(rot));
            float y2 = (float)(y * Math.Cos(rot) + x * Math.Sin(rot));

            muzzle1 = new Vector2(_position.X + x2, _position.Y + y2);
        }

        {
            float rot = Pi / 8 * (float)Math.Round(_rotation / (Pi / 8));

            float x2 = (float)(x * Math.Cos(rot) + y * Math.Sin(rot));
            float y2 = (float)(-y * Math.Cos(rot) + x * Math.Sin(rot));

            muzzle2 = new Vector2(_position.X + x2, _position.Y + y2);
        }

        _muzzle = new Tuple<Vector2, Vector2>(muzzle1, muzzle2);
        
        // sprite rotation
        {
            int rot = (int)Math.Round(_rotation / (Pi / 8));
        
            if (rot % 4 == 0)
            {
                _sprite = new Sprite(_playerSprites[0]);
                _spriteRot = Pi / 8 * rot;
                return;
            }

            _spriteRot = _rotation switch
            {
                >= 0         and < Pi / 2    => 0,
                >= Pi / 2    and < Pi        => Pi / 2,
                <= 0         and > -Pi / 2   => -Pi / 2,
                <= -Pi / 2   and > -Pi       => -Pi,
                _ => 0
            };

            _sprite = new Sprite(_playerSprites[rot.Mod(4)]);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // draw player sprite
        _sprite.Draw(spriteBatch, _position, _spriteRot, true);

        // draw crosshair sprite
        _crosshairSprite.Draw(spriteBatch, _cursorPosition);

        // draw debugging tools
        if (!_root.Debug) return;
        
        Texture2D rect = new(_root.GraphicsDevice, 2, 2);
        
        Color[] data = new Color[2 * 2];
        for(int i = 0; i < data.Length; ++i) data[i] = Color.White;
        rect.SetData(data);
        
        spriteBatch.Draw(rect, _muzzle.Item1, Color.Red);
        spriteBatch.Draw(rect, _muzzle.Item2, Color.Red);
    }
}