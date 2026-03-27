namespace SEP490G69.Battle.Combat
{
    using UnityEngine;

    public class MaxHPCalculator : IMaxHPCalculator
    {
        public const float BASE_HP = 100f;
        public float Calculate(float baseVitality)
        {
            return BASE_HP + (15f * Mathf.Pow(baseVitality, 0.85f));
        }
    }
}