namespace SEP490G69.Battle.Combat
{
    using UnityEngine;

    public class MaxStaminaCalculator : IMaxStaminaCalculator
    {
        public float CalculateMax(float baseStamina)
        {
            return 100 + (4 * Mathf.Pow(baseStamina, 0.6f));
        }
    }
}