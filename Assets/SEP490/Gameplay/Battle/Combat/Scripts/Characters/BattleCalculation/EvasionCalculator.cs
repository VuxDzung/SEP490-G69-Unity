namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public class EvasionCalculator : IEvasionCalculator
    {
        private readonly BaseBattleCharacterController _defender;

        public EvasionCalculator(BaseBattleCharacterController attacker)
        {
            _defender = attacker;
        }

        public float CalculateEvasionRate(BaseBattleCharacterController attacker)
        {
            InCombatStatus attackerAgi = attacker.GetCombatStatus(EStatusType.Agi);
            InCombatStatus defenderAgi = _defender.GetCombatStatus(EStatusType.Agi);

            if (defenderAgi == null || attackerAgi == null)
            {
                return 0f;
            }

            float deltaAgi = Mathf.Max(0, defenderAgi.Value - attackerAgi.Value);

            return 0.15f * (deltaAgi / (deltaAgi + 200f));
        }
    }
}