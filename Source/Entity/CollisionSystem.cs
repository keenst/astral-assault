using System.Collections.Generic;

namespace astral_assault;

public class CollisionSystem : IUpdateEventListener 
{
    public List<Collider> Colliders { get; } = new();

    public CollisionSystem()
    {
        UpdateEventSource.UpdateEvent += OnUpdate;
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        foreach (Collider collider in Colliders)
        {
            foreach (Collider other in Colliders)
            {
                if (collider == other) continue;

                if (!collider.CollidesWith(other)) continue;
                
                collider.Parent.OnCollision(other);
                other.Parent.OnCollision(collider);
            }
        }
    }

    public void AddCollider(Collider collider)
    {
        Colliders.Add(collider);
    }

    public void RemoveCollider(Collider collider)
    {
        Colliders.Remove(collider);
    }
}