using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class Bullet : IUpdateEventListener
{
    private readonly Game1 _root;
    private readonly Vector2 _velocity;

    public Vector2 Position;

    private Texture2D _rect;

    public Bullet(Game1 root, Vector2 position, float rotation, float speed)
    {
        _root = root;
        Position = position;
        
        _velocity = new Vector2(
            (float)Math.Cos(rotation),
            (float)Math.Sin(rotation)
            ) * speed;
        
        _rect = new Texture2D(_root.GraphicsDevice, 2, 2);
        
        Color[] data = new Color[2 * 2];
        for(int i = 0; i < data.Length; ++i) data[i] = Color.White;
        _rect.SetData(data);

        UpdateEventSource.UpdateEvent += OnUpdate;
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        Position += _velocity * e.DeltaTime;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_rect, Position, Color.Green);
    }
}