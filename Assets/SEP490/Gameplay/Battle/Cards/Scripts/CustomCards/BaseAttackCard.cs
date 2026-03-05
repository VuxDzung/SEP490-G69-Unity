namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    /// <summary>
    /// Base attack card. Handle most of the attack card logic.
    /// </summary>
    public class BaseAttackCard : BaseCard
    {
        public BaseAttackCard(CardSO cardSO) : base(cardSO) { }

        protected override void ExecuteAction(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            float damage = CalculateDamage(source);

            damage = source.StatEffectManager.ModifyDealableDamage(damage);
            damage = target.StatEffectManager.ModifyIncomingDamage(damage);

            Debug.Log($"Damage: {damage}");
            damage += CalculateExtraDmg(damage, source, target);
            for (int i = 0; i < Data.AtkCount; i++)
            {
                target.ReceiveDamage(damage, source);
            }
            OnAfterAttack(damage, source, target);
        }

        protected virtual void OnAfterAttack(float curDmg, BaseBattleCharacterController source, BaseBattleCharacterController target)
        {

        }

        protected virtual float CalculateDamage(BaseBattleCharacterController source)
        {
            float baseDamage = Data.BaseValue;
            float characterPow = source.ReadonlyDataHolder.GetPower();

            float stat = source.CurrentDataHolder.GetStatus(Data.ModifyStatType);

            float modified = baseDamage + Data.GetDelta(characterPow);

            return modified;
        }

        protected virtual float CalculateExtraDmg(float curDmg, BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            return 0;
        }
    }
}