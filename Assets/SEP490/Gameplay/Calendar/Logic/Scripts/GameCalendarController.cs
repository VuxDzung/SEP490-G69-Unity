namespace SEP490G69.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private TournamentProgressDAO _tournamentDAO;
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
            _tournamentDAO = new TournamentProgressDAO();
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

            CheckPendingTournamentResult(sessionId, out bool needCreateSnapshot, () =>
            {
                float fadeDur = 1f;
                float inFadeDur = 1f;
                GameUIManager.Singleton.HideFrame(GameConstants.FRAME_ID_MAIN_MENU);

                FadingController.Singleton.FadeIn2Out(fadeDur, inFadeDur, LocalizationManager.GetText(GameConstants.LOCALIZE_CATEGORY_UI_MESSAGE, "msg_new_week_start"), () =>
                {
                    TrainingController.OpenMainMenuBG();
                    GoToNextWeek(true);
                    GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_MAIN_MENU);
                });
            }, () => { });

            if (needCreateSnapshot == true)
            {
                // Create snapshot here.
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

        /// <summary>
        /// If the tournament is a checkpoint tournament, check whether the player has passed the tournament objective or not.
        ///     If the player failed to complete the objective, show a notification which notifies that the player has lost.
        ///     If the player pass the objective, check whether that is the final tournament or not.
        ///         If it's the final tournament, pending the player to prepare to graduate.
        ///         If not, go to next week and create a snapshot for that week.
        ///         
        /// This method needs to call when the player end the tournament and when the player enters the Main Menu scene when he/she enters from the Title screen in case the player finishs the tournament and close the app.
        /// </summary>
        /// <param name="ev"></param>
        private void HandleEndTournamentEvent(EndTournamentEvent ev)
        {
            //float fadeDur = 1f;
            //float inFadeDur = 1f;
            //string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

            //CheckPendingTournamentResult(sessionId, out bool needToCreateSnapshot, () => { }, () => { });

            //FadingController.Singleton.FadeIn2Out(fadeDur, inFadeDur, LocalizationManager.GetText(GameConstants.LOCALIZE_CATEGORY_UI_MESSAGE, "msg_new_week_start"), () =>
            //{
            //    TrainingController.HideTrainingMenuBG();
            //    TrainingController.OpenMainMenuBG();
            //    GameUIManager.Singleton.HideFrame(GameConstants.FRAME_ID_TRAINING_MENU);
            //    GoToNextWeek(true);

            //    if (needToCreateSnapshot == true)
            //    {
            //        // Create snapshot here.
            //    }

            //    GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_MAIN_MENU);
            //});
        }

        private void CheckPendingTournamentResult(string sessionId, out bool needCreateSnapshot, Action onTournamentDeleted, Action onCancelDeleteTournament)
        {
            needCreateSnapshot = false;

            _currentSesssion = _sessionDAO.GetById(sessionId);

            if (_currentSesssion == null)
            {
                Debug.Log($"<color=red>[GameCalendarController.HandleEndTournamentEvent error]</color> No session data exist with id {_currentSesssion.SessionId}");
                return;
            }

            // Only clear the active tournament if the logic does not have any error or
            // it does not have any pending condition such as lose the checkpoint tournament or pass the final tournament.
            bool canClearActiveTournament = DetermineTournamentResult(_currentSesssion, out bool needToCreateSnapshot);

            // Delete the tournament progression and continue to next week.
            if (canClearActiveTournament)
            {
                Debug.Log("Can delete tournament data");
                DeleteActiveTournamentData(_currentSesssion);
                onTournamentDeleted?.Invoke();
            }
            else
            {
                Debug.Log("No deletation is allowed.");
                onCancelDeleteTournament?.Invoke();
            }
        }

        private bool DetermineTournamentResult(PlayerTrainingSession sessionData, out bool needToCreateSnapshot)
        {
            needToCreateSnapshot = false;

            if (string.IsNullOrEmpty(sessionData.ActiveTournamentId))
            {
                return false;
            }

            if (IsMandatoryTournament(sessionData.ActiveTournamentId, out string error))
            {
                // Check if the player has passed the tournament objective.

                // If the player failed to pass the tournament objective -> game over.

                // If the player success in passing the tournament objective -> continue/victory.

                bool objectiveCompleted = false;

                TournamentProgressData tournamentData = _tournamentDAO.GetById(sessionData.ActiveTournamentId);
                if (tournamentData == null)
                {
                    Debug.LogError($"[GameCalendarController.HandleEndTournamentEvent error] No tournament data exist with id {sessionData.ActiveTournamentId}");
                    return false;
                }
                TournamentSO tournamentSO = TournamentConfig.GetTournamentById(tournamentData.RawTournamentId);
                if (tournamentSO == null)
                {
                    Debug.LogError($"[GameCalendarController.HandleEndTournamentEvent error] No tournament SO exist with id {tournamentData.RawTournamentId}");
                    return false;
                }
                TournamentObjectiveSO objectiveSO = tournamentSO.Objectives.Count > 0 ? tournamentSO.Objectives[0] : null;

                int playerTournamentPlace = 1;
                string sessionCharId = EntityIdConstructor.ConstructDBEntityId(sessionData.SessionId, sessionData.RawCharacterId);

                if (tournamentData.IsPlayerWon)
                {
                    playerTournamentPlace = 1;
                }
                else if (GetPlayerTournamentData(tournamentData.FinalParticipants, sessionCharId) != null) // Lose final, win semi-final.
                {
                    playerTournamentPlace = 2;
                }
                else if (GetPlayerTournamentData(tournamentData.SemiFinalParticipants, sessionCharId) != null) // Lose semi-final, win elimination
                {
                    playerTournamentPlace = 3;
                }
                else // Lose at the elimination round.
                {
                    playerTournamentPlace = 4;
                }

                if (objectiveSO != null)
                {
                    if (objectiveSO.ObjectiveParam == EObjectiveParam.TournamentPlace &&
                        playerTournamentPlace <= objectiveSO.RequiredAmount)
                    {
                        // Pass the objective
                        objectiveCompleted = true;
                        needToCreateSnapshot = true;
                    }
                    else
                    {
                        // Lose the objective
                        objectiveCompleted = false;
                    }
                }

                if (objectiveCompleted == false)
                {
                    // Show a notification here.
                    // Message: You have failed the checkpoint tournament. Do you want to rollback to the previous checkpoint?
                    return false; // Pending: (Clear tournament progress data when the player press Rollback)
                }
            }
            else if (!string.IsNullOrEmpty(error)) // If the function return false and has error message -> an error occurs, not a non-mandatory tournament.
            {
                Debug.LogError($"[GameCalendarController.HandleEndTournamentEvent error] {error}");
                return false;
            }

            if (sessionData.CurrentWeek + 1 >= CalendarConfig.GetTotalWeeks())
            {
                // Final week reached.
                // Graduate here.
                GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_PENDING_GRADUATE);
                return false; // Pending (Clear tournament progress data when the player press Graduate)
            }

            return true;
        }

        private void DeleteActiveTournamentData(PlayerTrainingSession sessionData)
        {
            if (sessionData == null)
            {
                return;
            }

            _tournamentDAO.Delete(sessionData.ActiveTournamentId);

            sessionData.ActiveTournamentId = string.Empty;
            _sessionDAO.Update(sessionData);
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

        public TournamentParticipantData GetPlayerTournamentData(List<TournamentParticipantData> participants, string sessionCharacterId)
        {
            if (participants == null || 
                participants.Count == 0 || 
                string.IsNullOrEmpty(sessionCharacterId))
            {
                return null;
            }
            return participants.FirstOrDefault(par => par.Id == sessionCharacterId);    
        }
    }

    public class NextWeekEvent : IEvent
    {
        public int Week { get; set; }
        public bool IsCheckpoint { get; set; }
        public bool IsFinal { get; set; }
    }
}