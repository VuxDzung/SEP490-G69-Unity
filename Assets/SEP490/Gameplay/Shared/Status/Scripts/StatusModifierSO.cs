namespace SEP490G69
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "StatusMod_", menuName = OrganizationConstants.NAMESPACE + "/Status/Status modifier data")]
    public class StatusModifierSO : ScriptableObject
    {
        [SerializeField] private EStatusType m_StatType;
        [SerializeField] private EOperator m_Operator;
        [SerializeField] private float m_Value;

        [SerializeField] private float m_BonusPerLevel;

        public EStatusType StatType => m_StatType;
        public EOperator Operator => m_Operator;
        public float Value => m_Value;
        public float BonusPerLevel => m_BonusPerLevel;
    }
}