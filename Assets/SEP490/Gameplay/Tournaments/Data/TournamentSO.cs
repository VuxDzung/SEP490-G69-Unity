namespace SEP490G69
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "TournamentSO_", menuName = OrganizationConstants.NAMESPACE + "/Tournaments/Tournament data")]
    public class TournamentSO : ScriptableObject
    {
        [SerializeField] private string tournamentId;
        [SerializeField] private string tournamentName;
        [SerializeField] private string rank;
        [SerializeField] private int goldGain;
        [SerializeField] private int rpGain;

        [SerializeField] private string[] EnemyIdArray;
        
        public string TournamentId => tournamentId;
        public string Name => tournamentName;
        public string Rank => rank;
        public int GoldGain => goldGain;
        public int RpGain => rpGain;
    }
}