namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public class ThornEffect : ICardSpecialEffect
    {
        public void OnAfterAction(BaseBattleCharacterController self, BaseBattleCharacterController target)
        {
            
        }

        public void OnAfterReceiveDmg(float damage, BaseBattleCharacterController self, BaseBattleCharacterController target)
        {
            float reflect = damage * 0.15f;// * Stack;

            self.LastAttacker.ReceiveDamage(reflect, self);
        }
    }
}