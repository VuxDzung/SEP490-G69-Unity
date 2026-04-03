namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public class CombatContext
    {
        public BaseCombatActor Attacker;
        public BaseCombatActor Defender;

        public CardSO Card;

        public float Damage;

        public bool IsCritical;
    }
}