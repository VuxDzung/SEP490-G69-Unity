namespace SEP490G69.Battle.Combat
{
    public interface ICombatStaminaCalculator 
    {
        /// <summary>
        /// Calculate max combat stamina.
        /// </summary>
        /// <param name="baseStamina"></param>
        /// <returns></returns>
        public float CalculateMax(float baseStamina);

        /// <summary>
        /// Calculate regeneration amount of stamina per turn.
        /// </summary>
        /// <param name="baseStamina"></param>
        /// <returns></returns>
        public float CalculateRegenStamina(float baseStamina);
    }
}