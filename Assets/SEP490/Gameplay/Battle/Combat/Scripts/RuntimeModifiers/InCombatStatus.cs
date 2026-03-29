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

        public InCombatStatus()
        {
            _currentValue = 0f;
        }
        public InCombatStatus(float baseValue)
        {
            _currentValue = baseValue;
        }

        public void SetCurrentValue(float value)
        {
            _currentValue = value;

            _maxValue = Mathf.Max(_currentValue, _maxValue);
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
                        value = mod.ModifierSO.GetModifiedStatus(value);
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
                _currentValue = modifier.GetModifiedStatus(_currentValue);
                return;
            }

            InCombatStatModifier existed = GetRuntimeModifier(modifier.Id);
            if (existed != null)
            {
                existed.AddOwner(ownerId);
            }
            else
            {
                InCombatStatModifier runtimeModifier = new InCombatStatModifier(modifier);
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
            List<InCombatStatModifier> modifiers = _modifierPool.Where(mod => mod.OwnerIds.Contains(ownerId)).ToList();

            // Step 2: Remove the owner id in each modifier.
            foreach (InCombatStatModifier modifier in modifiers)
            {
                modifier.RemoveOwner(ownerId);

                // Step 2.1: If the modifier owner list is empty, remove the modifier from the modifier pool.
                if (modifier.OwnerIds.Count == 0)
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
                    _currentValue = mod.ModifierSO.GetModifiedStatus(_currentValue);
                }
            }
        }

        public InCombatStatModifier GetRuntimeModifier(string modifierId)
        {
            return _modifierPool.FirstOrDefault(mod => mod.ModifierSO.Id == modifierId);
        }
    }
}