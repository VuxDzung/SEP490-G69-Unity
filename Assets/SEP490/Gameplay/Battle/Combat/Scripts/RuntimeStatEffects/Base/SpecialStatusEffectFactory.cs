namespace SEP490G69.Battle
{
    using SEP490G69;
    using System.Collections.Generic;

    public static class SpecialStatusEffectFactory
    {
        private static Dictionary<string, ICardSpecialEffect> effects =
            new Dictionary<string, ICardSpecialEffect>()
            {
                { StatusEffectConstants.STATUS_EFFECT_ID_0020, new SkipNextActionEffect() },
                { StatusEffectConstants.STATUS_EFFECT_ID_0026, new ReduceStaminaGainEffect() },
            };

        public static ICardSpecialEffect GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            effects.TryGetValue(id, out var effect);

            return effect;
        }
    }
}