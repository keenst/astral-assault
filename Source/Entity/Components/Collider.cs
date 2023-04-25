#region
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace AstralAssault;

public class Collider
{
    private readonly float m_mass;
    public readonly Entity Parent;
    public bool IsSolid;
    public int radius;
    public float Restitution;

    public Collider(Entity parent, bool isSolid, float mass)
    {
        Parent = parent;
        IsSolid = isSolid;
        m_mass = mass;
    }

    public Collider(Entity parent)
    {
        Parent = parent;
        IsSolid = false;
        m_mass = 0;
    }

    public bool CollidesWith(
        Collider other,
        float deltaTime) => MathF.Sqrt
    (
        MathF.Pow(other.Parent.Position.X - Parent.Position.X, 2) + MathF.Pow(other.Parent.Position.Y - Parent.Position.Y, 2
        )
    ) < (radius + other.radius);

    public static void ResolveCollision(Collider a, Collider b)
    {
        if (a.IsSolid && b.IsSolid && (a.Parent.TimeSinceSpawned > 512))
        {
            if (a.m_mass + b.m_mass == 0)
            {
                a.Parent.Velocity = Vector2.Zero;
                b.Parent.Velocity = Vector2.Zero;

                return;
            }

            var invMassA = a.m_mass > 0 ? 1 / a.m_mass : 0;
            var invMassB = b.m_mass > 0 ? 1 / b.m_mass : 0;
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

    private static Vector2[] GetCorners(Rectangle rect)
    {
        Vector2[] corners = new Vector2[4];
        corners[0] = new Vector2(rect.Left, rect.Top);
        corners[1] = new Vector2(rect.Right, rect.Top);
        corners[2] = new Vector2(rect.Right, rect.Bottom);
        corners[3] = new Vector2(rect.Left, rect.Bottom);

        return corners;
    }

    private static Vector2[] GetAxes(Vector2[] corners)
    {
        List<Vector2> axes = new List<Vector2>();

        for (int i = 0; i < corners.Length; i++)
        {
            Vector2 edge = corners[(i + 1) % corners.Length] - corners[i];
            Vector2 axis = new Vector2(-edge.Y, edge.X);
            axis.Normalize();

            if (axes.Contains(axis)) continue;

            axes.Add(axis);
        }

        return axes.ToArray();
    }

    private static Vector2 GetCenter(Vector2[] corners) => new Vector2
    (
        (corners[0].X + corners[1].X + corners[2].X + corners[3].X) / 4,
        (corners[0].Y + corners[1].Y + corners[2].Y + corners[3].Y) / 4
    );
}