using System;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public struct VelocityProperty : IParticleProperty
{
    private readonly float m_angleRangeStart;
    private readonly float m_angleRangeEnd;

    private readonly float m_speedRangeStart;
    private readonly float m_speedRangeEnd;

    public Vector2 Velocity { get; }

    public bool IsRange { get; }

    public VelocityProperty(float angleRangeStart, float angleRangeEnd, float speedRangeStart, float speedRangeEnd)
    {
        m_angleRangeStart = angleRangeStart;
        m_angleRangeEnd = angleRangeEnd;

        m_speedRangeStart = speedRangeStart;
        m_speedRangeEnd = speedRangeEnd;

        Velocity = Vector2.Zero;

        IsRange = true;
    }

    public VelocityProperty(float angle, float speed)
    {
        m_angleRangeStart = angle;
        m_angleRangeEnd = angle;

        m_speedRangeStart = speed;
        m_speedRangeEnd = speed;

        Vector2 normal = Vector2.UnitY.RotateVector(angle);

        Velocity = normal * speed;

        IsRange = false;
    }

    public Vector2 GetVelocity()
    {
        if (!IsRange) return Velocity;

        Random rnd = new Random();
        float multiplierAngle = rnd.NextSingle();
        float multiplierSpeed = rnd.NextSingle();

        float angle = m_angleRangeStart + (m_angleRangeEnd - m_angleRangeStart) * multiplierAngle;
        float speed = m_speedRangeStart + (m_speedRangeEnd - m_speedRangeStart) * multiplierSpeed;

        Vector2 normal = Vector2.UnitY.RotateVector(angle);

        return normal * speed;
    }
}