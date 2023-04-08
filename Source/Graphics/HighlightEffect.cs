namespace AstralAssault;

public struct HighlightEffect : IDrawTaskEffect
{
    public float Alpha { get; private set; }

    public HighlightEffect(float alpha)
    {
        Alpha = alpha;
    }

    public void SetAlpha(float alpha)
    {
        Alpha = alpha;
    }
}