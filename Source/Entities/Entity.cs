using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class Entity : IUpdateEventListener
{
    private Vector2 _position;
    private Vector2 _velocity;
    private float _delta;
    protected float _rotation;
    protected SpriteRenderer _spriteRenderer;

    public Entity(Vector2 position)
    {
        _position = position;
        UpdateEventSource.UpdateEvent += OnUpdate;
    }
    
    public virtual void OnUpdate(object sender, UpdateEventArgs e)
    {
        
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        _spriteRenderer.Draw(spriteBatch, _position, _rotation);
    }
}