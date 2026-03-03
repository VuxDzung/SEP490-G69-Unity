namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public class RuntimeStatusEffect 
    {
        #region Properties

        public StatusEffectSO Data { get; }
        public int Amount { get; set; }
        public int RemainingTurns { get; private set; }

        #endregion

        #region Fields

        private readonly BaseBattleCharacterController _owner;

        #endregion

        #region Constructor

        public RuntimeStatusEffect(StatusEffectSO data, BaseBattleCharacterController owner)
        {
            Data = data;
            _owner = owner;

            RemainingTurns = data.AliveTurnCount;
            Amount = 1;
        }

        #endregion

        #region Lifecycle

        /// <summary>
        /// Apply status effect immediately and then removed.
        /// </summary>
        public void OnApply()
        {
            if (Amount <= 0) return;

            if (Data.ApplyType == EApplyDiscardType.Immediate)
            {
                ApplyEffect();
                DecreaseAmount();
            }
        }

        public void OnTurnStart()
        {
            if (Data.ApplyType == EApplyDiscardType.DiscardAfterNthTurns)
            {
                RemainingTurns--;

                if (RemainingTurns <= 0)
                    Remove();
            }
        }

        public void OnTurnEnd()
        {
            // Reserved for future use
        }

        #endregion

        #region Damage Hooks

        public float ModifyIncomingDamage(float dmg)
        {
            if (Data.StatType == EStatusType.ReceivedDmg)
                return Data.GetModifiedStatus(dmg);

            return dmg;
        }

        public float ModifyDealableDmg(float dmg)
        {
            if (Data.StatType == EStatusType.Power)
                return Data.GetModifiedStatus(dmg);

            return dmg;
        }

        public void OnAfterReceiveDamage(float dmg)
        {
            if (Data.ApplyType == EApplyDiscardType.DiscardAfterBeingAtk)
            {
                DecreaseAmount();
            }
        }

        #endregion

        #region Internal Helpers

        private void ApplyEffect()
        {
            _owner.ApplyStatusEffect(Data);
        }

        private void DecreaseAmount()
        {
            Amount--;

            if (Amount <= 0)
                Remove();
        }

        private void Remove()
        {
            _owner.RemoveStatus(this);
        }

        #endregion
    }
}