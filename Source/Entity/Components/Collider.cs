using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public class Collider
{
    public readonly Entity Parent;
    public Rectangle Rectangle;
    public bool IsSolid;
    private Vector2[] _corners;
    private Vector2[] _axes;
    private readonly float _mass;
    
    public Collider(Entity parent, Rectangle rectangle, bool isSolid, float mass)
    {
        Parent = parent;
        Rectangle = rectangle;
        IsSolid = isSolid;
        _corners = GetCorners(Rectangle);
        _axes = GetAxes(_corners);
        _mass = mass;
    }
    
    public Collider(Entity parent, Rectangle rectangle)
    {
        Parent = parent;
        Rectangle = rectangle;
        IsSolid = false;
        _corners = GetCorners(Rectangle);
        _axes = GetAxes(_corners);
        _mass = 0;
    }
    
    public bool CollidesWith(
        Collider other, 
        float deltaTime,
        out Vector2 initialImpulseThis, 
        out Vector2 initialImpulseOther,
        out Vector2 totalImpulseThis,
        out Vector2 totalImpulseOther)
    {
        List<Vector2> axes = new();
        axes.AddRange(_axes);
        axes.AddRange(other._axes);
        axes = axes.Distinct().ToList();

        float minOverlap = float.MaxValue;

        Vector2 centerThis = GetCenter(_corners);
        Vector2 centerOther = GetCenter(other._corners);

        Vector2 relativeVelocity = centerOther - centerThis;

        initialImpulseThis = Vector2.Zero;
        initialImpulseOther = Vector2.Zero;
        totalImpulseThis = Vector2.Zero;
        totalImpulseOther = Vector2.Zero;

        foreach (Vector2 axis in axes)
        {
            float minThis   = float.MaxValue;
            float maxThis   = float.MinValue;
            float minOther  = float.MaxValue;
            float maxOther  = float.MinValue;

            foreach (Vector2 corner in _corners)
            {
                float projection = Vector2.Dot(corner, axis);

                if (projection < minThis)
                    minThis = projection;

                if (projection > maxThis)
                    maxThis = projection;
            }

            foreach (Vector2 corner in other._corners)
            {
                float projection = Vector2.Dot(corner, axis);

                if (projection < minOther)
                    minOther = projection;

                if (projection > maxOther)
                    maxOther = projection;
            }

            float overlap = Math.Min(maxThis, maxOther) - Math.Max(minThis, minOther);
            if (overlap < 0)
            {
                return false;
            }

            if (overlap >= minOverlap) continue;
            
            minOverlap = overlap;

            Vector2 direction = axis;

            if (Vector2.Dot(centerOther - centerThis, direction) < 0)
                direction = -direction;
            
            initialImpulseThis = -direction * (other._mass / (other._mass + _mass)) * overlap;
            initialImpulseOther = direction * (_mass / (other._mass + _mass)) * overlap;
        }
        
        totalImpulseThis = initialImpulseThis;
        totalImpulseOther = initialImpulseOther;

        Vector2 newRelativeVelocity = centerOther - centerThis;
        Vector2 impulse = (newRelativeVelocity - relativeVelocity) * (_mass * other._mass) / (_mass + other._mass);
        
        Vector2 impulseThis = -impulse * (other._mass / (_mass + other._mass));
        Vector2 impulseOther = impulse * (_mass  / (_mass + other._mass));
        
        totalImpulseThis += impulseThis;
        totalImpulseOther += impulseOther;
        return true;
    }

    public void SetPosition(Point position)
    {
        Rectangle.X = position.X - Rectangle.Width / 2;
        Rectangle.Y = position.Y - Rectangle.Height / 2;

        _corners = GetCorners(Rectangle);
        _axes = GetAxes(_corners);
    }

    private static Vector2[] GetCorners(Rectangle rect)
    {
        Vector2[] corners = new Vector2[4];
        corners[0] = new Vector2(rect.Left,  rect.Top);
        corners[1] = new Vector2(rect.Right, rect.Top);
        corners[2] = new Vector2(rect.Right, rect.Bottom);
        corners[3] = new Vector2(rect.Left,  rect.Bottom);
        return corners;
    }

    private static Vector2[] GetAxes(Vector2[] corners)
    {
        List<Vector2> axes = new();

        for (int i = 0; i < corners.Length; i++)
        {
            Vector2 edge = corners[(i + 1) % corners.Length] - corners[i];
            Vector2 axis = new(-edge.Y, edge.X);
            axis.Normalize();

            if (axes.Contains(axis)) continue;
            
            axes.Add(axis);
        }

        return axes.ToArray();
    }
    
    private static Vector2 GetCenter(Vector2[] corners)
    {
        return new Vector2(
            (corners[0].X + corners[1].X + corners[2].X + corners[3].X) / 4,
            (corners[0].Y + corners[1].Y + corners[2].Y + corners[3].Y) / 4);
    }
}