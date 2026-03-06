namespace SEP490G69.Calendar
{
    using SEP490G69.Tournament;
    using System.Collections.Generic;
    using UnityEngine;
    [CreateAssetMenu(fileName = "CalendarWeek_", menuName = OrganizationConstants.NAMESPACE + "/Calendar/Calendar week")]
    public class CalendarWeekSO : ScriptableObject
    {
        [SerializeField] private int m_Week;
        [SerializeField] private List<TournamentSO> m_Tournaments;

        public int Week => m_Week;
        public IReadOnlyList<TournamentSO> Tournaments => m_Tournaments;
    }
}