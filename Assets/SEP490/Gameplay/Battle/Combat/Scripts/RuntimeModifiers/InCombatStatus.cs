namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Combat;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// This class represent a status in combat includes:
    /// Base Stats: POW, VIT, STA, AGI, INT.
    /// Combat-only stats: damage, received damage, reflected damage, action cost.
    /// </summary>
    public class InCombatStatus 
    {
        private float _currentValue;
        private float _maxValue;
        private List<InCombatStatModifier> _modifierPool = new List<InCombatStatModifier>();
        private StatusEffectManager _effectsManager;

        public InCombatStatus(StatusEffectManager effectManager)
        {
            _currentValue = 0f;
            _effectsManager = effectManager;
        }

        public void SetCurrentValue(float value, bool clampMax = false)
        {
            _currentValue = value;
            if (clampMax)
            {
                _currentValue = Mathf.Clamp(_currentValue, 0f, _maxValue);
            }
        }

        public void SetMaxValue(float maxValue)
        {
            _maxValue = maxValue;
        }

        public float BaseValue => _currentValue;
        public float MaxValue => _maxValue;
        public float Value
        {
            get
            {
                float value = _currentValue;

                foreach (var mod in _modifierPool)
                {
                    if (mod.ModifierSO.ApplyValueType == EApplyValueType.GetterValue)
                    {
                        int stack = mod.OwnerStack;
                        value = mod.ModifierSO.GetModifiedStatus(value, stack);
                    }
                }

                if (value < 0f)
                {
                    value = 0f;
                }

                return value;
            }
        }

        public float GetValue(bool roundToInt)
        {
            return roundToInt ? (float)System.Math.Round(Value, 0) : Value;
        }

        public void AddModifier(CombatStatModifierSO modifier, string ownerId)
        {
            if (modifier.TriggerType == EModifierTriggerType.Immediate)
            {
                SetCurrentValue(modifier.GetModifiedStatus(_currentValue), true);
                return;
            }

            InCombatStatModifier existed = GetRuntimeModifier(modifier.Id);

            if (existed != null)
            {
                existed.AddOwner(ownerId);
            }
            else
            {
                InCombatStatModifier runtimeModifier = new InCombatStatModifier(modifier, _effectsManager);
                runtimeModifier.AddOwner(ownerId);

                _modifierPool.Add(runtimeModifier);
            }
        }

        public void RemoveModifier(string modifierId)
        {
            InCombatStatModifier existed = GetRuntimeModifier(modifierId);
            if (existed != null)
            {
                _modifierPool.Remove(existed);
            }
        }

        public void RemoveModifiersByOwner(string ownerId)
        {
            // Step 1: Get all modifiers which belongs to the owner.
            List<InCombatStatModifier> modifiers = _modifierPool.Where(mod => mod.Owners.Contains(ownerId)).ToList();

            // Step 2: Remove the owner id in each modifier.
            foreach (InCombatStatModifier modifier in modifiers)
            {
                modifier.RemoveOwner(ownerId);

                // Step 2.1: If the modifier owner list is empty, remove the modifier from the modifier pool.
                if (modifier.Owners.Count == 0)
                {
                    RemoveModifier(modifier.ModifierSO.Id);
                }
            }
        }

        public void Trigger(ETurnFlowEvent flowEvent)
        {
            foreach (InCombatStatModifier mod in _modifierPool)
            {
                if (mod.ModifierSO.TriggerType == EModifierTriggerType.ByTurnFlowEvent &&
                    mod.ModifierSO.TurnFlowEvent == flowEvent &&
                    mod.ModifierSO.ApplyValueType == EApplyValueType.BaseOrCurrentValue)
                {
                    SetCurrentValue(mod.ModifierSO.GetModifiedStatus(_currentValue), true);
                }
            }
        }

        public InCombatStatModifier GetRuntimeModifier(string modifierId)
        {
            return _modifierPool.FirstOrDefault(mod => mod.ModifierSO.Id == modifierId);
        }
    }
}