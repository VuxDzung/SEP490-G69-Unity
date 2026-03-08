namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public class ReduceStaminaGainEffect : ICardSpecialEffect
    {
        public float ModifyStatDelta(EStatusType statType, float delta, BaseBattleCharacterController self)
        {
            if (statType == EStatusType.Stamina && delta > 0)
            {
                return delta * 0.5f;
            }

            return delta;
        }

        public void OnTurnStart(BaseBattleCharacterController self) { }

        public void OnBeforeAction(BaseBattleCharacterController self, BaseBattleCharacterController target) { }

        public void OnAfterAction(BaseBattleCharacterController self, BaseBattleCharacterController target) { }

        public void OnAfterReceiveDmg(float damage, BaseBattleCharacterController self, BaseBattleCharacterController attacker) { }
    }
}