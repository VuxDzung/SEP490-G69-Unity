namespace SEP490G69.Battle
{
    using SEP490G69;
    using System.Collections.Generic;

    public static class CardEffectFactory
    {
        private static Dictionary<string, ICardSpecialEffect> effects =
            new Dictionary<string, ICardSpecialEffect>()
            {
                //{ "Execute", new ExecuteEffect() },
                { StatusEffectConstants.STATUS_EFFECT_ID_0017, new RegenerationEffect() },
                { StatusEffectConstants.STATUS_EFFECT_ID_0023, new ThornEffect() },
                { StatusEffectConstants.STATUS_EFFECT_ID_0024, new BerserkEffect() },
                //{ "AllOutStrike", new AllOutStrikeEffect() }
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