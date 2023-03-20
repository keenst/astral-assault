namespace AstralAssault;

public struct ColorChangeProperty : IParticleProperty
{
    public Palette.Colors[] Colors { get; }
    public int TimeBetweenColorsMS { get; }

    public ColorChangeProperty(Palette.Colors[] colors, int timeBetweenColorsMS)
    {
        Colors = colors;
        TimeBetweenColorsMS = timeBetweenColorsMS;
    }
}