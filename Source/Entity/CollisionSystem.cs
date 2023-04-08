using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public class CollisionSystem
{
    public List<Collider> Colliders { get; } = new List<Collider>();
    private List<Tuple<Collider, Collider>> m_lastCollisions = new List<Tuple<Collider, Collider>>();

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
                        other,
                        e.DeltaTime,
                        out Vector2 initialImpulseThis,
                        out Vector2 initialImpulseOther,
                        out Vector2 totalImpulseThis,
                        out Vector2 totalImpulseOther
                    ))
                    continue;

                if (collider.IsSolid && other.IsSolid && (collider.Parent.TimeSinceSpawned > 512))
                {
                    collider.Parent.Position += initialImpulseThis * e.DeltaTime;
                    other.Parent.Position += initialImpulseOther * e.DeltaTime;

                    collider.Parent.Velocity += totalImpulseThis * e.DeltaTime * 1000F;
                    other.Parent.Velocity += totalImpulseOther * e.DeltaTime * 1000F;

                    collider.SetPosition(collider.Parent.Position.ToPoint());
                    other.SetPosition(other.Parent.Position.ToPoint());
                }

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