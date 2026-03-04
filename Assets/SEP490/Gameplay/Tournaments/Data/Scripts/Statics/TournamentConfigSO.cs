namespace SEP490G69.Tournament
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "TournamentConfig", menuName = OrganizationConstants.NAMESPACE + "/Tournaments/Tournament config")]
    public class TournamentConfigSO : ScriptableObject
    {
        [SerializeField] private List<TournamentSO> m_Tournaments;

        public IReadOnlyList<TournamentSO> Tournaments => m_Tournaments;

        public TournamentSO GetTournamentById(string id)
        {
            if (m_Tournaments == null || m_Tournaments.Count == 0)
            {
                return null;
            }
            return m_Tournaments.FirstOrDefault(t => t.TournamentId.Equals(id));
        }
    }
}