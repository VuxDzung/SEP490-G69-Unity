namespace SEP490G69.Battle.Combat
{
    using UnityEngine;

    public class CombatStaminaCalculator : ICombatStaminaCalculator
    {
        public float CalculateMax(float baseStamina)
        {
            float value = 80f + (4 * Mathf.Pow(baseStamina, 0.6f));
            return Mathf.RoundToInt(value);
        }

        public float CalculateRegenStamina(float baseStamina)
        {
            float value = 20f + (baseStamina * 0.5f);
            return Mathf.RoundToInt(value); 
        }
    }
}