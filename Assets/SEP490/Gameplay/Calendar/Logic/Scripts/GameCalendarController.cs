namespace SEP490G69.Calendar
{
    using SEP490G69.Addons.Localization;
    using SEP490G69.GameSessions;
    using SEP490G69.Tournament;
    using SEP490G69.Training;
    using UnityEngine;

    public class GameCalendarController : MonoBehaviour, ISceneContext
    {
        private readonly string[] MONTH_NAME_KEYS = {
            "month_jan", "month_feb", "month_mar", "month_apr", "month_may", "month_jun",
            "month_jul", "month_aug", "month_sep", "month_oct", "month_nov", "month_dec"
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
        public TournamentConfigSO TournamentConfig
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

        private LocalizationManager _localizeManager;
        protected LocalizationManager LocalizationManager
        {
            get
            {
                if (_localizeManager == null)
                {
                    _localizeManager = ContextManager.Singleton.ResolveGameContext<LocalizationManager>();
                }
                return _localizeManager;
            }
        }

        protected PlayerTrainingSession CurrentSession
        {
            get
            {
                if (_currentSesssion == null)
                {
                    string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
                    _currentSesssion = _sessionDAO.GetById(sessionId);
                }
                return _currentSesssion;
            }
        }

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);

            _sessionDAO = new GameSessionDAO();
            _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            _currentSesssion = _sessionDAO.GetById(sessionId);

            if (_currentSesssion == null)
            {
                return;
            }

            _eventManager.Subscribe<TrainingCompletedEvent>(HandleTrainingCompleteEvent);
            _eventManager.Subscribe<EndTournamentEvent>(HandleEndTournamentEvent);
        }

        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
            _eventManager.Unsubscribe<TrainingCompletedEvent>(HandleTrainingCompleteEvent);
            _eventManager.Unsubscribe<EndTournamentEvent>(HandleEndTournamentEvent);
        }

        private void Start()
        {
            _sessionDAO = new GameSessionDAO();
            _calendarConfig = ContextManager.Singleton.GetDataSO<CalendarSO>();
            _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            _currentSesssion = _sessionDAO.GetById(sessionId);  

            if (_currentSesssion == null)
            {
                Debug.Log("Current session data is null");
            }
        }

        private void HandleTrainingCompleteEvent(TrainingCompletedEvent trainingCompletedEvent)
        {
            float fadeDur = 1f;
            float inFadeDur = 1f;
            FadingController.Singleton.FadeIn2Out(fadeDur, inFadeDur, LocalizationManager.GetText(GameConstants.LOCALIZE_CATEGORY_UI_MESSAGE, "msg_new_week_start"), () =>
            {
                TrainingController.HideTrainingMenuBG();
                TrainingController.OpenMainMenuBG();
                GameUIManager.Singleton.HideFrame(GameConstants.FRAME_ID_TRAINING_MENU);
                GoToNextWeek(true);
                GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_MAIN_MENU);
            });
        }

        private void HandleEndTournamentEvent(EndTournamentEvent ev)
        {
            float fadeDur = 1f;
            float inFadeDur = 1f;
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

            PlayerTrainingSession sessionData = _sessionDAO.GetById(sessionId);

            if (sessionData == null)
            {
                return;
            }

            if (IsMandatoryTournament(sessionData.ActiveTournamentId, out string error))
            {
                // Check if the player has passed the tournament objective.

                // If the player failed to pass the tournament objective -> game over.

                // If the player success in passing the tournament objective -> continue/victory.
            }
            else
            {
                // If the function return false and has error message -> an error occurs, not a non-mandatory tournament.
                if (!string.IsNullOrEmpty(error))
                {
                    return;
                }
            }

            string sessionTournamentId = sessionData.ActiveTournamentId;

            FadingController.Singleton.FadeIn2Out(fadeDur, inFadeDur, LocalizationManager.GetText(GameConstants.LOCALIZE_CATEGORY_UI_MESSAGE, "msg_new_week_start"), () =>
            {
                TrainingController.HideTrainingMenuBG();
                TrainingController.OpenMainMenuBG();
                GameUIManager.Singleton.HideFrame(GameConstants.FRAME_ID_TRAINING_MENU);
                GoToNextWeek(true);
                GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_MAIN_MENU);
            });
        }

        public void GoToNextWeek(bool saveToDB = true)
        {
            if (_currentSesssion == null)
            {
                Debug.Log("Current session data is null");

                return;
            }

            try
            {
                if (_currentSesssion.CurrentWeek < _calendarConfig.GetTotalWeeks() - 1)
                {
                    _currentSesssion.CurrentWeek++;

                    if (saveToDB)
                    {
                        LocalDBOrchestrator.UpdateDBChangeTime();

                        _sessionDAO.Update(_currentSesssion);
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

        public int GetCurrentWeekInt()
        {
            if (_currentSesssion == null)
            {
                return -1;
            }
            return _currentSesssion.CurrentWeek;
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

            string monthNameKey = MONTH_NAME_KEYS[monthIndex];
            string monthName = LocalizationManager.GetText(GameConstants.LOCALIZE_CATEGORY_MONTH_NAMES, monthNameKey);

            string yearSuffix = GetOrdinalSuffix(yearIndex + 1);
            Debug.Log($"Year: {yearIndex + 1}{yearSuffix}");

            string weekStr = LocalizationManager.GetText(GameConstants.LOCALIZE_CATEGORY_UI_MESSAGE, "msg_week");
            string yearStr = LocalizationManager.GetText(GameConstants.LOCALIZE_CATEGORY_UI_MESSAGE, "msg_year");

            return $"{weekStr} {weekInMonth}\n{monthName} {yearStr} {yearIndex + 1}";
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

        public bool TryEnterTournament(string tournamentId, out string result)
        {
            result = string.Empty;
            if (_currentSesssion == null)
            {
                result = "Current session model is null";
                return false;
            }
            CalendarWeekSO weekSO = GetWeekData(_currentSesssion.CurrentWeek + 1);
            if (weekSO == null)
            {
                result = $"Calendar week SO {_currentSesssion.CurrentWeek} is not registered";
                return false;
            }

            TournamentSO tournamentSO = weekSO.GetTournamentById(tournamentId);
            if (tournamentSO == null)
            {
                result = $"error: week {_currentSesssion.CurrentWeek} does not have tournament {tournamentId}";
                return false;
            }
            PlayerPrefs.SetString(GameConstants.PREF_KEY_TOURNAMENT_ID, tournamentId);
            return true;
        }

        public bool IsMandatoryTournament(string sessionTournamentId, out string error)
        {
            error = string.Empty;
            string rawTournamentId = EntityIdConstructor.ExtractRawEntityId(sessionTournamentId);
            if (string.IsNullOrEmpty(rawTournamentId))
            {
                error = $"Failed to extract raw tournament id from {sessionTournamentId}";
                Debug.LogError($"[GameCalendarController.IsMandatoryTournament fatal error] Failed to extract raw tournament id from {sessionTournamentId}");
                return false;
            }
            TournamentSO tournament = TournamentConfig.GetTournamentById(rawTournamentId);
            if (tournament == null)
            {
                error = $"TournamentSO with id {sessionTournamentId} does not exist!";

                Debug.LogError($"[GameCalendarController.IsMandatoryTournament fatal error] TournamentSO with id {sessionTournamentId} does not exist!");
                return false;
            }
            return tournament.IsCheckpointTournament;
        }

        /// <summary>
        /// Get the next objective.
        /// Ex: Current week = 4
        /// Next mandatory tournament is in week 10 -> get that tournament's first objective
        /// </summary>
        /// <returns></returns>
        public void GetNextObjective(out TournamentObjectiveSO objective, out TournamentConditionSO condition, out int remainWeeks)
        {
            objective = null;
            condition = null;
            remainWeeks = 0;

            if (CurrentSession == null)
            {
                return;
            }

            int currentWeek = CurrentSession.CurrentWeek + 1;
            int checkpointWeek = currentWeek;

            TournamentSO tournament = null;
            CalendarWeekSO checkpointWeekSO = null;

            foreach (var week in CalendarConfig.Weeks)
            {
                if (week.Week < currentWeek)
                    continue;

                tournament = week.GetMandatoryTournament();

                if (tournament == null)
                {
                    continue;
                }

                checkpointWeekSO = week;
                break;
            }

            if (tournament == null)
            {
                Debug.Log("[GameCalendarController.GetNextObjective] No checkpoint tournament existed");
                return;
            }

            checkpointWeek = checkpointWeekSO.Week;
            remainWeeks = checkpointWeek - currentWeek;
            Debug.Log($"Next checkpoint tournament: {tournament.TournamentId}");
            objective = tournament.Objectives.Count > 0 ? tournament.Objectives[0] : null;
            condition = tournament.EntryConditions.Count > 0 ? tournament.EntryConditions[0] : null;
        }

        public TournamentObjectiveSO GetTournamentObjective(string rawTournamentId)
        {
            if (string.IsNullOrEmpty(rawTournamentId))
            {
                return null;
            }
            return TournamentConfig.GetTournamentById(rawTournamentId).Objectives[0];
        }
    }

    public class NextWeekEvent : IEvent
    {
        public int Week { get; set; }
        public bool IsCheckpoint { get; set; }
        public bool IsFinal { get; set; }
    }
}