namespace SEP490G69.Exploration
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ExplorationSO", menuName = OrganizationConstants.NAMESPACE + "/Exloration/Exploration data")]
    public class ExplorationSO : ScriptableObject
    {
        [SerializeField] private string explorationId;
        [SerializeField] private string m_LocationName;
        [SerializeField] private string m_Difficulty;
        [SerializeField] private string m_BossId;

        [SerializeField] private List<RewardDataSO> m_PossibleRewards;

        public string ExplorationId => explorationId;
        public string LocationName => m_LocationName;
        public string Difficulty => m_Difficulty;
        public string BossId => m_BossId;
        public IReadOnlyList<RewardDataSO> PossibleRewards => m_PossibleRewards;
    }
}