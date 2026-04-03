using SEP490G69.Battle.Combat;

namespace SEP490G69.Battle
{
    public interface IEvasionCalculator 
    {
        public float CalculateEvasionRate(BaseCombatActor attacker);
    }
}