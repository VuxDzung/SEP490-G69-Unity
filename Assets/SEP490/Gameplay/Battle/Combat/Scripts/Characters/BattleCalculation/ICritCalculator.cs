namespace SEP490G69.Battle
{
    public interface ICritCalculator
    {
        public float CalculateCritChance(float mind);
        public float CalculateCritMul(float powerValue);
    }
}