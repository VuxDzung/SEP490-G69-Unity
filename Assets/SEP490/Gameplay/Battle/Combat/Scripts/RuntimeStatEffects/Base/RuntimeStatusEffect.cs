namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using System;

    /// <summary>
    /// This class holds the status effect in combat.
    /// </summary>
    public class RuntimeStatusEffect : IStatusTrigger
    {
        public const int STARTER_STACK = 1;

        public event Action<RuntimeStatusEffect> onStackEmpty;

        public StatusEffectSO Data { get; }

        public int Stack { get; private set; }

        private BaseBattleCharacterController owner;
        private ICardSpecialEffect _specialEffect;

        public ICardSpecialEffect SpecialEffect => _specialEffect;

        public RuntimeStatusEffect(StatusEffectSO data,
                                   BaseBattleCharacterController owner)
        {
            Data = data;
            this.owner = owner;
            _specialEffect = SpecialStatusEffectFactory.GetById(Data.EffectId);
            Stack = STARTER_STACK;
        }

        public void AddStack()
        {
            Stack++;
        }

        public void OnApply()
        {
            foreach (var mod in Data.Modifiers)
            {
                owner.AddEffectModifier(mod, Data.EffectId);
            }
        }

        public void OnTurnStart()
        {
            _specialEffect?.OnTurnStart(owner);

            if (Data.ApplyType == EApplyDiscardType.TurnStart)
            {
                DecreaseStack();
            }
        }

        public void OnTurnEnd()
        {
            if (Data.ApplyType == EApplyDiscardType.DiscardAfterNthTurns ||
                Data.ApplyType == EApplyDiscardType.TurnEnd)
            {
                DecreaseStack();
            }
        }

        public void OnAfterReceiveDamage(float damage)
        {
            // Handle reflect dmg logic.
            foreach (var modifier in Data.Modifiers)
            {
                if (modifier.StatType == EStatusType.ReceivedDmg)
                {
                    float reflect = modifier.GetModifiedStatus(damage);

                    owner.LastAttacker.ReceiveDamage(reflect, owner);
                }
            }

            if (Data.ApplyType == EApplyDiscardType.DiscardAfterBeingAtk)
            {
                DecreaseStack();
            }
        }

        private void DecreaseStack()
        {
            Stack--;

            if (Stack <= 0)
            {
                RemoveEffectModifiers();

                onStackEmpty?.Invoke(this);
            }
        }

        private void RemoveEffectModifiers()
        {
            owner.RemoveStatusEffect(Data.EffectId);
        }
    }
}