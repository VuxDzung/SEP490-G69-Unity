namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;

    public class CursedWhisperCard : BaseAttackCard
    {
        public CursedWhisperCard(CardSO cardSO) : base(cardSO)
        {
        }

        public override float CalculateExtraDmg(float curDmg, BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            if (target.StatEffectManager.Count(EEffectType.Debuff) > 0)
            {
                CustomVariable varExtraDmg = Data.GetVariableByName("extra_dmg");
                if (varExtraDmg != null)
                {
                    return varExtraDmg.GetDeltaValue(source);
                }
            }
            return base.CalculateExtraDmg(curDmg, source, target);
        }
    }
}