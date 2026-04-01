namespace SEP490G69.Battle.Combat
{
    using UnityEngine;

    public class CombatHPCalculator : ICombatHPCalculator
    {
        public const float BASE_HP = 100f;
        public float Calculate(float baseVitality)
        {
            float value = BASE_HP + (15f * Mathf.Pow(baseVitality, 0.85f));
            return Mathf.RoundToInt(value);
        }
    }
}