namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using UnityEngine;
    using UnityEngine.Purchasing;

    public static class DamageCalculator
    {
        public static float Calculate(BaseBattleCharacterController attacker, CardSO card)
        {
            float baseDamage = card.BaseValue;

            float stat = attacker.CurrentDataHolder
                .GetStatus(card.ModifyStatType);
            float result = baseDamage;

            switch (card.ModifyOp)
            {
                case EOperator.PercentAdd:
                    result += stat * card.ModifierValue;
                    break;

                case EOperator.FlatAdd:
                    result += card.ModifierValue;
                    break;
            }

            return Mathf.Max(0, result);
        }

        #region Archive
        //private static float CalculatePhysical(
        //    BaseBattleCharacterController attacker,
        //    BaseBattleCharacterController defender,
        //    float baseDamage)
        //{
        //    float power = attacker.CurrentDataHolder.GetPower();
        //    float defense = defender.CurrentDataHolder.GetDef();

        //    float raw = baseDamage + power;

        //    // diminishing defense
        //    float damageMultiplier = 100f / (100f + defense);

        //    return raw * damageMultiplier;
        //}

        //private static float CalculateMagical(
        //    BaseBattleCharacterController attacker,
        //    BaseBattleCharacterController defender,
        //    float baseDamage)
        //{
        //    float intelligence = attacker.CurrentDataHolder.GetINT();
        //    float defense = defender.CurrentDataHolder.GetDef();

        //    float raw = baseDamage + intelligence * 1.2f;

        //    float damageMultiplier = 100f / (100f + defense);

        //    return raw * damageMultiplier;
        //}
        #endregion
    }
}