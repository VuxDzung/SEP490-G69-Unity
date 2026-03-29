namespace SEP490G69.Battle.Combat
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CharacterActionGaugeProcessor : MonoBehaviour, IActionGaugeProcessor
    {
        /// <summary>
        /// Triggers when the energy bar is full.
        /// </summary>
        public event Action onEnergyFull;

        private BaseBattleCharacterController _owner;
        private EnergyTurnBar _energyTurnBar;
        private bool _paused;

        private readonly List<CombatStatModifierSO> _pendingGaugeModifiers = new List<CombatStatModifierSO>();
        private InCombatStatus _agiStatus;

        public void InitializeActionGauge(BaseBattleCharacterController owner, InCombatStatus agilityStatus)
        {
            _owner = owner;
            _agiStatus = agilityStatus;
            var strategy = new AgiBasedChargeStrategy(GameConstants.BASE_FILL_SPEED);
            _energyTurnBar = new EnergyTurnBar(strategy);
        }

        public float GetCurrentGaugeValue()
        {
            return _energyTurnBar.CurrentValue;
        }

        public float GetMaxGaugeValue()
        {
            return _energyTurnBar.MaxValue; 
        }

        public void Pause()
        {
            _paused = true;
        }

        public void ResetActionGauge()
        {
            _energyTurnBar?.Reset();

            ApplyQueuedGaugeModifiers();
        }

        public void Unpause()
        {
            _paused = false;
        }

        public void UpdateActionGauge(float deltaTime)
        {
            if (_energyTurnBar == null || _paused || _energyTurnBar.IsFull)
                return;

            _energyTurnBar.Update(_agiStatus.Value, deltaTime);

            if (_energyTurnBar.IsFull)
                onEnergyFull?.Invoke();
        }

        public void QueueGaugeModifier(CombatStatModifierSO modifier, bool immedite)
        {
            _pendingGaugeModifiers.Add(modifier);

            if (immedite)
            {
                ApplyQueuedGaugeModifiers();
            }
        }

        private void ApplyQueuedGaugeModifiers()
        {
            if (_pendingGaugeModifiers.Count == 0)
            {
                return;
            }

            float gauge = _energyTurnBar.CurrentValue;

            foreach (var modifier in _pendingGaugeModifiers)
            {
                gauge = modifier.GetModifiedStatus(_energyTurnBar.MaxValue);
            }

            gauge = Mathf.Clamp(gauge, 0, _energyTurnBar.MaxValue);

            _energyTurnBar.SetValue(gauge);

            _pendingGaugeModifiers.Clear();
        }
    }
}