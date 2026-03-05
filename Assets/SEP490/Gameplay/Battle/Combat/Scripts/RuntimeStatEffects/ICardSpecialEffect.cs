namespace SEP490G69
{
    using SEP490G69.Battle.Combat;

    public interface ICardSpecialEffect
    {
        public void OnAfterReceiveDmg(float damage, BaseBattleCharacterController self, BaseBattleCharacterController target);
        public void OnAfterAction(BaseBattleCharacterController self, BaseBattleCharacterController target);
    }
}