namespace SEP490G69.Battle.Combat
{
    public class CharacterStaminaManager
    {
        private BaseCombatActor _controller;
        private CharacterStatsManager _statsManager;

        public CharacterStaminaManager(BaseCombatActor controller)
        {
            _controller = controller;
            _statsManager = _controller.StatsManager;
        }

        public bool CanSpend(float cost)
        {
            float stamina = _statsManager.GetValue(EStatusType.Stamina);
            return stamina >= cost;
        }

        public void Spend(float value)
        {
            float stamina = _statsManager.GetValue(EStatusType.Stamina);

            float remainStamina = stamina - value;
            if (remainStamina < 0)
            {
                remainStamina = 0;
            }
            _statsManager.SetCurrentValue(EStatusType.Stamina, remainStamina, true);
        }

        public void RefillStatmina()
        {
            float refilledStamina = _statsManager.GetMaxValue(EStatusType.Stamina) * 0.7f;
            _statsManager.SetCurrentValue(EStatusType.Stamina, refilledStamina, true);
        }
    }
}