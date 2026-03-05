namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public class PurifyCard : BaseCard
    {
        public PurifyCard(CardSO data) : base(data)
        {
        }

        protected override void ExecuteAction(BaseBattleCharacterController self, BaseBattleCharacterController target)
        {
            base.ExecuteAction(self, target);
            RuntimeStatusEffect[] effects = self.StatEffectManager.GetEffectsByType(EEffectType.Debuff);
            foreach (var effect in effects)
            {
                self.StatEffectManager.Remove(effect);
            }
        }
    }
}