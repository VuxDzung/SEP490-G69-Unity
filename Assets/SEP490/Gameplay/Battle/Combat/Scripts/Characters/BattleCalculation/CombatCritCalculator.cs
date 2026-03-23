namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Combat;

    public class CombatCritCalculator : ICritCalculator
    {
        private readonly BaseBattleCharacterController _owner;

        public CombatCritCalculator(BaseBattleCharacterController owner)
        {
            _owner = owner;
        }

        public float CalculateCritChance()
        {
            InCombatStatus statInt = _owner.GetCombatStatus(EStatusType.Intelligence);

            if (statInt == null)
            {
                return 0;
            }

            return (0.05f + (0.6125f - 0.05f) * (statInt.Value / (statInt.Value + 250f)));
        }

        public float CalculateCritMul()
        {
            InCombatStatus statPow = _owner.GetCombatStatus(EStatusType.Power);

            if (statPow == null)
            {
                return 0;
            }

            return (1.5f + (2.5f - 1.5f) * (statPow.Value / (statPow.Value + 400f)));
        }
    }
}