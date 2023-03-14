using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public class CollisionSystem : IUpdateEventListener 
{
    public List<Collider> Colliders { get; } = new();
    private List<Tuple<Collider, Collider>> _lastCollisions = new();

    public CollisionSystem()
    {
        UpdateEventSource.UpdateEvent += OnUpdate;
    }

    public void OnUpdate(object sender, UpdateEventArgs e)
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
                    out Vector2 collisionNormal,
                    out float collisionDepth,
                    out Vector2 impulse))
                {
                    continue;
                }
                
                currentCollisions.Add(new Tuple<Collider, Collider>(collider, other));

                if (_lastCollisions.Any(t =>
                        (t.Item1 == collider && t.Item2 == other) ||
                        (t.Item2 == collider && t.Item1 == other)))
                    continue;

                if (collider.IsSolid && other.IsSolid && collider.Parent.TimeSinceSpawned > 1000)
                {
                    Vector2 forceA = impulse / e.DeltaTime;
                    Vector2 forceB = -impulse / e.DeltaTime;
                    
                    collider.Parent.Velocity += forceA / collider.Mass;
                    other.Parent.Velocity += forceB / other.Mass;
                    
                    Debug.WriteLine($"collider.Parent.Velocity: {collider.Parent.Velocity}");
                    Debug.WriteLine($"other.Parent.Velocity: {other.Parent.Velocity}");
                }

                collider.Parent.OnCollisionEnter(other);
                other.Parent.OnCollisionEnter(collider);
            }
        }

        if (currentCollisions.Count < _lastCollisions.Count)
        {
            List<Tuple<Collider, Collider>> removedCollisions = _lastCollisions.Except(currentCollisions).ToList();
            foreach (Tuple<Collider, Collider> removedCollision in removedCollisions)
            {
                removedCollision.Item1.Parent.OnCollisionExit(removedCollision.Item2);
                removedCollision.Item2.Parent.OnCollisionExit(removedCollision.Item1);
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