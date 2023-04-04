using System;
using System.Collections.Generic;
using System.Linq;

namespace AstralAssault
{
    public sealed class EffectContainer
    {
        internal List<IDrawTaskEffect> Effects { get; }

        internal EffectContainer(List<IDrawTaskEffect> effects) => Effects = effects;

        internal EffectContainer() => Effects = new();

        internal void SetEffect<TEffect, TParameter>(TParameter parameter)
        {
            if (!Effects.OfType<TEffect>().Any())
                Effects.Add((IDrawTaskEffect)Activator.CreateInstance(typeof(TEffect), parameter));
            else
            {
                int index = Effects.IndexOf((IDrawTaskEffect)Effects.OfType<TEffect>().First());
                Effects[index] = (IDrawTaskEffect)Activator.CreateInstance(typeof(TEffect), parameter);
            }
        }

        internal void RemoveEffect<TEffect>()
        {
            if (!Effects.OfType<TEffect>().Any()) return;

            int index = Effects.IndexOf((IDrawTaskEffect)Effects.OfType<TEffect>().First());
            Effects.RemoveAt(index);
        }
    }
}