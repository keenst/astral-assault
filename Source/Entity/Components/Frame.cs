#region
using System;
using Microsoft.Xna.Framework;
#endregion

namespace TheGameOfDoomHmmm.Source.Entity.Components;

public readonly struct Frame
{
    internal Rectangle[] Rotations { get; } = new Rectangle[4];
    internal Rectangle Source { get; }

    internal bool HasRotations
    {
        get => Rotations[0] != Rectangle.Empty;
    }

    internal int Time { get; }

    internal Frame(Rectangle e, Rectangle see, Rectangle se, Rectangle sse, int time = 0)
    {
        Rotations[0] = e;
        Rotations[1] = see;
        Rotations[2] = se;
        Rotations[3] = sse;

        Source = Rectangle.Empty;

        Time = time;
    }

    internal Frame(Rectangle source, int time = 0)
    {
        Array.Fill(Rotations, Rectangle.Empty);

        Source = source;

        Time = time;
    }
}