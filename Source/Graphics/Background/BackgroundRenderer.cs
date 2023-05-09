using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault.Background;

public class BackgroundRenderer
{
    private const int ScreenWidth = Game1.TargetWidth;
    private const int ScreenHeight = Game1.TargetHeight;

    private readonly Vector2 _parallaxOffset;
    private readonly Texture2D _nebulaTexture;
    private readonly List<Nebula> _nebulae = new();
    private readonly Random _rnd = new();
    
    private Vector2 _currentOffset;

    public BackgroundRenderer()
    {
        _nebulaTexture = AssetManager.Load<Texture2D>("Nebula1");
        
        float xOffset = RandomFloat(-1, 1);
        float yOffset = RandomFloat(-1, 1);
        _parallaxOffset = new Vector2(xOffset, yOffset);
        _parallaxOffset.Normalize();
        _parallaxOffset *= 3;
    }

    public List<DrawTask> GetDrawTasks()
    {
        List<DrawTask> drawTasks = new();

        foreach (Nebula nebula in _nebulae)
        {
            DrawTask nebulaTask = new(
                _nebulaTexture, 
                GetScreenPosition(nebula.Position).ToVector2(), 
                0, 
                LayerDepth.Background, 
                new List<IDrawTaskEffect>());
            
            drawTasks.Add(nebulaTask);
        }

        return drawTasks;
    }
    
    public void Update(float deltaTime)
    {
        _currentOffset += _parallaxOffset * deltaTime;
        
        foreach (Nebula nebula in _nebulae)
        {
            Point position = GetScreenPosition(nebula.Position - _nebulaTexture.Bounds.Size / new Point(2, 2));
            Rectangle nebulaRectangle = new(position, _nebulaTexture.Bounds.Size);
            
            if (nebulaRectangle.Intersects(new Rectangle(Point.Zero, new Point(ScreenWidth, ScreenHeight)))) continue;
            
            _nebulae.Remove(nebula);
            break;
        }
        
        if (_nebulae.Count < 2)
        {
            int[] sides = new int[2];

            if (_parallaxOffset.X > 0)
            {
                sides[0] = 3;
            }
            else
            {
                sides[0] = 2;
            }

            if (_parallaxOffset.Y > 0)
            {
                sides[1] = 0;
            }
            else
            {
                sides[1] = 3;
            }

            int side = sides[_rnd.Next(2)];

            int minY;
            int maxY;
            int minX;
            int maxX;
            
            Point position = new(_rnd.Next(minX, maxX), _rnd.Next(minY, maxY));

            _nebulae.Add(new Nebula(GetVirtualPosition(position)));
        }
    }
    
    private Point GetScreenPosition(Point position)
    {
        int x = position.X - (int)_currentOffset.X;
        int y = position.Y - (int)_currentOffset.Y;
        return new Point(x, y);
    }

    private Point GetVirtualPosition(Point screenPosition)
    {
        int x = screenPosition.X + (int)_currentOffset.X;
        int y = screenPosition.Y + (int)_currentOffset.Y;
        return new Point(x, y);
    }

    private float RandomFloat(float minValue, float maxValue)
    {
        return _rnd.NextSingle() * (maxValue - minValue) + minValue;
    }
}