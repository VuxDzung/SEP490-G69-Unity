namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using System;
    using UnityEngine;

    /// <summary>
    /// This class holds the status effect in combat.
    /// </summary>
    public class RuntimeStatusEffect : IStatusTrigger
    {
        public event Action<RuntimeStatusEffect> onStackEmpty;

        public StatusEffectSO Data { get; }

        public int Stack { get; private set; }

        private BaseBattleCharacterController owner;

        public RuntimeStatusEffect(StatusEffectSO data,
                                   BaseBattleCharacterController owner)
        {
            Data = data;
            this.owner = owner;

            Stack = 1;
        }

        public void AddStack()
        {
            Stack++;
        }

        public void OnApply()
        {
        }

        public void OnTurnStart()
        {
        }

        public void OnTurnEnd()
        {
            if (Data.EffectId == "Regeneration")
            {
                float maxHp = owner.ReadonlyDataHolder.GetVIT();

                float heal = maxHp * 0.1f * Stack;

                owner.CurrentDataHolder.ModifyStat(
                    EStatusType.Vitality,
                    heal);
            }

            if (Data.ApplyType == EApplyDiscardType.DiscardAfterNthTurns)
            {
                DecreaseStack();
            }
        }

        public float ModifyIncomingDamage(float dmg)
        {
            foreach (var modifier in Data.Modifiers)
            {
                if (modifier.StatType == EStatusType.ReceivedDmg)
                {
                    dmg += modifier.GetDelta(dmg);
                }
            }
            //if (Data.EffectId == "Vulnerable")
            //{
            //    dmg *= 1.2f * Stack;
            //}

            return dmg;
        }

        public float ModifyDealDamage(float dmg)
        {
            foreach (var modifier in Data.Modifiers)
            {
                if (modifier.StatType == EStatusType.Damage)
                {
                    dmg += modifier.GetDelta(dmg);
                }
            }
            //if (Data.EffectId == "Berserk")
            //{
            //    dmg *= 1.3f * Stack;
            //}

            return dmg;
        }

        public void OnAfterReceiveDamage(float dmg)
        {
            if (Data.EffectId == StatusEffectConstants.STATUS_EFFECT_ID_0023)
            {
                float reflect = dmg * 0.15f * Stack;

                owner.LastAttacker.ReceiveDamage(reflect, owner);
            }

            if (Data.ApplyType == EApplyDiscardType.DiscardAfterBeingAtk)
            {
                DecreaseStack();
            }
        }

        public void OnAction()
        {
            // Handle berserk logic.
            if (Data.EffectId != "ste_0024")
            {
                return;
            }

            float maxHp = owner.ReadonlyDataHolder.GetVIT();

            float selfDmg = maxHp * 0.15f * Stack;

            float current = owner.CurrentDataHolder.GetVIT();

            owner.CurrentDataHolder.SetStatus(EStatusType.Vitality, Mathf.Max(1, current - selfDmg));
        }

        private void DecreaseStack()
        {
            Stack--;
            if (Stack <= 0)
            {
                onStackEmpty?.Invoke(this);
            }
        }
    }
}