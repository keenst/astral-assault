using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AstralAssault.Background;

public class BackgroundRenderer
{
    private enum Side
    {
        Top,
        Right,
        Bottom,
        Left
    }
    
    private const int ScreenWidth = Game1.TargetWidth;
    private const int ScreenHeight = Game1.TargetHeight;
    private const int MaxNebulae = 10;

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
        
        for (int i = 0; i < MaxNebulae; i++)
        {
            SpawnInitialNebula();
        }
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
        
        if (_nebulae.Count < MaxNebulae)
        {
            SpawnNebula();
        }
    }

    private Side[] GetSpawnSides()
    {
        Side[] sides = new Side[2];

        if (_parallaxOffset.X > 0)
        {
            sides[0] = Side.Right;
        }
        else
        {
            sides[0] = Side.Left;
        }

        if (_parallaxOffset.Y > 0)
        {
            sides[1] = Side.Bottom;
        }
        else
        {
            sides[1] = Side.Top;
        }

        return sides;
    }
    
    private Point GetSpawnPosition(Side side)
    {
        for (int i = 0; i < 10; i++)
        {
            int x;
            int y;

            switch (side)
            {
                case Side.Top:
                    x = _rnd.Next(0, ScreenWidth);
                    y = -31;
                    break;
                case Side.Right:
                    x = ScreenWidth + 31;
                    y = _rnd.Next(0, ScreenHeight);
                    break;
                case Side.Bottom:
                    x = _rnd.Next(0, ScreenWidth);
                    y = ScreenHeight + 31;
                    break;
                case Side.Left:
                    x = -31;
                    y = _rnd.Next(0, ScreenHeight);
                    break;
                default:
                    x = 0;
                    y = 0;
                    break;
            }

            Point position = new(x, y);

            if (CanSpawnNebulaOnPoint(position))
            {
                return position;
            }
        }

        return new Point(0, 0);
    }

    private void GetSideRanges(Side side, out int minX, out int maxX, out int minY, out int maxY)
    {
        switch (side)
        {
            case Side.Top:
                minX = -31;
                maxX = ScreenWidth + 31;
                minY = -31;
                maxY = -31;
                break;
            case Side.Right:
                minX = ScreenWidth + 31;
                maxX = ScreenWidth + 31;
                minY = -31;
                maxY = ScreenHeight + 31;
                break;
            case Side.Bottom:
                minX = -31;
                maxX = ScreenWidth + 31;
                minY = ScreenHeight + 31;
                maxY = ScreenHeight + 31;
                break;
            case Side.Left:
                minX = -31;
                maxX = -31;
                minY = -31;
                maxY = ScreenHeight + 31;
                break;
            default:
                minX = 0;
                maxX = 0;
                minY = 0;
                maxY = 0;
                break;
        }
    }

    private void SpawnNebula()
    {
        Side[] sides = GetSpawnSides();
        Side side = sides[_rnd.Next(2)];
        Point position = GetSpawnPosition(side);

        _nebulae.Add(new Nebula(GetVirtualPosition(position)));
    }

    private void SpawnInitialNebula()
    {
        int x;
        int y;
        
        bool positionIsTaken;
        do
        {
            positionIsTaken = false;
            
            x = _rnd.Next(ScreenWidth);
            y = _rnd.Next(ScreenHeight);

            if (!CanSpawnNebulaOnPoint(new Point(x, y)))
            {
                positionIsTaken = true;
            }
        } while (positionIsTaken);
        
        _nebulae.Add(new Nebula(new Point(x, y)));
    }

    private bool CanSpawnNebulaOnPoint(Point position)
    {
        Point virtualPosition = GetVirtualPosition(new Point(position.X, position.Y));
        Rectangle checkRange = new(virtualPosition.X - 64, virtualPosition.Y - 64, 128, 128);

        foreach (Nebula nebula in _nebulae)
        {
            if (nebula.Rectangle.Intersects(checkRange))
            {
                return false;
            }
        }

        return true;
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