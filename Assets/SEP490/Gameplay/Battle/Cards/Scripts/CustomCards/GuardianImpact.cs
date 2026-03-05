namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public class GuardianImpact : BaseAttackCard
    {
        public GuardianImpact(CardSO cardSO) : base(cardSO)
        {
        }

        protected override bool CheckInflictCondition(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            float currentVit = source.CurrentDataHolder.GetVIT();
            float enemyPow = target.CurrentDataHolder.GetPower();
            if (currentVit > enemyPow)
            {
                return true;
            }
            return false;
        }
    }
}