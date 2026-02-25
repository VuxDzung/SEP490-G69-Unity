namespace SEP490G69
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "StatusMod_", menuName = OrganizationConstants.NAMESPACE + "/Status/Status modifier data")]
    public class StatusModifierSO : ScriptableObject
    {
        [SerializeField] private string m_Id;
        [SerializeField] private EStatusType m_StatType;
        [SerializeField] private EOperator m_Operator;
        [SerializeField] private float m_Value;
        [SerializeField] private float m_BonusPerLevel;

        [Header("Dev notation")]
        [TextArea]
        [SerializeField] private string m_Description;

        public string Id => m_Id;
        public EStatusType StatType => m_StatType;
        public EOperator Operator => m_Operator;
        public float Value => m_Value;
        public float BonusPerLevel => m_BonusPerLevel;
        public string Description => m_Description;

        public float GetModifierValue(float targetValue)
        {
            switch(m_Operator)
            {
                case EOperator.PercentAdd:
                    return targetValue + targetValue * m_Value;
                case EOperator.PercentSub:
                    return targetValue - targetValue * m_Value;
                case EOperator.FlatAdd:
                    return targetValue + m_Value;
                case EOperator.FlatSub:
                    return targetValue - m_Value;
            }
            return targetValue;
        }
    }
}