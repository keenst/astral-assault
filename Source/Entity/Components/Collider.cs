#region
using System;
using Microsoft.Xna.Framework;
#endregion

namespace AstralAssault;

public class Collider
{
    public readonly float Mass;
    public readonly Entity Parent;
    public bool IsSolid;
    public int Radius;
    public float Restitution;

    public Collider(Entity parent, bool isSolid, int mass)
    {
        Parent = parent;
        IsSolid = isSolid;
        Mass = mass;
        Restitution = 0;
    }

    public Collider(Entity parent)
    {
        Parent = parent;
        IsSolid = false;
        Mass = 0f;
        Restitution = 0;
    }

    public Collider(Entity parent, Rectangle rectangle)
    {
        Parent = parent;
        IsSolid = false;
        Mass = 0f;
    }

    public bool CollidesWith(
        Collider other,
        float deltaTime) => MathF.Sqrt
    (
        MathF.Pow(other.Parent.Position.X - Parent.Position.X, 2) + MathF.Pow
        (
            other.Parent.Position.Y - Parent.Position.Y, 2
        )
    ) <= (Radius + other.Radius);

    public static void ResolveCollision1(Collider a, Collider b)
    {
        if (a.IsSolid && b.IsSolid && (a.Parent.TimeSinceSpawned > 512))
        {
            if ((a.Mass + b.Mass) == 0f)
            {
                a.Parent.Velocity = Vector2.Zero;
                b.Parent.Velocity = Vector2.Zero;

                return;
            }

            float invMassA = a.Mass > 0f ? 1 / a.Mass : 0;
            float invMassB = b.Mass > 0f ? 1 / b.Mass : 0;
            Vector2 rv = b.Parent.Velocity - a.Parent.Velocity;
            Vector2 normal = Vector2.Normalize(b.Parent.Position - a.Parent.Position);
            float velAlongNormal = Vector2.Dot(rv, normal);

            if (velAlongNormal > 0) return;

            float e = MathHelper.Min(a.Restitution, b.Restitution);
            float j = -(1 + e) * velAlongNormal / (invMassA + invMassB);
            Vector2 impulse = j * normal;
            a.Parent.Velocity -= invMassA * impulse;
            b.Parent.Velocity += invMassB * impulse;
        }
    }
}