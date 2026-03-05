namespace SEP490G69
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public class BerserkEffect : ICardSpecialEffect
    {
        public void OnAfterAction(BaseBattleCharacterController self, BaseBattleCharacterController target)
        {
            float maxHp = self.ReadonlyDataHolder.GetVIT();

            float selfDmg = maxHp * 0.15f;// * Stack;

            float current = self.CurrentDataHolder.GetVIT();

            self.CurrentDataHolder.SetStatus(EStatusType.Vitality, Mathf.Max(1, current - selfDmg));
        }

        public void OnAfterReceiveDmg(float damage, BaseBattleCharacterController self, BaseBattleCharacterController target) { }
    }
}