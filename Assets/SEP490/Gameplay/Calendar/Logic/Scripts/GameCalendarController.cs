namespace SEP490G69.Calendar
{
    using SEP490G69.GameSessions;
    using UnityEngine;

    public class GameCalendarController : MonoBehaviour, ISceneContext
    {
        private readonly string[] MONTH_NAMES = {
            "Jan", "Feb", "Mar", "Apr", "May", "Jun",
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };

        private GameSessionDAO _sessionDAO;
        private PlayerTrainingSession _currentSesssion;
        private CalendarSO _calendarConfig;
        private EventManager _eventManager;

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);

            _sessionDAO = new GameSessionDAO(LocalDBInitiator.GetDatabase());
            _calendarConfig = ContextManager.Singleton.GetDataSO<CalendarSO>();
            _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            _currentSesssion = _sessionDAO.GetSession(sessionId);

            if (_currentSesssion == null)
            {
                return;
            }
        }
        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
        }

        private void Start()
        {
            _sessionDAO = new GameSessionDAO(LocalDBInitiator.GetDatabase());
            _calendarConfig = ContextManager.Singleton.GetDataSO<CalendarSO>();
            _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            _currentSesssion = _sessionDAO.GetSession(sessionId);  

            if (_currentSesssion == null)
            {
                return;
            }
        }

        public void GoToNextWeek()
        {
            if (_currentSesssion == null)
            {
                return;
            }

            try
            {
                if (_currentSesssion.CurrentWeek < _calendarConfig.GetTotalWeeks() - 1)
                {
                    _currentSesssion.CurrentWeek++;
                    _sessionDAO.UpdateSession(_currentSesssion);

                    _eventManager.Publish(new NextWeekEvent
                    {
                        Week = _currentSesssion.CurrentWeek,
                        IsCheckpoint = _currentSesssion.CurrentWeek % 144 == 0,
                        IsFinal = false
                    });
                }
                else
                {
                    Debug.Log("Final week reached.");
                    _eventManager.Publish(new NextWeekEvent
                    {
                        Week = _currentSesssion.CurrentWeek,
                        IsCheckpoint = _currentSesssion.CurrentWeek % 48 == 0, // End a year.
                        IsFinal = true,
                    });
                }
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        public string GetCalendarTime()
        {
            if (_currentSesssion == null) return "No Session";

            int totalWeeks = _currentSesssion.CurrentWeek;
            Debug.Log($"Current week: {_currentSesssion.CurrentWeek}");

            // Calculate based on 0-based of CurrentWeek
            // Year (0, 1, 2)
            int yearIndex = totalWeeks / 48;
            // Month in year (0 - 11)
            int monthIndex = (totalWeeks % 48) / 4;
            // Week in month (1 - 4)
            int weekInMonth = (totalWeeks % 4) + 1;

            // Format: Week 1 Jan 1st Year
            string yearSuffix = GetOrdinalSuffix(yearIndex + 1);
            string monthName = MONTH_NAMES[monthIndex];

            return $"Week {weekInMonth}\n{monthName} {yearIndex + 1}{yearSuffix} Year";
        }

        public int GetRemainTimeOfYear()
        {
            if (_currentSesssion == null)
            {
                return -1;
            }
            return 48 - (_currentSesssion.CurrentWeek + 1);
        }

        private string GetOrdinalSuffix(int number)
        {
            switch (number)
            {
                case 1: return "st";
                case 2: return "nd";
                case 3: return "rd";
                default: return "th";
            }
        }
    }

    public class NextWeekEvent : IEvent
    {
        public int Week { get; set; }
        public bool IsCheckpoint { get; set; }
        public bool IsFinal { get; set; }
    }
}