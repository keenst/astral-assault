#region
using System;
using System.Collections.Generic;
#endregion

namespace AstralAssault;

public class CollisionSystem
{
    private List<Tuple<Collider, Collider>> m_lastCollisions = new List<Tuple<Collider, Collider>>();
    public List<Collider> Colliders { get; } = new List<Collider>();

    public void OnUpdate(object sender, UpdateEventArgs e)
    {
        List<Tuple<Collider, Collider>> currentCollisions = new List<Tuple<Collider, Collider>>();

        for (int i = 0; i < (Colliders.Count - 1); i++)
        {
            Collider collider = Colliders[i];

            for (int j = i + 1; j < Colliders.Count; j++)
            {
                Collider other = Colliders[j];

                if (collider == other) continue;

                if (!collider.CollidesWith
                    (
                        other
                    ))
                    continue;

                Collider.ResolveCollision1(collider, other);

                Tuple<Collider, Collider> colliderPair = new Tuple<Collider, Collider>(collider, other);
                currentCollisions.Add(colliderPair);

                if (m_lastCollisions.Contains(colliderPair)) continue;

                collider.Parent.OnCollision(other);
                other.Parent.OnCollision(collider);
            }
        }

        m_lastCollisions = currentCollisions;
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