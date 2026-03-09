namespace SEP490G69.Calendar
{
    using SEP490G69.Tournament;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    [CreateAssetMenu(fileName = "CalendarWeek_", menuName = OrganizationConstants.NAMESPACE + "/Calendar/Calendar week")]
    public class CalendarWeekSO : ScriptableObject
    {
        [SerializeField] private int m_Week;
        [SerializeField] private List<TournamentSO> m_Tournaments;

        public int Week => m_Week;
        public IReadOnlyList<TournamentSO> Tournaments => m_Tournaments;

        public TournamentSO GetTournamentById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            if (m_Tournaments.Count == 0)
            {
                return null;
            }

            return m_Tournaments.FirstOrDefault(t => t.TournamentId.Equals(id));
        }
    }
}