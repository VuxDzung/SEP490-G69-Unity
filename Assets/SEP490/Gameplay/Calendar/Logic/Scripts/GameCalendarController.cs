namespace SEP490G69.Calendar
{
    using SEP490G69.GameSessions;
    using SEP490G69.Tournament;
    using SEP490G69.Training;
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

        private GameTrainingController _trainingController;
        protected GameTrainingController TrainingController
        {
            get
            {
                if (_trainingController == null)
                {
                    ContextManager.Singleton.TryResolveSceneContext(out _trainingController);
                }
                return _trainingController;
            }
        }
        public CalendarSO CalendarConfig
        {
            get
            {
                if ( _calendarConfig == null)
                {
                    _calendarConfig = ContextManager.Singleton.GetDataSO<CalendarSO>();
                }
                return _calendarConfig;
            }
        }

        private TournamentConfigSO _tournamentConfig;
        protected TournamentConfigSO TournamentConfig
        {
            get
            {
                if (_tournamentConfig == null)
                {
                    _tournamentConfig = ContextManager.Singleton.GetDataSO<TournamentConfigSO>();
                }
                return _tournamentConfig;
            }
        }

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);

            _sessionDAO = new GameSessionDAO(LocalDBInitiator.GetDatabase());
            _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            _currentSesssion = _sessionDAO.GetSession(sessionId);

            if (_currentSesssion == null)
            {
                return;
            }

            _eventManager.Subscribe<TrainingCompletedEvent>(HandleTrainingCompleteEvent);
        }
        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
            _eventManager.Unsubscribe<TrainingCompletedEvent>(HandleTrainingCompleteEvent);
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

        private void HandleTrainingCompleteEvent(TrainingCompletedEvent trainingCompletedEvent)
        {
            float fadeDur = 1f;
            float inFadeDur = 1f;
            FadingController.Singleton.FadeIn2Out(fadeDur, inFadeDur, "New week started", () =>
            {
                TrainingController.HideTrainingMenuBG();
                TrainingController.OpenMainMenuBG();
                GameUIManager.Singleton.HideFrame(GameConstants.FRAME_ID_TRAINING_MENU);
                GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_MAIN_MENU);
                GoToNextWeek(false);
            });
        }

        public void GoToNextWeek(bool saveToDB = true)
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

                    if (saveToDB)
                    {
                        _sessionDAO.UpdateSession(_currentSesssion);
                    }

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

        public CalendarWeekSO GetWeekData(int week)
        {
            if (_calendarConfig == null || _calendarConfig.GetTotalWeeks() == 0)
            {
                return null;
            }
            return _calendarConfig.GetWeek(week);
        }

        public string GetCurrentWeek()
        {
            if (_currentSesssion == null) return "No Session";

            int totalWeeks = _currentSesssion.CurrentWeek;

            return (totalWeeks + 1).ToString();
        }
        public string GetCalendarTime()
        {
            if (_currentSesssion == null)
                return "No Session";

            return GetCalendarTime(_currentSesssion.CurrentWeek);
        }
        public string GetCalendarTime(int week)
        {
            if (_currentSesssion == null)
                return "No Session";

            Debug.Log($"Calendar query week: {week}");

            // Year index (0,1,2...)
            int yearIndex = week / 48;

            // Month index (0-11)
            int monthIndex = (week % 48) / 4;

            // Week in month (1-4)
            int weekInMonth = (week % 4) + 1;

            string monthName = MONTH_NAMES[monthIndex];
            string yearSuffix = GetOrdinalSuffix(yearIndex + 1);
            Debug.Log($"Year: {yearIndex + 1}{yearSuffix}");
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

        public int GetTotalWeeks()
        {
            return CalendarConfig.GetTotalWeeks();
        }
    }

    public class NextWeekEvent : IEvent
    {
        public int Week { get; set; }
        public bool IsCheckpoint { get; set; }
        public bool IsFinal { get; set; }
    }
}