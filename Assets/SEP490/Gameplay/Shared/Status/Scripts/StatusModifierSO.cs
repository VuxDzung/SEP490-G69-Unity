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
        [Header("Random number")]
        [SerializeField] private bool m_RandomNumber;
        [SerializeField] private float m_Min;
        [SerializeField] private float m_Max;

        [Header("Dev notation")]
        [TextArea]
        [SerializeField] private string m_Description;

        public string Id => m_Id;
        public EStatusType StatType => m_StatType;
        public EOperator Operator => m_Operator;
        public float Value => m_Value;
        public string Description => m_Description;

        public float GetModifiedStatus(float targetValue)
        {
            float value = GetRandomNumber();
            switch (m_Operator)
            {
                case EOperator.PercentAdd:
                    return targetValue + targetValue * value;
                case EOperator.PercentSub:
                    return targetValue - targetValue * value;
                case EOperator.FlatAdd:
                    return targetValue + value;
                case EOperator.FlatSub:
                    return targetValue - value;
            }
            return targetValue;
        }

        public float GetRandomNumber()
        {
            if (m_RandomNumber)
            {
                return Random.Range(m_Min, m_Max);
            }
            return m_Value;
        }
    }
}