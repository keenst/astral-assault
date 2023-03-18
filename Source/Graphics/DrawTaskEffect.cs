namespace AstralAssault;

public struct DrawTaskEffect
{
    public Effects Effect { get; }
    public float Alpha { get; }
    
    public DrawTaskEffect(Effects effect, float alpha)
    {
        Effect = effect;
        Alpha = alpha;
    }
    
    public static DrawTaskEffect None => new(Effects.None, 0);
}