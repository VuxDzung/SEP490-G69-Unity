namespace SEP490G69.Calendar
{
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
    }
}