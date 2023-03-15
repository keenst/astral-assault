using System;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public struct Frame
{
    public Rectangle[] Rotations { get; } = new Rectangle[4];
    public Rectangle Source { get; }
    public int Time { get; }
    
    public Frame(Rectangle e, Rectangle see, Rectangle se, Rectangle sse, int time = 0)
    {
        Rotations[0] = e;
        Rotations[1] = see;
        Rotations[2] = se;
        Rotations[3] = sse;

        Source = Rectangle.Empty;
        
        Time = time;
    }
    
    public Frame(Rectangle source, int time = 0)
    {
        Array.Fill(Rotations, Rectangle.Empty);
        
        Source = source;
        
        Time = time;
    }
}