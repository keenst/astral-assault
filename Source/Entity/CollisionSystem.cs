using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public class CollisionSystem 
{
    public List<Collider> Colliders { get; } = new();
    private List<Tuple<Collider, Collider>> _lastCollisions = new();

    public CollisionSystem()
    {
    }

    public void Update(float deltaTime)
    {
        List<Tuple<Collider, Collider>> currentCollisions = new();

        for (int i = 0; i < Colliders.Count - 1; i++)
        {
            Collider collider = Colliders[i];
            for (int j = i + 1; j < Colliders.Count; j++)
            {
                Collider other = Colliders[j];
                if (collider == other) continue;

                if (!collider.CollidesWith(
                        other, 
                        deltaTime,
                        out Vector2 initialImpulseThis,
                        out Vector2 initialImpulseOther,
                        out Vector2 totalImpulseThis,
                        out Vector2 totalImpulseOther)) 
                    continue;

                if (collider.IsSolid && other.IsSolid && collider.Parent.TimeSinceSpawned > 1000)
                {
                    collider.Parent.Position += initialImpulseThis * deltaTime;
                    other.Parent.Position += initialImpulseOther * deltaTime;
                    
                    collider.Parent.Velocity += totalImpulseThis * deltaTime * 1000F;
                    other.Parent.Velocity += totalImpulseOther * deltaTime * 1000F;
                    
                    collider.SetPosition(collider.Parent.Position.ToPoint());
                    other.SetPosition(other.Parent.Position.ToPoint());
                }

                Tuple<Collider, Collider> colliderPair = new(collider, other);
                currentCollisions.Add(colliderPair);
                if (_lastCollisions.Contains(colliderPair)) continue;

                collider.Parent.OnCollision(other);
                other.Parent.OnCollision(collider);
            }
        }

        _lastCollisions = currentCollisions;
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