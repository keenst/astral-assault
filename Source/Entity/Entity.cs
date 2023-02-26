using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace astral_assault;

public class Entity : IUpdateEventListener
{
    protected Vector2 Position;
    protected Vector2 Velocity;
    protected float Rotation;
    protected SpriteRenderer SpriteRenderer;
    protected Collider Collider;
    protected Game1 Root;
    
    private float _delta;

    public Entity(Game1 root, Vector2 position)
    {
        Root = root;
        Position = position;
        UpdateEventSource.UpdateEvent += OnUpdate;
    }
    
    public virtual void OnUpdate(object sender, UpdateEventArgs e)
    {
        
    }

    public void OnCollision(Collider other)
    {
        Debug.WriteLine("collision!");
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        SpriteRenderer.Draw(spriteBatch, Position, Rotation);
    }

    public void Destroy()
    {
        Root.Entities.Remove(this);
        Root.CollisionSystem.RemoveCollider(Collider);
    }
}