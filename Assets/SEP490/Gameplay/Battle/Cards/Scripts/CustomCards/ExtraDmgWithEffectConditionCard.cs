namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;

    public class ExtraDmgWithEffectConditionCard : BaseAttackCard
    {
        private readonly CustomVariable _varExtraDmg;
        private readonly CustomVariable _varEffectId;

        public ExtraDmgWithEffectConditionCard(CardSO cardSO) : base(cardSO)
        {
            _varExtraDmg = cardSO.GetVariableByName("extra_dmg");
            _varEffectId = cardSO.GetVariableByName("conditional_effect");
        }

        public override float CalculateExtraDmg(float curDmg, PlayerActorController source, BaseCombatActor target)
        {
            string effectId = _varEffectId.GetValue<string>();
            if (source.EffectsManager.GetById(effectId) != null)
            {
                return _varExtraDmg.GetDeltaValue(source);
            }
            return 0f;
        }
    }
}