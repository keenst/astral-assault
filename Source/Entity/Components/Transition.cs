using System;

namespace AstralAssault;

public readonly struct Transition
{
    public string ConditionName { get; }
    public float ConditionThreshold { get; }
    
    public int From { get; }
    public int To { get; }
    
    public int[] AnimationPath { get; }
    public Tuple<int, int> FromTo => new(From, To);

    public Transition(int from, int to, int[] animationPath, string conditionName, float conditionThreshold)
    {
        From = from;
        To = to;
        AnimationPath = animationPath;
        ConditionName = conditionName;
        ConditionThreshold = conditionThreshold;
    }
}