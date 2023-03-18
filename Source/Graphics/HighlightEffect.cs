namespace AstralAssault;

public struct HighlightEffect : IDrawTaskEffect
{
    public float Alpha { get; set; }
    
    public HighlightEffect(float alpha)
    {
        Alpha = alpha;
    }
}