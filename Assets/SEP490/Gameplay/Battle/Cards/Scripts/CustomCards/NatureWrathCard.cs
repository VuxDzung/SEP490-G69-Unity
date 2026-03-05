namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public class NatureWrathCard : BaseAttackCard
    {
        private readonly float _extraDmgMulValue = 0.5f;

        public NatureWrathCard(CardSO cardSO) : base(cardSO)
        {
        }

        protected override float CalculateExtraDmg(float curDmg, BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            if (source.StatEffectManager.GetEffectById(StatusEffectConstants.STATUS_EFFECT_ID_0023) != null)
            {
                return curDmg * _extraDmgMulValue;
            }
            return base.CalculateExtraDmg(curDmg, source, target);
        }
    }
}