namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;

    public interface ICardEffectReceiver
    {
        public void ReceiveCardEffect(CardSO cardSO, BaseBattleCharacterController source, BaseBattleCharacterController target);
    }
}