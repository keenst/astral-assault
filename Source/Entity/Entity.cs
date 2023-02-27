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
    protected OutOfBounds OutOfBoundsBehavior = OutOfBounds.Wrap;

    protected enum OutOfBounds
    {
        DoNothing,
        Wrap,
        Destroy
    }

    public Entity(Game1 root, Vector2 position)
    {
        Root = root;
        Position = position;
        UpdateEventSource.UpdateEvent += OnUpdate;
    }
    
    public virtual void OnUpdate(object sender, UpdateEventArgs e)
    {
        Position += Velocity * e.DeltaTime;
        Collider?.SetPosition(Position.ToPoint());

        switch (OutOfBoundsBehavior)
        {
            case OutOfBounds.DoNothing:
            {
                break;
            }
            
            case OutOfBounds.Destroy:
            {
                if (Position.X is < 0 or > Game1.TargetWidth ||
                    Position.Y is < 0 or > Game1.TargetHeight)
                {
                    Destroy();
                }

                break;
            }
            
            case OutOfBounds.Wrap:
            {
                Position.X = Position.X switch
                {
                    < 0 => Game1.TargetWidth,
                    > Game1.TargetWidth => 0,
                    _ => Position.X
                };

                Position.Y = Position.Y switch
                {
                    < 0 => Game1.TargetHeight,
                    > Game1.TargetHeight => 0,
                    _ => Position.Y
                };
                
                break;
            }
        }
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