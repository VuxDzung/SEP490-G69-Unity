namespace SEP490G69.Calendar
{
    using SEP490G69.Tournament;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "CalendarSO", menuName = OrganizationConstants.NAMESPACE + "/Calendar/Calendar config")]
    public class CalendarSO : ScriptableObject
    {
        [SerializeField] private List<CalendarWeekSO> m_Week;

        public CalendarWeekSO GetWeek(int week)
        {
            return m_Week.FirstOrDefault(w => w.Week == week);
        }

        public int GetTotalWeeks()
        {
            return m_Week.Count;
        }


        /// <summary>
        /// Get the next mandatory tournament
        /// Ex:
        /// - Current week = 10
        /// - Mandatory tournament is at week 20 -> get that tournament
        /// - Mandatory tournament is at week 8 -> skip.
        /// </summary>
        /// <param name="currentWeek"></param>
        /// <returns></returns>
        public TournamentSO GetNextForcedTournament(int currentWeek)
        {
            TournamentSO nextTournament = null;
            int minWeek = int.MaxValue;

            foreach (CalendarWeekSO week in m_Week)
            {
                if (week.Week < currentWeek)
                {
                    continue;
                }
                foreach (var tournament in week.Tournaments)
                {
                    if (!tournament.IsMandatory)
                        continue;

                    if (week.Week < minWeek)
                    {
                        minWeek = week.Week;
                        nextTournament = tournament;
                    }
                }
            }
            return nextTournament;
        }
    }
}