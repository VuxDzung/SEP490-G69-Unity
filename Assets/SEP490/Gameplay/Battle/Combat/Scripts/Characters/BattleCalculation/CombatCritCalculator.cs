namespace SEP490G69.Battle
{
    public class CombatCritCalculator : ICritCalculator
    {
        public float CalculateCritChance(float mind)
        {
            return (0.05f + (0.6125f - 0.05f) * (mind / (mind + 250f)));
        }

        public float CalculateCritMul(float powerValue)
        {
            return (1.5f + (2.5f - 1.5f) * (powerValue / (powerValue + 400f)));
        }
    }
}