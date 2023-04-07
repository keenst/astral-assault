using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace AstralAssault;

public static class Palette
{
    public enum Colors
    {
        Grey1,
        Grey2,
        Grey3,
        Grey4,
        Grey5,
        Grey6,
        Grey7,
        Grey8,
        Grey9,
        Blue1,
        Blue2,
        Blue3,
        Blue4,
        Blue5,
        Blue6,
        Blue7,
        Blue8,
        Blue9,
        Green1,
        Green2,
        Green3,
        Green4,
        Green5,
        Green6,
        Green7,
        Green8,
        Green9,
        Beige1,
        Beige2,
        Beige3,
        Beige4,
        Beige5,
        Beige6,
        Beige7,
        Beige8,
        Beige9,
        Yellow1,
        Yellow2,
        Yellow3,
        Yellow4,
        Yellow5,
        Yellow6,
        Yellow7,
        Yellow8,
        Yellow9,
        Red1,
        Red2,
        Red3,
        Red4,
        Red5,
        Red6,
        Red7,
        Red8,
        Red9,
        Purple1,
        Purple2,
        Purple3,
        Purple4,
        Purple5,
        Purple6,
        Purple7,
        Purple8,
        Purple9,
        Black
    }

    private struct PaletteColor
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public PaletteColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
    }

    private static readonly List<PaletteColor> PaletteColors = new();

    public static void Init()
    {
        string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        string[] lines = File.ReadAllLines(path + "/Content/gora64.hex");

        foreach (string line in lines)
        {
            byte r = byte.Parse(line[..2], System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(line[2..4], System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(line[4..6], System.Globalization.NumberStyles.HexNumber);

            Palette.PaletteColors.Add(new(r, g, b));
        }
    }

    public static Color GetColor(Colors name, float alpha = 1)
    {
        PaletteColor color = Palette.PaletteColors[(int)name];

        return new(color.R, color.G, color.B, (byte)(alpha * 255));
    }

    public static Vector4 GetColorVector(Colors name)
    {
        PaletteColor color = Palette.PaletteColors[(int)name];

        return new(color.R / 255F, color.G / 255F, color.B / 255F, 1);
    }
}