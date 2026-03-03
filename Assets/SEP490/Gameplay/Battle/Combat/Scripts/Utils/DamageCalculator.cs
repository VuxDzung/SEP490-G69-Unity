namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public static class DamageCalculator
    {
        public static float Calculate(
            BaseBattleCharacterController attacker,
            BaseBattleCharacterController defender,
            float baseDamage,
            EDamageType damageType)
        {
            float finalDamage = baseDamage;

            switch (damageType)
            {
                case EDamageType.Physical:
                    finalDamage = CalculatePhysical(attacker, defender, baseDamage);
                    break;

                case EDamageType.Magic:
                    finalDamage = CalculateMagical(attacker, defender, baseDamage);
                    break;

                case EDamageType.True:
                    finalDamage = baseDamage;
                    break;
            }

            foreach (var status in attacker.ActiveStatuses)
            {
                finalDamage = status.ModifyDealableDmg(finalDamage);
            }

            return Mathf.Max(0, finalDamage);
        }

        private static float CalculatePhysical(
            BaseBattleCharacterController attacker,
            BaseBattleCharacterController defender,
            float baseDamage)
        {
            float power = attacker.CharacterDataHolder.GetPower();
            float defense = defender.CharacterDataHolder.GetDef();

            float raw = baseDamage + power;

            // diminishing defense
            float damageMultiplier = 100f / (100f + defense);

            return raw * damageMultiplier;
        }

        private static float CalculateMagical(
            BaseBattleCharacterController attacker,
            BaseBattleCharacterController defender,
            float baseDamage)
        {
            float intelligence = attacker.CharacterDataHolder.GetINT();
            float defense = defender.CharacterDataHolder.GetDef();

            float raw = baseDamage + intelligence * 1.2f;

            float damageMultiplier = 100f / (100f + defense);

            return raw * damageMultiplier;
        }
    }
}