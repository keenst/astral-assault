using System;
using System.Collections.Generic;
using System.Linq;
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

    [Flags]
    private enum Direction
    {
        Horizontal,
        Vertical
    }

    private readonly Dictionary<Side, int> _baseSpawnSideWeights = new()
    {
        { Side.Top, ScreenWidth },
        { Side.Right, ScreenHeight },
        { Side.Bottom, ScreenWidth },
        { Side.Left, ScreenHeight }
    };

    private const int ScreenWidth = Game1.TargetWidth;
    private const int ScreenHeight = Game1.TargetHeight;
    
    private const int MaxNebulae = 10;
    private const int MaxStars = 150;

    private const float ZMin = -1;
    private const float ZMax = 1;
    private const float NebulaZ = 0;
    
    private const float ParallaxSpeedMin = 0.2F;
    private const float ParallaxSpeedMax = 2;

    private readonly Vector2 _parallaxOffset;
    private readonly Texture2D _nebulaTexture;
    private readonly Texture2D _starTexture;
    private readonly Random _rnd = new();
    
    private readonly List<Nebula> _nebulae = new();
    private readonly List<Vector3> _stars = new();
    
    private Vector2 _currentOffset;

    public BackgroundRenderer(Game root)
    {
        _nebulaTexture = AssetManager.Load<Texture2D>("Nebula1");
        _starTexture = CreateStarTexture(root);
        
        float xOffset = RandomFloat(-1, 1);
        float yOffset = RandomFloat(-1, 1);
        _parallaxOffset = new Vector2(xOffset, yOffset);
        _parallaxOffset.Normalize();
        _parallaxOffset *= 3;
        
        for (int i = 0; i < MaxNebulae; i++)
        {
            SpawnInitialNebula();
        }
        
        for (int i = 0; i < MaxStars; i++)
        {
            SpawnInitialStar();
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

        foreach (Vector3 star in _stars)
        {
            DrawTask starTask = new(
                _starTexture,
                GetScreenPosition(star).ToVector2(),
                0,
                LayerDepth.Background,
                new List<IDrawTaskEffect>());
            
            drawTasks.Add(starTask);
        }

        return drawTasks;
    }
    
    public void Update(float deltaTime)
    {
        _currentOffset += _parallaxOffset * deltaTime;
        
        HandleRemoving();
        HandleSpawning();
    }

    private void HandleRemoving()
    {
        foreach (Nebula nebula in _nebulae)
        {
            Vector3 virtualPosition = 
                nebula.Position - new Vector3(_nebulaTexture.Bounds.Size.ToVector2() / 2, nebula.Position.Z);
            
            Point position = GetScreenPosition(virtualPosition);
            Rectangle nebulaRectangle = new(position, _nebulaTexture.Bounds.Size);
            
            if (nebulaRectangle.Intersects(new Rectangle(Point.Zero, new Point(ScreenWidth, ScreenHeight)))) continue;
            
            _nebulae.Remove(nebula);
            break;
        }

        foreach (Vector3 star in _stars)
        {
            Point position = GetScreenPosition(star);

            if (position.X is >= 0 and <= ScreenWidth && 
                position.Y is >= 0 and <= ScreenHeight) 
                continue;
            
            _stars.Remove(star);
            break;
        }
    }
    
    private void HandleSpawning()
    {
        if (_nebulae.Count < MaxNebulae)
        {
            SpawnNebula();
        }

        if (_stars.Count < MaxStars)
        {
            SpawnStar();
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
    
    private bool GetSpawnPosition(Side side, int width, int height, float z, out Vector3 position, int padding = 0)
    {
        GetSideRanges(side, width, height, out int minX, out int maxX, out int minY, out int maxY);
        Direction direction = GetDirectionFromSide(side) ^ Direction.Horizontal;

        HashSet<int> checkedNumbers = new();
        
        int rangeLength = direction == Direction.Vertical ? maxX - minX : maxY - minY;
        
        do
        {
            int randomNum = direction == Direction.Vertical 
                ? _rnd.Next(minX, maxX) 
                : _rnd.Next(minY, maxY);
            
            if (checkedNumbers.Contains(randomNum)) continue;

            checkedNumbers.Add(randomNum);
            
            position = direction == Direction.Vertical
                ? new Vector3(randomNum, minY, z) 
                : new Vector3(minX, randomNum, z);

            if (padding == 0) return true;

            if (CanSpawnOnPoint(position, width + padding, height + padding)) return true;
        } while (checkedNumbers.Count < rangeLength);
        
        position = Vector3.Zero;
        return false;
    }

    private static void GetSideRanges(
        Side side, 
        int width, 
        int height, 
        out int minX, 
        out int maxX, 
        out int minY, 
        out int maxY)
    {
        switch (side)
        {
            case Side.Top:
                minX = 0;
                maxX = ScreenWidth;
                minY = -(height / 2 - 1);
                maxY = minY;
                break;
            case Side.Right:
                minX = ScreenWidth + (width / 2 - 1);
                maxX = minX;
                minY = 0;
                maxY = ScreenHeight;
                break;
            case Side.Bottom:
                minX = 0;
                maxX = ScreenWidth;
                minY = ScreenHeight + (height / 2 - 1);
                maxY = minY;
                break;
            case Side.Left:
                minX = -(width / 2 - 1);
                maxX = minX;
                minY = 0;
                maxY = ScreenHeight;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(side), side, null);
        }
    }

    private Side PickSpawnSide(Side[] sides)
    {
        Dictionary<Side, float> sideWeights = new();
        
        foreach (Side side in sides)
        {
            Direction direction = GetDirectionFromSide(side);

            float multiplier = direction switch
            {
                Direction.Horizontal => MathF.Abs(_parallaxOffset.X),
                Direction.Vertical => MathF.Abs(_parallaxOffset.Y),
                _ => throw new ArgumentOutOfRangeException()
            };

            sideWeights.Add(side, _baseSpawnSideWeights[side] * multiplier);
        }

        float sumOfWeights = sideWeights.Values.Sum();
        
        float random = RandomFloat(0, sumOfWeights);
        foreach (Side side in sides)
        {
            if (random < sideWeights[side])
            {
                return side;
            }
            random -= sideWeights[side];
        }
        
        throw new Exception("Was unable to find spawn side");
    }

    private void SpawnStar()
    {
        Side[] sides = GetSpawnSides();
        Side side = PickSpawnSide(sides);

        float z = RandomFloat(ZMin, ZMax);
        
        if (!GetSpawnPosition(side, 1, 1, z, out Vector3 position)) return;

        Vector3 virtualPosition = GetVirtualPosition(position);
        
        _stars.Add(virtualPosition);
    }
    
    private void SpawnInitialStar()
    {
        int x = _rnd.Next(ScreenWidth);
        int y = _rnd.Next(ScreenHeight);
        float z = RandomFloat(ZMin, ZMax);
        
        _stars.Add(new Vector3(x, y, z));
    }

    private void SpawnNebula()
    {
        Side[] sides = GetSpawnSides();
        Side side = PickSpawnSide(sides);

        if (!GetSpawnPosition(side, 64, 64, NebulaZ, out Vector3 position, 64)) return;

        _nebulae.Add(new Nebula(GetVirtualPosition(position)));
    }

    private void SpawnInitialNebula()
    {
        Vector3 position = new(0, 0, NebulaZ);

        bool positionIsTaken;
        do
        {
            positionIsTaken = false;
            
            position.X = _rnd.Next(ScreenWidth);
            position.Y = _rnd.Next(ScreenHeight);

            if (!CanSpawnOnPoint(position, 128, 128))
            {
                positionIsTaken = true;
            }
        } while (positionIsTaken);
        
        _nebulae.Add(new Nebula(position));
    }

    private bool CanSpawnOnPoint(Vector3 position, int checkWidth, int checkHeight)
    {
        Vector3 virtualPosition = GetVirtualPosition(position);
        Rectangle checkRange = new(
            (int)virtualPosition.X - checkWidth / 2, 
            (int)virtualPosition.Y - checkHeight / 2, 
            checkWidth, 
            checkHeight);

        foreach (Nebula nebula in _nebulae)
        {
            if (nebula.Rectangle.Intersects(checkRange))
            {
                return false;
            }
        }

        return true;
    }

    private Point GetScreenPosition(Vector3 position)
    {
        int x = (int)(position.X - _currentOffset.X * GetParallaxSpeedMultiplier(position.Z));
        int y = (int)(position.Y - _currentOffset.Y * GetParallaxSpeedMultiplier(position.Z));
        return new Point(x, y);
    }

    private Vector3 GetVirtualPosition(Vector3 screenPosition)
    {
        int x = (int)(screenPosition.X + _currentOffset.X * GetParallaxSpeedMultiplier(screenPosition.Z));
        int y = (int)(screenPosition.Y + _currentOffset.Y * GetParallaxSpeedMultiplier(screenPosition.Z));
        return new Vector3(x, y, screenPosition.Z);
    }

    private static Texture2D CreateStarTexture(Game root)
    {
        Color[] data = { Color.White };
        Texture2D texture = new(root.GraphicsDevice, 1, 1);
        texture.SetData(data);
        return texture;
    }

    private static Direction GetDirectionFromSide(Side side)
    {
        return side switch
        {
            Side.Top => Direction.Vertical,
            Side.Right => Direction.Horizontal,
            Side.Bottom => Direction.Vertical,
            Side.Left => Direction.Horizontal,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
    }

    private static float GetParallaxSpeedMultiplier(float z)
    {
        z = (z + 1) / 2;
        return z * (ParallaxSpeedMax - ParallaxSpeedMin) + ParallaxSpeedMin;
    }
    
    private float RandomFloat(float minValue, float maxValue)
    {
        return _rnd.NextSingle() * (maxValue - minValue) + minValue;
    }
}