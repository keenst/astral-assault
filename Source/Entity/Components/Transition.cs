using System;

namespace AstralAssault;

public struct Transition
{
    private int _from;
    private int _to;

    public int[] AnimationPath { get; }
    public Tuple<int, int> FromTo => new(_from, _to);

    public Transition(int from, int to, int[] animationPath)
    {
        _from = from;
        _to = to;
        AnimationPath = animationPath;
    }
}