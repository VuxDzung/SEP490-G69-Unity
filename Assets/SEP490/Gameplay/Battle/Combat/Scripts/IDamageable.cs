namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;

    public interface IDamageable
    {
        public void ReceiveCardEffect(CardSO cardSO);
    }
}