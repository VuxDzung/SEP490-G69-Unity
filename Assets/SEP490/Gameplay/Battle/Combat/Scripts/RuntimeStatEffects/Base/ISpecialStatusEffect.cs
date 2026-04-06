namespace SEP490G69
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;

    public interface ISpecialStatusEffect
    {
        void SetSO(StatusEffectSO effectSO);

        void OnTurnStart(BaseCombatActor self, BaseCombatActor opponent);
        void OnTurnEnd(BaseCombatActor self, BaseCombatActor opponent);

        void OnBeforeAction(BaseCombatActor self, BaseCombatActor target);
        void OnAfterAction(BaseCombatActor self, BaseCombatActor target);

        void OnHitTarget(BaseCombatActor self, BaseCombatActor target);
        void OnAfterBeingAttacked(float damage, BaseCombatActor self, BaseCombatActor attacker);

        void OnApplied(BaseCombatActor self);
        void OnDiscarded(BaseCombatActor source);

        void TriggerManually(BaseCombatActor self, BaseCombatActor attacker);
    }
}