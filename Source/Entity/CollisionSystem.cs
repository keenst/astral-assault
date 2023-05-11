#region
using System;
using System.Collections.Generic;
using TheGameOfDoomHmmm.Source.Entity.Components;
#endregion

namespace TheGameOfDoomHmmm.Source.Entity.Entities;

public sealed class CollisionSystem
{
    private List<Tuple<Collider, Collider>> m_lastCollisions = new List<Tuple<Collider, Collider>>();
    internal List<Collider> Colliders { get; } = new List<Collider>();

    internal void OnUpdate()
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

    internal void AddCollider(Collider collider)
    {
        Colliders.Add(collider);
    }

    internal void RemoveCollider(Collider collider)
    {
        Colliders.Remove(collider);
    }
}