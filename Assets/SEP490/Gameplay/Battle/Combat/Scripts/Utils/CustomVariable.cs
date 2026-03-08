namespace SEP490G69
{
    using SEP490G69.Battle.Combat;
    using System;
    using UnityEngine;

    [System.Serializable]
    public class CustomVariable : InspectorPrimtiveVariable
    {
        [SerializeField] private EStatusType m_BaseStatus;
        [SerializeField] private EOperator m_Operator;

        /// <summary>
        /// Represent the base input to calculate the value.
        /// </summary>
        public EStatusType BaseStatys => m_BaseStatus;

        public float GetFinalValue(BaseBattleCharacterController statusReader)
        {
            // 1. Base value from character
            float baseValue = statusReader.GetCombatStatus(m_BaseStatus).Value;

            // 2. Config value from inspector
            float configValue = Convert.ToSingle(GetValue());

            // 3. Apply operator
            return m_Operator switch
            {
                EOperator.PercentAdd => baseValue + baseValue * configValue,
                EOperator.PercentSub => baseValue - baseValue * configValue,
                EOperator.FlatAdd => baseValue + configValue,
                EOperator.FlatSub => baseValue - configValue,
                EOperator.Mul => baseValue * configValue,
                EOperator.Set => configValue,
                _ => baseValue
            };
        }

        public float GetDeltaValue(BaseBattleCharacterController statusReader)
        {
            // 1. Base value from character
            float baseValue = statusReader.GetCombatStatus(m_BaseStatus).Value;

            // 2. Config value from inspector
            float configValue = Convert.ToSingle(GetValue());

            // 3. Apply operator
            return m_Operator switch
            {
                EOperator.PercentAdd => baseValue * configValue,
                EOperator.PercentSub => - baseValue * configValue,
                EOperator.FlatAdd => configValue,
                EOperator.FlatSub => - configValue,
                EOperator.Set => configValue,
                _ => baseValue
            };
        }
    }
}