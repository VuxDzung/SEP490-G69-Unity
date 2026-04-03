namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;

    public class PersistentEffect : ISpecialStatusEffect
    {
        private StatusEffectSO _effectSO;

        private CustomVariable _varMinHP;

        public void SetSO(StatusEffectSO effectSO)
        {
            _effectSO = effectSO;
            _varMinHP = _effectSO.GetVariableByName("min_hp");
        }

        public void OnAfterAction(BaseCombatActor self, BaseCombatActor target)
        {
            float minHPValue = _varMinHP.GetValue<float>();

            if (self.StatsManager.GetValue(EStatusType.HP) < minHPValue)
            {
                self.StatsManager.SetCurrentValue(EStatusType.HP, minHPValue);
            }
        }

        public void OnAfterBeingAttacked(float damage, BaseCombatActor self, BaseCombatActor attacker)
        {
        }

        public void OnApplied(BaseCombatActor self)
        {
        }

        public void OnBeforeAction(BaseCombatActor self, BaseCombatActor target)
        {
        }

        public void OnDiscarded(BaseCombatActor source)
        {
        }

        public void OnHitTarget(BaseCombatActor self, BaseCombatActor target)
        {
        }

        public void OnTurnEnd(BaseCombatActor self, BaseCombatActor opponent)
        {
        }

        public void OnTurnStart(BaseCombatActor self, BaseCombatActor opponent)
        {
        }
    }
}