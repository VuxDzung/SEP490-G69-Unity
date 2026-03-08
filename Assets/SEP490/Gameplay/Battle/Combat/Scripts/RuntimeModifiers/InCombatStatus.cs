namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Combat;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class represent a status in combat includes:
    /// Base Stats: POW, VIT, STA, AGI, INT.
    /// Combat-only stats: damage, received damage, reflected damage, action cost.
    /// </summary>
    public class InCombatStatus 
    {
        private float _currentValue;
        private List<CombatStatModifierSO> _modifierPool = new List<CombatStatModifierSO>();

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
        }

        public float BaseValue => _currentValue;

        public float Value
        {
            get
            {
                float value = _currentValue;

                foreach (var mod in _modifierPool)
                {
                    if (mod.ApplyValueType == EApplyValueType.GetterValue)
                    {
                        value = mod.GetModifiedStatus(value);
                    }
                }

                return value;
            }
        }

        public void AddModifier(CombatStatModifierSO modifier)
        {
            if (modifier.TriggerType == EModifierTriggerType.Immediate)
            {
                _currentValue = modifier.GetModifiedStatus(_currentValue);
                return;
            }

            _modifierPool.Add(modifier);
        }

        public void RemoveModifier(CombatStatModifierSO modifier)
        {
            CombatStatModifierSO existed = GetModifierById(modifier.Id);
            if (existed != null)
            {
                _modifierPool.Remove(existed);
            }
        }

        public void Trigger(ETurnFlowEvent flowEvent)
        {
            foreach (CombatStatModifierSO mod in _modifierPool)
            {
                if (mod.TriggerType == EModifierTriggerType.ByTurnFlowEvent &&
                    mod.TurnFlowEvent == flowEvent &&
                    mod.ApplyValueType == EApplyValueType.BaseOrCurrentValue)
                {
                    _currentValue = mod.GetModifiedStatus(_currentValue);
                }
            }
        }

        public CombatStatModifierSO GetModifierById(string modifierId)
        {
            return _modifierPool.FirstOrDefault(m => m.Id == modifierId);
        }
    }
}