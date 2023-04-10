using System;
using System.Collections.Generic;

namespace AstralAssault;

public static class ExtensionMethods
{
    public static int Mod(this int x, int y)
    {
        return (Math.Abs(x * y) + x) % y;
    }
    
    public static Tuple<TKey, TValue> ToTuple<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair)
    {
        return new Tuple<TKey, TValue>(keyValuePair.Key, keyValuePair.Value);
    }
}