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

        private BaseCombatActor _owner;
        private ISpecialStatusEffect _specialEffect;

        public ISpecialStatusEffect SpecialEffect => _specialEffect;

        public RuntimeStatusEffect(StatusEffectSO data, BaseCombatActor owner)
        {
            Data = data;
            this._owner = owner;
            _specialEffect = SpecialEffectFactory.GetById(Data.EffectId);
            _specialEffect?.SetSO(data);
            Stack = STARTER_STACK;
        }

        public void AddStack()
        {
            Stack++;
        }

        public void OnApply()
        {
            foreach (CombatStatModifierSO mod in Data.Modifiers)
            {
                _owner.AddEffectModifier(mod, Data.EffectId);
            }
            _specialEffect?.OnApplied(_owner);
        }

        public void OnDiscard()
        {
            _specialEffect?.OnDiscarded(_owner);
            RemoveEffectModifiers();
        }

        public void OnTurnStart()
        {
            _specialEffect?.OnTurnStart(_owner, _owner.LastAttacker);

            if (Data.ApplyType == EEffectApplyType.TurnStart)
            {
                if (Data.EffectList.Count > 0)
                {
                    foreach (var effect in Data.EffectList)
                    {
                        _owner.EffectsManager.AddStatusEffect(effect);
                    }
                }
            }

            if (Data.DecayType == EDecayType.TurnStart)
            {
                DecreaseStack();
            }
        }

        public void OnTurnEnd()
        {
            _specialEffect?.OnTurnEnd(_owner, _owner.LastAttacker);
            if (Data.ApplyType == EEffectApplyType.TurnEnd)
            {
                if (Data.EffectList.Count > 0)
                {
                    foreach (var effect in Data.EffectList)
                    {
                        _owner.EffectsManager.AddStatusEffect(effect);
                    }
                }
            }
            if (Data.DecayType == EDecayType.TurnEnd)
            {
                DecreaseStack();
            }
        }

        public void TriggerManually(BaseCombatActor attacker)
        {
            _specialEffect?.TriggerManually(_owner, attacker);
        }

        public void OnHitTarget(BaseCombatActor opponent)
        {
            if (Data.ApplyType == EEffectApplyType.OnHitTarget)
            {
                if (Data.EffectList.Count > 0)
                {
                    foreach (var effect in Data.EffectList)
                    {
                        _owner.EffectsManager.AddStatusEffect(effect);
                    }
                }
            }

            _specialEffect?.OnHitTarget(_owner, opponent);
        }

        public void OnAfterBeingAttacked(float damage)
        {
            // Handle reflect dmg logic.
            //foreach (var modifier in Data.Modifiers)
            //{
            //    if (modifier.StatType == EStatusType.RelectedDmg)
            //    {
            //        float reflect = modifier.GetModifiedStatus(damage);

            //        _owner.LastAttacker.ReceiveDamage(reflect, _owner);
            //    }
            //}

            if (Data.ApplyType == EEffectApplyType.OnBeingAttacked)
            {
                if (Data.EffectList.Count > 0)
                {
                    foreach (var effect in Data.EffectList)
                    {
                        _owner.EffectsManager.AddStatusEffect(effect);
                    }
                }
            }

            _specialEffect?.OnAfterBeingAttacked(damage, _owner, _owner.LastAttacker);

            if (Data.DecayType == EDecayType.OnBeingAttacked)
            {
                DecreaseStack();
            }
        }

        private void DecreaseStack()
        {
            Stack--;

            if (Stack <= 0)
            {
                onStackEmpty?.Invoke(this);
            }
        }

        private void RemoveEffectModifiers()
        {
            _owner.RemoveEffectModifiers(Data.EffectId);
        }
    }
}