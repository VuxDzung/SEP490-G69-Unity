namespace SEP490G69.Battle.Combat
{
    public interface ICardEffectReceiver
    {
        public void ReceiveCardEffect(BaseBattleCharacterController source, BaseBattleCharacterController target);
    }
}