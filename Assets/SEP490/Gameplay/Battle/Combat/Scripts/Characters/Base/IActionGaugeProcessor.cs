namespace SEP490G69.Battle.Combat
{
    using System;
    public interface IActionGaugeProcessor
    {
        /// <summary>
        /// Triggers when the energy bar is full.
        /// </summary>
        public event Action onEnergyFull;

        public void InitializeActionGauge(BaseBattleCharacterController owner, InCombatStatus agilityValue);
        public void UpdateActionGauge(float deltaTime);
        public void ResetActionGauge();
        public float GetCurrentGaugeValue();
        public float GetMaxGaugeValue();
        public void Pause();
        public void Unpause();

        void QueueGaugeModifier(CombatStatModifierSO modifier, bool immedite);
    }
}