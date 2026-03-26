namespace SEP490G69.Battle.Combat
{
    using UnityEngine;

    public class MaxHPCalculator : IMaxHPCalculator
    {
        public float Calculate(float baseVitality)
        {
            return 500f + (15f * Mathf.Pow(baseVitality, 0.85f));
        }
    }
}