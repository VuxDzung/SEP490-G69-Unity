namespace SEP490G69.Exploration
{
    public class BossContactPercentCalculator : IBossContactPercentCalculator
    {
        public const float BASE_CONTACT_PERCENT = 0.1f;

        public float CalculateContactPercent(int exploreCount)
        {
            return BASE_CONTACT_PERCENT + (exploreCount * BASE_CONTACT_PERCENT);
        }
    }
}