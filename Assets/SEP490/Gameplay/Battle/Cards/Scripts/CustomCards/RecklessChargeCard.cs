namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    /// <summary>
    /// High damage but receive 20% Damage Dealt for self.
    /// </summary>
    public class RecklessChargeCard : BaseAttackCard
    {
        public const string VAR_RECEIEVED_DEALT_DMG = "received_dealt_dmg";

        private float _selfDamagePercent = 0.2f;

        public RecklessChargeCard(CardSO cardSO) : base(cardSO)
        {
        }

        protected override void OnAfterAttack(float curDmg, BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            base.OnAfterAttack(curDmg, source, target);
            CustomVariable varReceivedDealtDmg = Data.GetVariableByName(VAR_RECEIEVED_DEALT_DMG);
            if (varReceivedDealtDmg != null )
            {
                float receivedDmg = varReceivedDealtDmg.GetFinalValue(source); // curDmg * _selfDamagePercent;
                source.ReceiveDamage(receivedDmg, target);
            }
        }
    }
}