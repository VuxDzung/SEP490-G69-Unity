namespace SEP490G69.Calendar
{
    using UnityEngine;
    [CreateAssetMenu(fileName = "CalendarWeek_", menuName = OrganizationConstants.NAMESPACE + "/Calendar/Calendar week")]
    public class CalendarWeekSO : ScriptableObject
    {
        [SerializeField] private int m_Week;

        public int Week => m_Week;
    }
}