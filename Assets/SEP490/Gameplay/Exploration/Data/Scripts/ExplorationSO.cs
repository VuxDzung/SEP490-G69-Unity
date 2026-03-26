namespace SEP490G69.Exploration
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ExplorationSO", menuName = OrganizationConstants.NAMESPACE + "/Exloration/Exploration data")]
    public class ExplorationSO : ScriptableObject
    {
        [SerializeField] private string explorationId;
        [SerializeField] private string m_LocationName;
        [SerializeField] private EDifficulty m_Difficulty;
        [SerializeField] private string m_BossId;
        [SerializeField] private Material m_ScrollableMat;

        [SerializeField] private List<RewardDataSO> m_PossibleRewards;

        public string ExplorationId => explorationId;
        public string LocationName => m_LocationName;
        public EDifficulty Difficulty => m_Difficulty;
        public string BossId => m_BossId;
        public Material ScrollableMat => m_ScrollableMat;
        public IReadOnlyList<RewardDataSO> PossibleRewards => m_PossibleRewards;
    }

    [System.Serializable]
    public class ExploreEventData
    {
        [SerializeField] private string m_EventId;
        [Range(0, 1f)]
        [SerializeField] private float m_AppearsPercent;
    }
}