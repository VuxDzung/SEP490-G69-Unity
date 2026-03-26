namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;

    /// <summary>
    /// Only usable if HP < 25%. Massive damage.
    /// </summary>
    public class LastResortCard : BaseAttackCard
    {
        public LastResortCard(CardSO cardSO) : base(cardSO)
        {
        }

        protected override bool ExecuteCondition(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            float currentVit = source.GetCombatStatus(EStatusType.HP).Value;
            float maxVit = source.GetCombatStatus(EStatusType.HP).MaxValue;
            if ((currentVit / maxVit) < 0.25f)
            {
                return true;
            }
            return false;
        }
    }
}