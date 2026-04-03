using SEP490G69.Battle.Cards;

namespace SEP490G69.Battle.Combat
{
    public class BleedEffect : ISpecialStatusEffect
    {
        private StatusEffectSO _effectSO;

        public void OnAfterBeingAttacked(float damage, BaseCombatActor self, BaseCombatActor attacker)
        {
            RuntimeStatusEffect bleedEffect = self.EffectsManager.GetById(_effectSO.EffectId);
            if (bleedEffect != null)
            {
                float bleedDamage = (bleedEffect.Stack * 2) + (attacker.StatsManager.GetValue(EStatusType.Power) * 0.1f);
                self.ReceiveDamage(bleedDamage, attacker, true);
            }
        }

        #region Unused methods
        public void OnAfterAction(BaseCombatActor self, BaseCombatActor target) { }

        public void OnBeforeAction(BaseCombatActor self, BaseCombatActor target) { }

        public void OnTurnEnd(BaseCombatActor self, BaseCombatActor opponent)
        {

        }

        public void OnTurnStart(BaseCombatActor self, BaseCombatActor opponent) { }

        public void OnDiscarded(BaseCombatActor source) { }

        public void OnHitTarget(BaseCombatActor self, BaseCombatActor target)
        {
            
        }

        public void OnApplied(BaseCombatActor self)
        {

        }

        public void SetSO(StatusEffectSO effectSO)
        {
            _effectSO = effectSO;
        }

        #endregion
    }
}