namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;

    public class BerserkerSlashCard : BaseAttackCard
    {
        public BerserkerSlashCard(CardSO cardSO) : base(cardSO)
        {
        }

        protected override bool CheckForceCritCondition(BaseCombatActor source)
        {
            string berserkId = StatusEffectConstants.STATUS_EFFECT_ID_0017;

            if (source.EffectsManager.GetById(berserkId) != null)
            {
                return true;
            }

            return false;
        }
    }
}