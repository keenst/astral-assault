using System;

namespace AstralAssault;

public static class ExtensionMethods
{
    public static int Mod(this int x, int y)
    {
        return (Math.Abs(x * y) + x) % y;
    }
}