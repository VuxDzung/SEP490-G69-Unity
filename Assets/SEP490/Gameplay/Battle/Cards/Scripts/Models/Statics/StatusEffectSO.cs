namespace SEP490G69.Battle.Cards
{
    using NUnit.Framework.Constraints;
    using System.Runtime.ConstrainedExecution;
    using UnityEngine;

    [CreateAssetMenu(fileName = "StatusEffect_", menuName = OrganizationConstants.NAMESPACE + "/Battle/Cards/Status effect data")]
    public class StatusEffectSO : ScriptableObject
    {
        [SerializeField] private string m_EffectId;
        [SerializeField] private string m_EffectName;
        [SerializeField] private string m_EffectDesc;
        [SerializeField] private Sprite m_Icon;
        [SerializeField] private ETargetType m_Target;
        [SerializeField] private EApplyDiscardType m_ApplyType;
        [Header("Discard by turn count")]
        [SerializeField] private int m_AliveTurnCount;

        [Header("Modifier value settings")]
        [SerializeField] private EStatusType m_StatType;
        [SerializeField] private EOperator m_Op;
        [SerializeField] private float m_EffectValue;

        public string EffectId => m_EffectId;
        public string EffectName => m_EffectName;
        public string EffectDesc => m_EffectDesc;
        public Sprite Icon => m_Icon;
        public ETargetType TargetType => m_Target;
        public EApplyDiscardType ApplyType => m_ApplyType;
        public int AliveTurnCount => m_AliveTurnCount;  
        public EOperator Op => m_Op;
        public EStatusType StatType => m_StatType;
        public float EffectValue => m_EffectValue;

        public float GetDelta(float targetValue)
        {
            switch (m_Op)
            {
                case EOperator.PercentAdd:
                    return targetValue * m_EffectValue;

                case EOperator.PercentSub:
                    return -targetValue * m_EffectValue;

                case EOperator.FlatAdd:
                    return m_EffectValue;

                case EOperator.FlatSub:
                    return -m_EffectValue;
                case EOperator.Set:
                    return targetValue - m_EffectValue;
            }

            return 0f;
        }
        public float GetModifiedStatus(float targetValue)
        {
            switch (m_Op)
            {
                case EOperator.PercentAdd:
                    return targetValue + targetValue * m_EffectValue;
                case EOperator.PercentSub:
                    return targetValue - targetValue * m_EffectValue;
                case EOperator.FlatAdd:
                    return targetValue + m_EffectValue;
                case EOperator.FlatSub:
                    return targetValue - m_EffectValue;
            }
            return targetValue;
        }
    }
}