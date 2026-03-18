namespace SEP490G69.Legacy
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "LegacyStat_", menuName = OrganizationConstants.NAMESPACE + "/Legacy/Legacy stat data")]
    public class LegacyStatSO : ScriptableObject
    {
        [SerializeField] private string m_LegacyStatId;
        [SerializeField] private EStatusType m_StatType;
        [SerializeField] private int m_BaseValue;
        [SerializeField] private int m_BonusPerLevel;
        [SerializeField] private string m_LegacyName;
        [SerializeField] private string m_LegacyDesc;
        [SerializeField] private Sprite m_Icon;
        [SerializeField] private int m_BaseCost;
        [SerializeField] private int m_CostPerLevel;

        public string LegacyStatId => m_LegacyStatId;
        public EStatusType StatType => m_StatType;
        public int BaseValue => m_BaseValue;
        public int BonusPerLevel => m_BonusPerLevel;
        public string LegacyName => m_LegacyName;
        public string LegacyDesc => m_LegacyDesc;
        public Sprite Icon => m_Icon;
        public int BaseCost => m_BaseCost;
        public int CostPerLevel => m_CostPerLevel;
    }
}