namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public class NullifyCard : BaseAttackCard
    {
        public NullifyCard(CardSO cardSO) : base(cardSO)
        {
        }

        protected override void OnAfterAttack(float curDmg, BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            base.OnAfterAttack(curDmg, source, target);
            RuntimeStatusEffect statEffect = target.StatEffectManager.GetRandomStatusEffect();
            target.StatEffectManager.Remove(statEffect);
        }
    }
}