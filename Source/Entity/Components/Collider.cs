#region
using System;
using Microsoft.Xna.Framework;
#endregion

namespace TheGameOfDoomHmmm.Source.Entity.Components;

public sealed class Collider
{
    private readonly bool m_isSolid;
    private readonly float m_mass;
    private readonly float m_restitution;
    internal readonly Entities.Entity Parent;
    internal int Radius;

    internal Collider(Entities.Entity parent, bool isSolid, int mass)
    {
        Parent = parent;
        m_isSolid = isSolid;
        m_mass = mass;
        m_restitution = 0;
    }

    internal Collider(Entities.Entity parent)
    {
        Parent = parent;
        m_isSolid = false;
        m_mass = 0f;
        m_restitution = 0;
    }

    internal bool CollidesWith(
        Collider other) => MathF.Sqrt
    (
        MathF.Pow(other.Parent.Position.X - Parent.Position.X, 2) + MathF.Pow
        (
            other.Parent.Position.Y - Parent.Position.Y, 2
        )
    ) <= (Radius + other.Radius);

    internal static void ResolveCollision(Collider a, Collider b)
    {
        if (!a.m_isSolid || !b.m_isSolid || (a.Parent.TimeSinceSpawned <= 512)) return;

        if ((a.m_mass + b.m_mass) == 0f)
        {
            a.Parent.Velocity = Vector2.Zero;
            b.Parent.Velocity = Vector2.Zero;

            return;
        }

        float invMassA = a.m_mass > 0f ? 1 / a.m_mass : 0;
        float invMassB = b.m_mass > 0f ? 1 / b.m_mass : 0;
        Vector2 rv = b.Parent.Velocity - a.Parent.Velocity;
        Vector2 normal = Vector2.Normalize(b.Parent.Position - a.Parent.Position);
        float velAlongNormal = Vector2.Dot(rv, normal);

        if (velAlongNormal > 0) return;

        float e = MathHelper.Min(a.m_restitution, b.m_restitution);
        float j = -(1 + e) * velAlongNormal / (invMassA + invMassB);
        Vector2 impulse = j * normal;
        a.Parent.Velocity -= invMassA * impulse;
        b.Parent.Velocity += invMassB * impulse;
    }
}