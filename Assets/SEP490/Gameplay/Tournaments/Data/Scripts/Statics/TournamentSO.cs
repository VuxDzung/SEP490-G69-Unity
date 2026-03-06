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
        [SerializeField] private string[] EnemyIdArray;

        [Tooltip("The entry condition which the player's character must met to participate in the tournament")]
        [SerializeField] private List<TournamentConditionSO> m_EntryConditions;
        [Tooltip("Objective(s) which the player must complete to continue game progression")]
        [SerializeField] private List<TournamentObjectiveSO> m_Objectives;
        [Tooltip("When the tournament ends, the player shall receive the reward based on the tournament rank/position/top")]
        [SerializeField] private List<RewardRankData> m_RewardRanks;

        public string TournamentId => tournamentId;
        public string Name => tournamentName;
        public bool IsCheckpointTournament => m_IsCheckpointTournament;
        public string[] EnemyIds => EnemyIdArray;
        public IReadOnlyList<RewardRankData> RewardRanks => m_RewardRanks;
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