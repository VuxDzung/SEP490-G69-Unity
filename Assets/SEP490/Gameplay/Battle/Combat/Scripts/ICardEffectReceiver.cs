namespace SEP490G69.Battle.Combat
{
    public interface ICardEffectReceiver
    {
        public void ExecuteCard(BaseBattleCharacterController source, BaseBattleCharacterController target);
    }
}