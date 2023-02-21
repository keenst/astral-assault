using System;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timer = System.Timers.Timer;

namespace astral_assault;

public class FlameRenderer
{
    private readonly Vector2[] _points = new Vector2[5];
    private readonly Texture2D[] _textures = new Texture2D[5];

    private bool _isOn;
    private float _followSpeed;
    private int _transitionStage;
    private int _transitionSign;
    private const float FollowSpeedMax = 20;
    private const float FollowSpeed = 4;
    private const float FollowSpeedMin = 0;
    private const int MaxTransitionStage = 5;
    
    private Game1 _root;

    public FlameRenderer(Game1 root)
    {
        _root = root;

        _textures[0] = new Texture2D(_root.GraphicsDevice, 6, 6);
        _textures[1] = new Texture2D(_root.GraphicsDevice, 5, 5);
        _textures[2] = new Texture2D(_root.GraphicsDevice, 4, 4);
        _textures[3] = new Texture2D(_root.GraphicsDevice, 3, 3);
        _textures[4] = new Texture2D(_root.GraphicsDevice, 2, 2);

        foreach (Texture2D texture in _textures)
        {
            int width = texture.Width;
            
            Color[] data = new Color[width * width];
            for(int i = 0; i < data.Length; ++i) data[i] = Color.White;
            texture.SetData(data);
        }

        Timer timer = new(100);
        timer.Elapsed += OnTimer;
        timer.Enabled = true;
    }

    public void Update(Vector2 origin, float offset)
    {
        if (offset == 0) return;
        
        _points[0] = origin;

        for (int i = 1; i < _points.Length; i++)
        {
            if (!(Vector2.Distance(_points[i - 1], _points[i]) > offset)) continue;
            
            float diffX = _points[i - 1].X - _points[i].X;
            float diffY = _points[i - 1].Y - _points[i].Y;
            
            float directionBetween = (float)Math.Atan2(diffY, diffX);

            Vector2 forward = new(
                (float)Math.Cos(directionBetween),
                (float)Math.Sin(directionBetween));

            _points[i] += forward * _followSpeed;
        }
    }

    private void OnTimer(object sender, ElapsedEventArgs elapsedEventArgs)
    {
        if (_transitionSign == 0) return;
        
        if (_transitionStage is MaxTransitionStage or MaxTransitionStage * -1)
        {
            _transitionSign = 0;
        }
        
        _transitionStage += _transitionSign;

        // TODO: make more readable
        _followSpeed += _transitionSign * _transitionSign > 0
            ? FollowSpeedMax * _transitionStage / MaxTransitionStage
            : FollowSpeedMin * _transitionStage / MaxTransitionStage;
    }

    public void Start()
    {
        _transitionSign = 1;
    }

    public void Stop()
    {
        _transitionSign = -1;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_transitionStage is MaxTransitionStage or MaxTransitionStage * -1) return;
        
        for (int i = 0; i < _points.Length; i++)
        {
            float half = _textures[i].Width / 2F;
            
            spriteBatch.Draw(
                _textures[i], 
                _points[i], 
                null, 
                i switch
                {
                    0 => Color.White,
                    1 => Color.Aqua,
                    2 => Color.Yellow,
                    3 => Color.Red,
                    4 => Color.Beige
                }, 
                0,
                new Vector2(half, half),
                new Vector2(1, 1),
                SpriteEffects.None,
                0);
        }
    }
}