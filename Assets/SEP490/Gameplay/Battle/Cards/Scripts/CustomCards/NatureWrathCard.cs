namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;

    /// <summary>
    /// Deal damage. Dealing extra +50% dmg if you have Thorns.
    /// </summary>
    public class NatureWrathCard : BaseAttackCard
    {
        private readonly float _extraDmgMulValue = 0.5f;
        private const string VAR_EXTRA_DMG = "extra_dmg";

        public NatureWrathCard(CardSO cardSO) : base(cardSO)
        {
        }

        protected override float CalculateExtraDmg(float curDmg, BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            if (source.StatEffectManager.GetById(StatusEffectConstants.STATUS_EFFECT_ID_0023) != null)
            {
                CustomVariable var = Data.GetVariableByName(VAR_EXTRA_DMG);
                if (var != null)
                {
                    return var.GetDeltaValue(source);
                }
            }
            return base.CalculateExtraDmg(curDmg, source, target);
        }
    }
}