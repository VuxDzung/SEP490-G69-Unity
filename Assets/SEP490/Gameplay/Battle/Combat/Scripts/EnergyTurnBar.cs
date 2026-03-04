namespace SEP490G69.Battle.Combat
{
    using System;
    using UnityEngine;

    public class EnergyTurnBar 
    {
        public Action OnBarFilled;
        public float CurrentValue { get; private set; }
        public float MaxValue { get; private set; } = 100f;

        private IChargeStrategy chargeStrategy;

        public bool IsFull => CurrentValue >= MaxValue;

        public EnergyTurnBar(IChargeStrategy strategy)
        {
            this.chargeStrategy = strategy;
        }

        public void Update(float deltaTime)
        {
            if (IsFull)
            {
                return;
            }

            float amount = chargeStrategy.ChargeEnergy(deltaTime);
            CurrentValue += amount;
            CurrentValue = Mathf.Clamp(CurrentValue, 0, MaxValue);

            if (IsFull)
            {
                OnBarFilled?.Invoke();
            }
        }

        public void Reset()
        {
            CurrentValue = 0;
        }

        public void SetStrategy(IChargeStrategy strategy)
        {
            chargeStrategy = strategy;
        }
    }
}