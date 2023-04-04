using System;
using System.Collections.Generic;
using System.Linq;

namespace AstralAssault
{
    public sealed class EffectContainer
    {
        internal List<IDrawTaskEffect> Effects { get; }

        internal EffectContainer(List<IDrawTaskEffect> effects) => this.Effects = effects;

        internal EffectContainer() => this.Effects = new();

        internal void SetEffect<TEffect, TParameter>(TParameter parameter)
        {
            if (!this.Effects.OfType<TEffect>().Any())
                this.Effects.Add((IDrawTaskEffect)Activator.CreateInstance(typeof(TEffect), parameter));
            else
            {
                int index = this.Effects.IndexOf((IDrawTaskEffect)this.Effects.OfType<TEffect>().First());
                this.Effects[index] = (IDrawTaskEffect)Activator.CreateInstance(typeof(TEffect), parameter);
            }
        }

        internal void RemoveEffect<TEffect>()
        {
            if (!this.Effects.OfType<TEffect>().Any()) return;

            int index = this.Effects.IndexOf((IDrawTaskEffect)this.Effects.OfType<TEffect>().First());
            this.Effects.RemoveAt(index);
        }
    }
}