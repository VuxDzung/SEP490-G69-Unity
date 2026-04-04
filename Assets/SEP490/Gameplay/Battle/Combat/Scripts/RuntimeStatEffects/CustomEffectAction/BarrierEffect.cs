using SEP490G69.Battle.Cards;

namespace SEP490G69.Battle.Combat
{
    public class BarrierEffect : ISpecialStatusEffect
    {
        public void OnAfterAction(BaseCombatActor self, BaseCombatActor target)
        {
            
        }

        public void OnAfterBeingAttacked(float damage, BaseCombatActor self, BaseCombatActor attacker)
        {
            
        }

        public void OnApplied(BaseCombatActor self)
        {
            self.CombatController.Shield.OpenShieldBarrier();
        }

        public void OnBeforeAction(BaseCombatActor self, BaseCombatActor target)
        {
            
        }

        public void OnDiscarded(BaseCombatActor source)
        {
            source.CombatController.Shield.CloseShieldBarrier();
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
    }
}