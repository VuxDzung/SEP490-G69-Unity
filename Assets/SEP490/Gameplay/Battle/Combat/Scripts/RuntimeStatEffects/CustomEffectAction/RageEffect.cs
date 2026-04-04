using SEP490G69.Battle.Cards;

namespace SEP490G69.Battle.Combat
{
    public class RageEffect : ISpecialStatusEffect
    {
        public void OnTurnStart(BaseCombatActor self, BaseCombatActor opponent)
        {

        }

        public void OnTurnEnd(BaseCombatActor self, BaseCombatActor opponent)
        {

        }

        public void OnBeforeAction(BaseCombatActor self, BaseCombatActor target)
        {

        }

        public void OnAfterAction(BaseCombatActor self, BaseCombatActor target)
        {

        }

        public void OnAfterBeingAttacked(float damage, BaseCombatActor self, BaseCombatActor attacker) { }

        public void OnApplied(BaseCombatActor self) { }

        public void OnDiscarded(BaseCombatActor source) { }

        public void OnHitTarget(BaseCombatActor self, BaseCombatActor target)
        {
            string damageUpId = "ste_0001";
            self.EffectsManager.RemoveById(damageUpId);
        }

        public void SetSO(StatusEffectSO effectSO) { }

        public void TriggerManually(BaseCombatActor self, BaseCombatActor attacker) { }

    }
}