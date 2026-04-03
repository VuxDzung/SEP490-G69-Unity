namespace SEP490G69.Battle.Combat
{
    using SEP490G69;
    using System.Collections.Generic;

    public static class SpecialEffectFactory
    {
        private static Dictionary<string, ISpecialStatusEffect> effects =
            new Dictionary<string, ISpecialStatusEffect>()
            {
                { StatusEffectConstants.STATUS_EFFECT_ID_0006, new StunEffect() },
                { StatusEffectConstants.STATUS_EFFECT_ID_0009, new DecayEffect() },
                { StatusEffectConstants.STATUS_EFFECT_ID_0010, new BleedEffect() },
                { StatusEffectConstants.STATUS_EFFECT_ID_0021, new PersistentEffect() },
                { StatusEffectConstants.STATUS_EFFECT_ID_0020, new RageEffect() },
                { StatusEffectConstants.STATUS_EFFECT_ID_0022, new DoomEffect() },
            };

        public static ISpecialStatusEffect GetById(string id)
        {
            if (string.IsNullOrEmpty(id) || effects == null || effects.Count == 0)
            {
                return null;
            }

            effects.TryGetValue(id, out var effect);

            return effect;
        }
    }
}