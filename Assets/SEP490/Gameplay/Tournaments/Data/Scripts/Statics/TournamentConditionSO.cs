namespace SEP490G69.Tournament
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "TournamentCondition_", menuName = OrganizationConstants.NAMESPACE + "/Tournaments/Tournament condition data")]
    public class TournamentConditionSO : ScriptableObject
    {
        [SerializeField] private string m_ConditionId;
        [SerializeField] private string m_ConditionName;
        [SerializeField] private string m_TournamentDesc;
        [SerializeField] private bool m_DisplayToUI;
        [SerializeField] private List<ConditionParamData> m_EntryParams;

        public bool DisplayToUI => m_DisplayToUI;
        public string ConditionId => m_ConditionId.Trim();
        public string ConditionName => m_ConditionName.Trim();
        public string ConditionDesc => m_TournamentDesc.Trim();
        public IReadOnlyList<ConditionParamData> EntryParams => m_EntryParams;
    }

    [System.Serializable]
    public struct ConditionParamData
    {
        [SerializeField] private EStatusType m_StatusType;
        [SerializeField] private float m_RequiredValue;

        public EStatusType StatusType => m_StatusType;
        public float RequiredValue => m_RequiredValue;
    }
}