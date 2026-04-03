namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;

    public class PurifyCard : BaseCard
    {
        public PurifyCard(CardSO data) : base(data) { }

        public override void Execute(PlayerActorController source, BaseCombatActor target)
        {
            base.Execute(source, target);
            var effects = source.EffectsManager.GetEffectsByType(EEffectType.Debuff);
            foreach (var effect in effects)
            {
                source.EffectsManager.Remove(effect);
            }
        }
    }
}