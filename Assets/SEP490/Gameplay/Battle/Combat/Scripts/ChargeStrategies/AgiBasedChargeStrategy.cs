namespace SEP490G69.Battle.Combat
{
    public class AgiBasedChargeStrategy : IChargeStrategy
    {
        private readonly float _baseSpeed;
        private const float K = 400f;

        public AgiBasedChargeStrategy(float baseSpeed)
        {
            _baseSpeed = baseSpeed;
        }

        /// <summary>
        /// FillSpeed = BaseSpeed * (1+ (2 * AGI) / (AGI+400))
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public float ChargeEnergy(float agility, float deltaTime)
        {
            float multiplier = 1f + (2f * agility) / (agility + K);

            float finalSpeed = _baseSpeed * multiplier;

            return finalSpeed * deltaTime;
        }
    }
}