namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;

    public class ThornsEffect : ISpecialStatusEffect
    {
        public void OnAfterBeingAttacked(float damage, BaseCombatActor self, BaseCombatActor attacker)
        {
            float reflectedDamage = 0f;
            if (self.IsPlayerOwnership)
            {
                reflectedDamage = self.StatsManager.GetValue(EStatusType.Vitality) * 0.25f;
            }
            else
            {
                reflectedDamage = self.StatsManager.GetValue(EStatusType.Attack) * 0.25f;
            }
            attacker.ReceiveDamage(reflectedDamage, attacker, true);
        }

        #region Unused
        public void OnApplied(BaseCombatActor self)
        {

        }

        public void OnBeforeAction(BaseCombatActor self, BaseCombatActor target)
        {
            
        }
        public void OnAfterAction(BaseCombatActor self, BaseCombatActor target)
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

        public void SetSO(StatusEffectSO effectSO)
        {
            
        }

        public void TriggerManually(BaseCombatActor self, BaseCombatActor attacker) { }

        #endregion
    }
}