namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;

    public class RegenerationEffect : ICardSpecialEffect
    {
        public void OnAfterAction(BaseBattleCharacterController self, BaseBattleCharacterController target)
        {
            float maxHp = self.ReadonlyDataHolder.GetVIT();

            float heal = maxHp * 0.1f;// * Stack;

            self.CurrentDataHolder.ModifyStat(EStatusType.Vitality, heal);
        }

        public void OnAfterReceiveDmg(float damage, BaseBattleCharacterController self, BaseBattleCharacterController target)
        {
            throw new System.NotImplementedException();
        }
    }
}