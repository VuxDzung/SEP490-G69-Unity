namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;

    public class BerserkerSlashCard : BaseAttackCard
    {
        public BerserkerSlashCard(CardSO cardSO) : base(cardSO) { }

        protected override bool CheckForceCritCondition(BaseBattleCharacterController source)
        {
            if (source.StatEffectManager.GetById(CardConstants.CARD_ID_18) != null)
            {
                return true;
            }
            return false;
        }
    }
}