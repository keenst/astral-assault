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
    public readonly float Mass;
    
    public Collider(Entity parent, Rectangle rectangle, bool isSolid, float mass)
    {
        Parent = parent;
        Rectangle = rectangle;
        IsSolid = isSolid;
        _corners = GetCorners(Rectangle);
        _axes = GetAxes(_corners);
        Mass = mass;
    }
    
    public Collider(Entity parent, Rectangle rectangle)
    {
        Parent = parent;
        Rectangle = rectangle;
        IsSolid = false;
        _corners = GetCorners(Rectangle);
        _axes = GetAxes(_corners);
        Mass = 0;
    }
    
    public bool CollidesWith(
        Collider other,
        out Vector2 collisionNormal, 
        out float collisionDepth, 
        out Vector2 impulse)
    {
        collisionNormal = Vector2.Zero;
        collisionDepth = 0;
        impulse = Vector2.Zero;
        
        Vector2 minimumTranslationVector = CalculateMTV(Rectangle, other.Rectangle);

        if (minimumTranslationVector == Vector2.Zero) return false;

        collisionNormal = minimumTranslationVector.Normalized();
        collisionDepth = minimumTranslationVector.Length();
        
        Vector2 relativeVelocity = other.Parent.Velocity - Parent.Velocity;
        
        float velocityAlongNormal = Vector2.Dot(relativeVelocity, collisionNormal);
        
        if (velocityAlongNormal > 0) return false;
        
        float impulseMagnitude = -(1 + 0.5F) * velocityAlongNormal / (1 / Mass + 1 / other.Mass);
        impulse = impulseMagnitude * collisionNormal;

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

    private static Vector2 CalculateMTV(Rectangle rectA, Rectangle rectB)
    {
        Vector2[] cornersA = GetCorners(rectA);
        Vector2[] cornersB = GetCorners(rectB);

        Vector2[] axes = GetAxes(cornersA).Concat(cornersB).Distinct().ToArray();
        
        float minOverlap = float.MaxValue;
        Vector2 minimumTranslationVector = Vector2.Zero;
        
        foreach (Vector2 axis in axes)
        {
            float minA = float.MaxValue;
            float maxA = float.MinValue;
            float minB = float.MaxValue;
            float maxB = float.MinValue;

            foreach (Vector2 corner in cornersA)
            {
                float projection = Vector2.Dot(corner, axis);
                minA = MathF.Min(minA, projection);
                maxA = MathF.Max(maxA, projection);
            }

            foreach (Vector2 corner in cornersB)
            {
                float projection = Vector2.Dot(corner, axis);
                minB = MathF.Min(minB, projection);
                maxB = MathF.Max(maxB, projection);
            }

            float overlap = MathF.Min(maxA, maxB) - MathF.Max(minA, minB);
            if (overlap <= 0)
            {
                return Vector2.Zero;
            }
            
            if (overlap >= minOverlap) continue;
            
            minOverlap = overlap;
            minimumTranslationVector = axis * overlap;
        }

        return minimumTranslationVector;
    }
}