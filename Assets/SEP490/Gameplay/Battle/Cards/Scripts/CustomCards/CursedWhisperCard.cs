namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public class CursedWhisperCard : BaseAttackCard
    {
        public CursedWhisperCard(CardSO cardSO) : base(cardSO)
        {
        }

        protected override float CalculateExtraDmg(float curDmg, BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            if (target.StatEffectManager.Count() > 0)
            {
                return curDmg * 1.5f;
            }
            return base.CalculateExtraDmg(curDmg, source, target);
        }
    }
}