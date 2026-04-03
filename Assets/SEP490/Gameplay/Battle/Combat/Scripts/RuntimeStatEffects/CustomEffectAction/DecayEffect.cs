using SEP490G69.Battle.Cards;

namespace SEP490G69.Battle.Combat
{
    /// <summary>
    ///  Reduce health after each turn.
    /// </summary>
    public class DecayEffect : ISpecialStatusEffect
    {
        public void OnTurnStart(BaseCombatActor self, BaseCombatActor opponent) { }

        public void OnTurnEnd(BaseCombatActor self, BaseCombatActor opponent)
        {
            RuntimeStatusEffect statusEffect = self.EffectsManager.GetById("ste_0009");
            if (statusEffect != null && statusEffect.Data != null)
            {
                float extraDamage = statusEffect.Stack * (1 + opponent.StatsManager.GetValue(EStatusType.Intelligence) * 0.01f);
                self.ReceiveDamage(extraDamage, opponent);
            }
        }

        #region Unsused

        public void OnAfterAction(BaseCombatActor self, BaseCombatActor target)
        {

        }

        public void OnAfterBeingAttacked(float damage, BaseCombatActor self, BaseCombatActor attacker)
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

        public void OnApplied(BaseCombatActor self)
        {
        }

        public void SetSO(StatusEffectSO effectSO)
        {
        }
        #endregion
    }
}