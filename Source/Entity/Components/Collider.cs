using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace astral_assault;

public class Collider
{
    public Entity Parent;
    public Rectangle Rectangle;
    public Vector2[] Corners;
    public Vector2[] Axes;

    public Collider(Entity parent, Rectangle rectangle)
    {
        Parent = parent;
        Rectangle = rectangle;
        Corners = GetCorners(Rectangle);
        Axes = GetAxes(Corners);
    }
    
    public bool CollidesWith(Collider other)
    {
        List<Vector2> axes = new();
        axes.AddRange(Axes);
        axes.AddRange(other.Axes);
        axes = axes.Distinct().ToList();

        foreach (Vector2 axis in axes)
        {
            float minThis   = float.MaxValue;
            float maxThis   = float.MinValue;
            float minOther  = float.MaxValue;
            float maxOther  = float.MinValue;

            foreach (Vector2 corner in Corners)
            {
                float projection = Vector2.Dot(corner, axis);

                if (projection < minThis)
                    minThis = projection;

                if (projection > maxThis)
                    maxThis = projection;
            }

            foreach (Vector2 corner in other.Corners)
            {
                float projection = Vector2.Dot(corner, axis);

                if (projection < minOther)
                    minOther = projection;

                if (projection > maxOther)
                    maxOther = projection;
            }

            if (maxThis < minOther || maxOther < minThis)
                return false;
        }
        
        return true;
    }

    public void SetPosition(Point position)
    {
        Rectangle.X = position.X - Rectangle.Width / 2;
        Rectangle.Y = position.Y - Rectangle.Height / 2;

        Corners = GetCorners(Rectangle);
        Axes = GetAxes(Corners);
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
}