namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;

    public class HealthBelowNthValueCondition : IUsableCondition
    {
        private CustomVariable _varMaxHealth;

        public bool IsCardUsable(CardSO card, SceneCombatController battleManager)
        {
            _varMaxHealth = card.GetVariableByName("max_health_value");

            if (_varMaxHealth == null)
            {
                return true;
            }

            float playerCurrentHP = battleManager.Player.StatsManager.GetValue(EStatusType.HP);
            float playerMaxHP = battleManager.Player.StatsManager.GetMaxValue(EStatusType.HP);

            return playerCurrentHP / playerMaxHP < _varMaxHealth.GetValue<float>();
        }
    }
}