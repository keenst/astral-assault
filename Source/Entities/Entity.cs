using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class Entity : IUpdateEventListener
{
    protected Vector2 Position;
    protected Vector2 Velocity;
    protected float Rotation;
    protected SpriteRenderer SpriteRenderer;
    
    private float _delta;

    public Entity(Vector2 position)
    {
        Position = position;
        UpdateEventSource.UpdateEvent += OnUpdate;
    }
    
    public virtual void OnUpdate(object sender, UpdateEventArgs e)
    {
        
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        SpriteRenderer.Draw(spriteBatch, Position, Rotation);
    }
}