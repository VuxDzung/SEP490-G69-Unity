using UnityEngine;

namespace SEP490G69.Battle
{
    public class CombatCritCalculator : ICritCalculator
    {
        public float CalculateCritChance(float agility)
        {
            float chance = Mathf.Min(agility * 0.01f, 0.4f);
            return (float)System.Math.Round(chance, 1);
        }

        public float CalculateCritMul(float agility)
        {
            float mul = Mathf.Min(1.5f + (agility * 0.02f), 2.2f);
            return (float)System.Math.Round(mul, 1); //(1.5f + (2.5f - 1.5f) * (powerValue / (powerValue + 400f)));
        }
    }
}