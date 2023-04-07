using System;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public struct VelocityProperty : IParticleProperty
{
    private readonly float _angleRangeStart;
    private readonly float _angleRangeEnd;

    private readonly float _speedRangeStart;
    private readonly float _speedRangeEnd;

    public Vector2 Velocity { get; }

    public bool IsRange { get; }

    public VelocityProperty(float angleRangeStart, float angleRangeEnd, float speedRangeStart, float speedRangeEnd)
    {
        _angleRangeStart = angleRangeStart;
        _angleRangeEnd = angleRangeEnd;

        _speedRangeStart = speedRangeStart;
        _speedRangeEnd = speedRangeEnd;

        Velocity = Vector2.Zero;

        IsRange = true;
    }

    public VelocityProperty(float angle, float speed)
    {
        _angleRangeStart = angle;
        _angleRangeEnd = angle;

        _speedRangeStart = speed;
        _speedRangeEnd = speed;

        Vector2 normal = Vector2.UnitY.RotateVector(angle);

        Velocity = normal * speed;

        IsRange = false;
    }

    public Vector2 GetVelocity()
    {
        if (!IsRange) return Velocity;

        Random rnd = new();
        float multiplierAngle = rnd.NextSingle();
        float multiplierSpeed = rnd.NextSingle();

        float angle = _angleRangeStart + (_angleRangeEnd - _angleRangeStart) * multiplierAngle;
        float speed = _speedRangeStart + (_speedRangeEnd - _speedRangeStart) * multiplierSpeed;

        Vector2 normal = Vector2.UnitY.RotateVector(angle);

        return normal * speed;
    }
}