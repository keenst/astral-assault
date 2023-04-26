#region
using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace AstralAssault;

public class Collider
{
    public readonly float m_mass;
    public readonly Entity Parent;
    public bool IsSolid;
    public int radius;
    public float Restitution;

    public Collider(Entity parent, bool isSolid, int mass)
    {
        Parent = parent;
        IsSolid = isSolid;
        m_mass = mass;
    }

    public Collider(Entity parent)
    {
        Parent = parent;
        IsSolid = false;
        m_mass = 0f;
    }

    public bool CollidesWith(
        Collider other,
        float deltaTime) => MathF.Sqrt
    (
        MathF.Pow(other.Parent.Position.X - Parent.Position.X, 2) + MathF.Pow(other.Parent.Position.Y - Parent.Position.Y, 2
        )
    ) <= (radius + other.radius);

    public static void ResolveCollision1(Collider a, Collider b)
    {
        if (a.IsSolid && b.IsSolid && (a.Parent.TimeSinceSpawned > 512))
        {
            if (a.m_mass + b.m_mass == 0f)
            {
                a.Parent.Velocity = Vector2.Zero;
                b.Parent.Velocity = Vector2.Zero;

                return;
            }

            var invMassA = a.m_mass > 0f ? 1 / a.m_mass : 0;
            var invMassB = b.m_mass > 0f ? 1 / b.m_mass : 0;
            Vector2 rv = b.Parent.Velocity - a.Parent.Velocity;
            Vector2 normal = Vector2.Normalize(b.Parent.Position - a.Parent.Position);
            float velAlongNormal = Vector2.Dot(rv, normal);

            if (velAlongNormal > 0) return;

            float e = MathHelper.Min(a.Restitution, b.Restitution);
            float j = (-(1 + e) * velAlongNormal) / (invMassA + invMassB);
            Vector2 impulse = j * normal;
            a.Parent.Velocity -= invMassA * impulse;
            b.Parent.Velocity += invMassB * impulse;
        }
    }
}