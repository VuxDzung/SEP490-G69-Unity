namespace SEP490G69.Battle.Combat
{
    using System.Collections.Generic;
    using UnityEngine;

    public class CharacterStatsManager
    {
        private readonly Dictionary<EStatusType, InCombatStatus> _stats;
        private readonly BaseCombatActor _controller;

        public CharacterStatsManager(BaseCombatActor controller)
        {
            _stats = new Dictionary<EStatusType, InCombatStatus>();
            _controller = controller;
        }

        public void AddStatus(EStatusType statusType, InCombatStatus status)
        {
            _stats[statusType] = status;
        }

        public InCombatStatus Get(EStatusType type) => _stats.TryGetValue(type, out var s) ? s : null;

        public float GetValue(EStatusType type) => Get(type)?.Value ?? 0f;

        public bool TryGetStatus(EStatusType statType, out InCombatStatus status)
        {
            return _stats.TryGetValue(statType, out status);
        }

        public InCombatStatus GetStatus(EStatusType statType)
        {
            if (_stats.ContainsKey(statType))
            {
                return _stats[statType];
            }
            return null;
        }

        public float GetMaxValue(EStatusType statType) => Get(statType)?.MaxValue ?? 0f;

        public void ApplyStatsDelta(CombatStatModifierSO statModifier)
        {
            if (statModifier == null)
                return;

            if (_stats.TryGetValue(statModifier.StatType, out var status) == false)
                return;

            float maxValue = GetMaxValue(statModifier.StatType);
            float currentValue = status.Value;

            float calculationValue = statModifier.CalculateSource switch
            {
                EStatCalculationSource.Current => currentValue,
                EStatCalculationSource.Max => maxValue,
                EStatCalculationSource.Lost => maxValue - currentValue,
                EStatCalculationSource.FixedValue => statModifier.Value,
                _ => currentValue
            };

            float delta = statModifier.GetDelta(calculationValue);

            // Let status effects modify the delta
            //delta = _controller.EffectsManager.ModifyStatDelta(statModifier.StatType, delta);

            float newValue = currentValue + delta;

            newValue = Mathf.Clamp(newValue, 0, maxValue);

            status.SetCurrentValue(newValue, true);
        }

        public void AddEffectModifier(CombatStatModifierSO modifierSO, string effectId)
        {
            if (TryGetStatus(modifierSO.StatType, out InCombatStatus status))
            {
                status.AddModifier(modifierSO, effectId);
            }
        }

        public void RemoveEffectModifiers(string effectId)
        {
            foreach (var status in _stats.Values)
            {
                status.RemoveModifiersByOwner(effectId);
            }
        }

        public void TriggerFlowEvent(ETurnFlowEvent flowEvent)
        {
            foreach (var status in _stats.Values)
            {
                status.Trigger(flowEvent);
            }
        }

        public void SetCurrentValue(EStatusType statType, float newCurrentValue, bool clampMax = false)
        {
            if (TryGetStatus(statType, out InCombatStatus status))
            {
                status.SetCurrentValue(newCurrentValue, clampMax);
            }
        }

        public void SetMaxValue(EStatusType statType, float newMaxValue)
        {
            if (TryGetStatus(statType, out InCombatStatus status))
            {
                status.SetMaxValue(newMaxValue);
            }
        }
    }
}