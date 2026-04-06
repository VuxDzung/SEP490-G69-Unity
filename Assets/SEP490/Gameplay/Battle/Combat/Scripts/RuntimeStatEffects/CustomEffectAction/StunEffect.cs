namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;

    public class StunEffect : ISpecialStatusEffect
    {
        public void OnApplied(BaseCombatActor source)
        {
            source.EnableSkipTurn();
        }
        public void OnDiscarded(BaseCombatActor source)
        {
            source.DisableSkipTurn();
        }

        #region Unused
        public void OnBeforeAction(BaseCombatActor self, BaseCombatActor target) { }

        public void OnAfterAction(BaseCombatActor self, BaseCombatActor target) { }

        public void OnHitTarget(BaseCombatActor self, BaseCombatActor target) { }

        public void OnAfterBeingAttacked(float damage, BaseCombatActor self, BaseCombatActor attacker) { }

        public void OnTurnEnd(BaseCombatActor self, BaseCombatActor opponent) { }

        public void OnTurnStart(BaseCombatActor self, BaseCombatActor opponent) { }

        public void SetSO(StatusEffectSO effectSO) { }

        public void TriggerManually(BaseCombatActor self, BaseCombatActor attacker) { }

        #endregion
    }
}