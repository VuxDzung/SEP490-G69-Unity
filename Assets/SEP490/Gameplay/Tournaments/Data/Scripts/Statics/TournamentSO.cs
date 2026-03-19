namespace SEP490G69.Tournament
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "TournamentSO_", menuName = OrganizationConstants.NAMESPACE + "/Tournaments/Tournament data")]
    public class TournamentSO : ScriptableObject
    {
        [SerializeField] private string tournamentId;
        [SerializeField] private string tournamentName;
        [SerializeField] private bool m_IsCheckpointTournament;
        [SerializeField] private bool m_IsMandatory;
        [SerializeField] private string[] EnemyIdArray;

        [Tooltip("The entry condition which the player's character must met to participate in the tournament")]
        [SerializeField] private List<TournamentConditionSO> m_EntryConditions;
        [Tooltip("Objective(s) which the player must complete to continue game progression")]
        [SerializeField] private List<TournamentObjectiveSO> m_Objectives;

        [SerializeField] private List<RewardDataSO> m_ChampionRewards;
        [SerializeField] private List<RewardDataSO> m_SemiFinalRewards;
        [SerializeField] private List<RewardDataSO> m_EliminationRewards;

        public string TournamentId => tournamentId;
        public string Name => tournamentName;
        public bool IsCheckpointTournament => m_IsCheckpointTournament;
        public string[] EnemyIds => EnemyIdArray;
        public bool IsMandatory => m_IsMandatory;

        public IReadOnlyList<TournamentConditionSO> EntryConditions => m_EntryConditions;
        public IReadOnlyList<TournamentObjectiveSO> Objectives => m_Objectives;

        public IReadOnlyList<RewardDataSO> ChampionRewards => m_ChampionRewards;
        public IReadOnlyList<RewardDataSO> SemiFinalRewards => m_SemiFinalRewards;
        public IReadOnlyList<RewardDataSO> EliminationRewards => m_EliminationRewards;
    }

    [System.Serializable]
    public struct RewardRankData
    {
        [SerializeField] private int m_Rank;
        [SerializeField] private List<RewardDataSO> m_Rewards;
        
        public int Rank => m_Rank;
        public IReadOnlyList<RewardDataSO> Rewards => m_Rewards;
    }
}