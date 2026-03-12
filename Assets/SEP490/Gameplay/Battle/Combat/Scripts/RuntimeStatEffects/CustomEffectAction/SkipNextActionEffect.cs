namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public class SkipNextActionEffect : ICardSpecialEffect
    {
        private bool used = false;

        public void OnBeforeAction(BaseBattleCharacterController self, BaseBattleCharacterController target)
        {

        }

        public void OnTurnStart(BaseBattleCharacterController self)
        {
            if (used)
            {
                return;
            }

            used = true;

            Debug.Log($"{self.name} skips action");

            self.SelectNoAction();
            self.ExecuteCard(self, null);
        }

        public void OnAfterAction(BaseBattleCharacterController self, BaseBattleCharacterController target) { }

        public void OnAfterReceiveDmg(float damage, BaseBattleCharacterController self, BaseBattleCharacterController attacker) { }

        public float ModifyStatDelta(EStatusType statType, float delta, BaseBattleCharacterController self)
        {
            return delta;
        }
    }
}