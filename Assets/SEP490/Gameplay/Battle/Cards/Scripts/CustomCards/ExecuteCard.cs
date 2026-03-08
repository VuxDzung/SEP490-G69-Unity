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
        private const string VAR_EXTRA_DMG = "extra_dmg";
        public ExecuteCard(CardSO cardSO) : base(cardSO)
        {
        }

        protected override float CalculateExtraDmg(float curDmg, BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            if (target.StatEffectManager.GetById(StatusEffectConstants.STATUS_EFFECT_ID_0018) != null)
            {
                CustomVariable varExtraDmg = Data.GetVariableByName(VAR_EXTRA_DMG);
                if (varExtraDmg != null)
                {
                    return varExtraDmg.GetDeltaValue(source);
                }
                //return curDmg * _extraDmgMultiplyValue;
            } 
            return base.CalculateExtraDmg(curDmg, source, target);
        }
    }
}