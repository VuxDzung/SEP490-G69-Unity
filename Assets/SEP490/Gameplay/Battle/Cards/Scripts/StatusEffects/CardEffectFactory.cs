using SEP490G69;
using System.Collections.Generic;
using UnityEngine;

public static class CardEffectFactory 
{
    private static Dictionary<string, ICardSpecialEffect> effects =
        new Dictionary<string, ICardSpecialEffect>()
        {
            //{ "Execute", new ExecuteEffect() },
            //{ "TimeWarp", new TimeWarpEffect() },
            //{ "BerserkerSlash", new BerserkerSlashEffect() },
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
