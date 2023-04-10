#region
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace AstralAssault;

public class Collider
{
    private readonly float m_mass;
    public readonly Entity Parent;
    public bool IsSolid;
    private Vector2[] m_axes;
    private Vector2[] m_corners;
    public Rectangle Rectangle;

    public Collider(Entity parent, Rectangle rectangle, bool isSolid, float mass)
    {
        Parent = parent;
        Rectangle = rectangle;
        IsSolid = isSolid;
        m_corners = GetCorners(Rectangle);
        m_axes = GetAxes(m_corners);
        m_mass = mass;
    }

    public Collider(Entity parent, Rectangle rectangle)
    {
        Parent = parent;
        Rectangle = rectangle;
        IsSolid = false;
        m_corners = GetCorners(Rectangle);
        m_axes = GetAxes(m_corners);
        m_mass = 0;
    }

    public bool CollidesWith(
        Collider other,
        float deltaTime,
        out Vector2 initialImpulseThis,
        out Vector2 initialImpulseOther,
        out Vector2 totalImpulseThis,
        out Vector2 totalImpulseOther)
    {
        List<Vector2> axes = new List<Vector2>();
        axes.AddRange(m_axes);
        axes.AddRange(other.m_axes);

        for (int i = 0; i < axes.Count; i++)
        {
            for (int j = i + 1; j < axes.Count; j++)
            {
                if (axes[i] == axes[j])
                {
                    axes.RemoveAt(j);
                    j--;
                }
            }
        }

        float minOverlap = float.MaxValue;

        Vector2 centerThis = GetCenter(m_corners);
        Vector2 centerOther = GetCenter(other.m_corners);

        Vector2 relativeVelocity = centerOther - centerThis;

        initialImpulseThis = Vector2.Zero;
        initialImpulseOther = Vector2.Zero;
        totalImpulseThis = Vector2.Zero;
        totalImpulseOther = Vector2.Zero;

        foreach (Vector2 axis in axes)
        {
            float minThis = float.MaxValue;
            float maxThis = float.MinValue;
            float minOther = float.MaxValue;
            float maxOther = float.MinValue;

            foreach (Vector2 corner in m_corners)
            {
                float projection = Vector2.Dot(corner, axis);

                if (projection < minThis)
                    minThis = projection;

                if (projection > maxThis)
                    maxThis = projection;
            }

            foreach (Vector2 corner in other.m_corners)
            {
                float projection = Vector2.Dot(corner, axis);

                if (projection < minOther)
                    minOther = projection;

                if (projection > maxOther)
                    maxOther = projection;
            }

            float overlap = Math.Min(maxThis, maxOther) - Math.Max(minThis, minOther);

            if (overlap < 0) return false;

            if (overlap >= minOverlap) continue;

            minOverlap = overlap;

            Vector2 direction = axis;

            if (Vector2.Dot(centerOther - centerThis, direction) < 0)
                direction = -direction;

            initialImpulseThis = -direction * (other.m_mass / (other.m_mass + m_mass)) * overlap;
            initialImpulseOther = direction * (m_mass / (other.m_mass + m_mass)) * overlap;
        }

        totalImpulseThis = initialImpulseThis;
        totalImpulseOther = initialImpulseOther;

        Vector2 newRelativeVelocity = centerOther - centerThis;
        Vector2 impulse = (newRelativeVelocity - relativeVelocity) * (m_mass * other.m_mass) / (m_mass + other.m_mass);

        Vector2 impulseThis = -impulse * (other.m_mass / (m_mass + other.m_mass));
        Vector2 impulseOther = impulse * (m_mass / (m_mass + other.m_mass));

        totalImpulseThis += impulseThis;
        totalImpulseOther += impulseOther;

        return true;
    }

    public void SetPosition(Point position)
    {
        Rectangle.X = position.X - Rectangle.Width / 2;
        Rectangle.Y = position.Y - Rectangle.Height / 2;

        m_corners = GetCorners(Rectangle);
        m_axes = GetAxes(m_corners);
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