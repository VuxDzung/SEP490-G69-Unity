namespace SEP490G69.Battle.Combat
{
    public class CharacterShieldController
    {
        private BaseCombatActor _controller;
        private CharacterStatsManager _statsManager;

        private bool _hasBarrier;

        public CharacterShieldController(BaseCombatActor controller)
        {
            _controller = controller;
            _statsManager = _controller.StatsManager;
        }

        public void Initialize()
        {
            float maxVit = _statsManager.GetMaxValue(EStatusType.Vitality);
            _statsManager.SetMaxValue(EStatusType.Shield, maxVit * 0.75f);
        }

        public void AddShield(EStatusType scaleStat, float baseCardShield, float modifierValue)
        {
            float additionalShield = CalculateStackShieldValue(scaleStat, baseCardShield, modifierValue);

            _statsManager.SetCurrentValue(EStatusType.ReceivedShield, additionalShield, true);

            float statShield = _statsManager.GetValue(EStatusType.Shield);
            float receivedShield = _statsManager.GetValue(EStatusType.ReceivedShield);

            float finalShield = statShield + receivedShield;

            _statsManager.SetCurrentValue(EStatusType.Shield, finalShield, true);
        }

        public float CalculateStackShieldValue(EStatusType scaleStat, float baseCardShield, float modifierValue)
        {
            return baseCardShield + (_statsManager.GetValue(scaleStat) * modifierValue);
        }

        public void AbsorbDamage(float damage, out float remaining)
        {
            float shield = _statsManager.GetValue(EStatusType.Shield);

            if (shield >= damage)
            {
                _statsManager.SetCurrentValue(EStatusType.Shield, shield - damage);
                remaining = 0;
            }
            else
            {
                remaining = damage - shield;
                _statsManager.SetCurrentValue(EStatusType.Shield, 0f);
            }
        }

        public void ResetShield()
        {
            if (_hasBarrier == true)
            {
                return;
            }

            float maxVit = _statsManager.GetMaxValue(EStatusType.Vitality);
            _statsManager.SetMaxValue(EStatusType.Shield, maxVit * 0.75f);
        }

        public void OpenShieldBarrier()
        {
            _hasBarrier = true;
        }
        public void CloseShieldBarrier()
        {
            _hasBarrier = false;
        }
    }
}