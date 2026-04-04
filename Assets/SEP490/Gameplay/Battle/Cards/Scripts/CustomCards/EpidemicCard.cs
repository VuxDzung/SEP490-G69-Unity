namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;

    public class EpidemicCard : BaseAttackCard
    {
        public EpidemicCard(CardSO cardSO) : base(cardSO)
        {
        }

        protected override void OnAfterAttack(float curDmg, BaseCombatActor source, BaseCombatActor opponent)
        {
            base.OnAfterAttack(curDmg, source, opponent);
            string decayId = StatusEffectConstants.STATUS_EFFECT_ID_0009;
            if (opponent.EffectsManager.GetById(decayId) != null)
            {
                opponent.EffectsManager.ManualTriggerEffect(decayId, source);
            }
        }
    }
}