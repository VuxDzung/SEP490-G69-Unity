namespace SEP490G69.Tournament
{
    using UnityEngine;

    public enum EObjectiveParam
    {
        None = 0,
        TournamentPlace = 1,
    }

    [CreateAssetMenu(fileName = "TournamentOjt_", menuName = OrganizationConstants.NAMESPACE + "/Tournaments/Objective data")]
    public class TournamentObjectiveSO : ScriptableObject
    {
        [SerializeField] private string m_ObjectiveId;
        [SerializeField] private string m_ObjectiveName;
        [SerializeField] private string m_ObjectiveDescription;
        [SerializeField] private EObjectiveParam m_ObjectiveParam;
        [SerializeField] private float m_RequiredAmount;

        public string ObjectiveId => m_ObjectiveId;
        public string ObjectiveName => m_ObjectiveName;
        public string ObjectiveDesc => m_ObjectiveDescription;
        public EObjectiveParam ObjectiveParam => m_ObjectiveParam;
        public float RequiredAmount => m_RequiredAmount;
    }
}