namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public class EvasionCalculator : IEvasionCalculator
    {
        private readonly BaseCombatActor _defender;

        public EvasionCalculator(BaseCombatActor attacker)
        {
            _defender = attacker;
        }

        public float CalculateEvasionRate(BaseCombatActor attacker)
        {
            float attackerAgi = attacker.StatsManager.GetValue(EStatusType.Agi);
            float defenderAgi = _defender.StatsManager.GetValue(EStatusType.Agi);

            float deltaAgi = Mathf.Max(0, attackerAgi - attackerAgi);

            return 0.05f + 0.5f * (deltaAgi / (deltaAgi + 100f));
        }
    }
}