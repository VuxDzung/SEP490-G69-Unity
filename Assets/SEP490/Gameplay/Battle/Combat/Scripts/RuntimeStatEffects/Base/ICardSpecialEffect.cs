namespace SEP490G69
{
    using SEP490G69.Battle.Combat;

    public interface ICardSpecialEffect
    {
        void OnTurnStart(BaseBattleCharacterController self);

        void OnBeforeAction(BaseBattleCharacterController self, BaseBattleCharacterController target);

        void OnAfterAction(BaseBattleCharacterController self, BaseBattleCharacterController target);

        void OnAfterReceiveDmg(float damage, BaseBattleCharacterController self, BaseBattleCharacterController attacker);

        float ModifyStatDelta(EStatusType statType, float delta, BaseBattleCharacterController self);
    }
}