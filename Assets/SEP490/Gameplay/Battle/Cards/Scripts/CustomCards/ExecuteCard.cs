namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    /// <summary>
    /// Deal damage. Dealing extra 2x damage if target has Vulnerable.
    /// </summary>
    public class ExecuteCard : BaseAttackCard
    {
        private readonly float _extraDmgMultiplyValue = 2;

        public ExecuteCard(CardSO cardSO) : base(cardSO)
        {
        }

        protected override float CalculateExtraDmg(float curDmg, BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            if (target.StatEffectManager.GetEffectById(StatusEffectConstants.STATUS_EFFECT_ID_0018) != null)
            {
                return curDmg * _extraDmgMultiplyValue;
            } 
            return base.CalculateExtraDmg(curDmg, source, target);
        }
    }
}