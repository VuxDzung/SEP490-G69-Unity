namespace SEP490G69.Tournament
{
    using System.Collections.Generic;
    using UnityEngine;
    using SEP490G69.GameSessions;
    using SEP490G69.Addons.LoadScreenSystem;
    using System.Collections;
    using SEP490G69.Battle.Cards;
    using SEP490G69.Shared;
    using System.Linq;

    public class GameTournamentController : MonoBehaviour, ISceneContext
    {
        #region Inspector

        [Header("Configurations")]
        [SerializeField] private TournamentConfigSO m_TournamentConfig;
        [SerializeField] private CharacterConfigSO m_CharacterConfig;

        [Header("Testing")]
        [SerializeField] private bool m_IsTesting;
        [SerializeField] private string m_TournamentId;
        [SerializeField] private bool m_ClearExistedProgress = false;

        #endregion


        #region DAO References

        private GameSessionDAO _sessionDAO;
        private PlayerCharacterRepository _characterRepo;
        private TournamentProgressDAO _tournamentDAO;

        #endregion


        #region Runtime Data

        private TournamentSO _currentTournamentSO;

        private List<TournamentParticipant> m_CurrentParticipants = new();
        private List<TournamentParticipant> m_CurrentRoundParticipants = new();
        private List<TournamentParticipant> m_SemiFinalParticipants = new();
        private List<TournamentParticipant> m_FinalParticipants = new();
        private int m_CurrentRoundIndex = 0;

        private UITournamentBracketScreen _bracketFrame;

        private string _sessionTournamentId;

        #endregion

        private EventManager _eventManager;


        #region Unity Lifecycle

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);
            _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();
            LoadDAOs();
        }

        private void Start()
        {
            if (m_IsTesting)
            {
                LoadTournamentData(m_TournamentId);
                return;
            }
            string tournamentId = PlayerPrefs.GetString(GameConstants.PREF_KEY_TOURNAMENT_ID);
            if (string.IsNullOrEmpty(tournamentId))
            {
                Debug.LogError($"Tournament id {tournamentId} is not in the cache!");
                return;
            }
            LoadTournamentData(tournamentId);
        }

        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
        }

        #endregion


        // =========================================================
        // INITIALIZATION
        // =========================================================

        #region Initialization

        /// <summary>
        /// Load DAO instances from LiteDB
        /// </summary>
        private void LoadDAOs()
        {
            _sessionDAO = new GameSessionDAO();
            _characterRepo = new PlayerCharacterRepository();
            _tournamentDAO = new TournamentProgressDAO();
        }

        /// <summary>
        /// Entry point to load or create tournament progress
        /// </summary>
        public void LoadTournamentData(string tournamentId)
        {
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            _sessionTournamentId = EntityIdConstructor.ConstructDBEntityId(sessionId, tournamentId);// $"{sessionId}:{tournamentId}";

            TournamentProgressData saved = _tournamentDAO.GetById(_sessionTournamentId);

            if (saved != null)
            {
                if (m_ClearExistedProgress)
                {
                    _tournamentDAO.Delete(_sessionTournamentId);
                }
                else
                {
                    RestoreProgress(saved);

                    if (saved.WaitingForPlayerBattle)
                    {
                        if (saved.IsBattleFinished)
                        {
                            ResumeAfterCombat(saved);
                        }
                        else
                        {
                            StartPlayerBattle(saved.PendingEnemyId);
                        }
                        return;
                    }

                    OpenTournamentBracketUI();
                    UpdateBracketUI();
                    return;
                }
            }

            CreateNewTournament(tournamentId);
        }


        /// <summary>
        /// Restore tournament progress from database
        /// </summary>
        private void RestoreProgress(TournamentProgressData data)
        {
            _currentTournamentSO = m_TournamentConfig.GetTournamentById(data.RawTournamentId);

            m_CurrentRoundIndex = data.CurrentRoundIndex;
            m_SemiFinalParticipants = RestoreParticipants(data.SemiFinalParticipants);
            m_FinalParticipants = RestoreParticipants(data.FinalParticipants);
            m_CurrentParticipants = RestoreParticipants(data.Participants);

            m_CurrentRoundParticipants = RestoreParticipants(data.CurrentRoundParticipants);
        }


        /// <summary>
        /// Convert saved data to runtime participants
        /// </summary>
        private List<TournamentParticipant> RestoreParticipants(List<TournamentParticipantData> data)
        {
            List<TournamentParticipant> list = new List<TournamentParticipant>();

            if (data == null)
            {
                return new List<TournamentParticipant>();
            }

            foreach (var p in data)
            {
                string charId = p.CharacterId;
                Debug.Log($"Character id: {charId}");
                string[] idParts = p.CharacterId.Split(':');
                if (idParts.Length == 2)
                {
                    charId = idParts[1];
                }
                BaseCharacterSO character = m_CharacterConfig.GetCharacterById(charId);
                if (character == null)
                {
                    Debug.LogError($"Character so with id {p.CharacterId} is null");
                }

                list.Add(new TournamentParticipant
                {
                    Id = p.Id,
                    Name = character.CharacterName,
                    Avatar = character.Thumbnail,
                    IsPlayer = p.IsPlayer,
                    TotalStats = p.TotalStats,
                    SlotIndex = p.SlotIndex,
                    IsEliminated = p.IsEliminated
                });
            }

            return list;
        }

        #endregion



        // =========================================================
        // CREATE NEW TOURNAMENT
        // =========================================================

        #region Create Tournament

        /// <summary>
        /// Create new tournament and initialize bracket
        /// </summary>
        public void CreateNewTournament(string tournamentId)
        {
            _currentTournamentSO = m_TournamentConfig.GetTournamentById(tournamentId);

            if (_currentTournamentSO == null)
            {
                Debug.LogError($"Tournament {tournamentId} not found");
                return;
            }

            m_CurrentParticipants.Clear();

            // Add player
            TournamentParticipant player = GetPlayerParticipantData();
            if (player != null)
                m_CurrentParticipants.Add(player);

            // Add NPC enemies
            int npcCount = Mathf.Min(7, _currentTournamentSO.EnemyIds.Length);

            for (int i = 0; i < npcCount; i++)
            {
                string enemyId = _currentTournamentSO.EnemyIds[i];
                m_CurrentParticipants.Add(GetNPCParticipantData(enemyId));
            }

            ShuffleParticipants(m_CurrentParticipants);

            // Assign bracket slot
            for (int i = 0; i < m_CurrentParticipants.Count; i++)
            {
                m_CurrentParticipants[i].SlotIndex = i;
            }

            // Initialize round
            m_CurrentRoundParticipants.Clear();
            m_CurrentRoundParticipants.AddRange(m_CurrentParticipants);

            m_CurrentRoundIndex = 0;

            SaveProgress();

            OpenTournamentBracketUI();
        }

        #endregion



        // =========================================================
        // PARTICIPANT CREATION
        // =========================================================

        #region Participant Creation

        private TournamentParticipant GetPlayerParticipantData()
        {
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            if (string.IsNullOrEmpty(sessionId)) return null;

            PlayerTrainingSession sessionData = _sessionDAO.GetById(sessionId);
            if (sessionData == null) return null;

            SessionCharacterData characterData =
                _characterRepo.GetCharacterData(sessionId, sessionData.RawCharacterId);

            BaseCharacterSO characterSO =
                m_CharacterConfig.GetCharacterById(sessionData.RawCharacterId);

            CharacterDataHolder holder = new CharacterDataHolder.Builder()
                .WithCharacterData(characterData)
                .WithCharacterSO(characterSO)
                .Build();

            float totalStats =
                holder.GetVIT() +
                holder.GetPower() +
                holder.GetAgi() +
                holder.GetINT() +
                holder.GetStamina();

            return new TournamentParticipant
            {
                Id = holder.GetRuntimeId(),
                Name = holder.GetCharacterName(),
                Avatar = holder.GetAvatar(),
                IsPlayer = true,
                TotalStats = totalStats,
                IsEliminated = false
            };
        }


        private TournamentParticipant GetNPCParticipantData(string enemyId)
        {
            EnemySO enemySO = m_CharacterConfig.GetCharacterById(enemyId).ConvertAs<EnemySO>();

            if (enemySO == null) return new TournamentParticipant();

            return new TournamentParticipant
            {
                Id = enemySO.CharacterId,
                Name = enemySO.CharacterName,
                Avatar = enemySO.Thumbnail,
                TotalStats = enemySO.TotalStats,
                IsPlayer = false,
                IsEliminated = false
            };
        }

        #endregion



        // =========================================================
        // BRACKET SETUP
        // =========================================================

        #region Bracket Setup

        /// <summary>
        /// Fisher-Yates shuffle
        /// </summary>
        private void ShuffleParticipants(List<TournamentParticipant> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int r = Random.Range(0, i + 1);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }


        private void OpenTournamentBracketUI()
        {
            _bracketFrame = GameUIManager.Singleton
                                         .ShowFrame(GameConstants.FRAME_ID_TOURNAMENT_BRACKET)
                                         .AsFrame<UITournamentBracketScreen>();

            _bracketFrame.SetupTournament(
                _currentTournamentSO.Name,
                m_CurrentParticipants,
                this);
        }

        #endregion



        // =========================================================
        // TOURNAMENT PROGRESSION
        // =========================================================

        #region Progress Tournament

        public void RequestProgressTournament()
        {
            if (m_CurrentRoundParticipants == null || m_CurrentRoundParticipants.Count <= 1)
            {
                Debug.LogError("Invalid round participants");
                return;
            }

            List<TournamentParticipant> winners = new();

            for (int i = 0; i < m_CurrentRoundParticipants.Count; i += 2)
            {
                TournamentParticipant p1 = m_CurrentRoundParticipants[i];
                TournamentParticipant p2 = m_CurrentRoundParticipants[i + 1];

                TournamentParticipant winner = ResolveMatch(p1, p2);

                if (winner == null)
                {
                    return;
                }

                winners.Add(winner);
            }

            // STORE BRACKET HISTORY
            if (m_CurrentRoundIndex == 0)
            {
                m_SemiFinalParticipants = new List<TournamentParticipant>(winners);
            }
            else if (m_CurrentRoundIndex == 1)
            {
                m_FinalParticipants = new List<TournamentParticipant>(winners);
            }

            m_CurrentRoundParticipants = winners;
            m_CurrentRoundIndex++;

            UpdateBracketUI();
            SaveProgress();

            if (m_CurrentRoundParticipants.Count == 1)
            {
                OnTournamentFinished(m_CurrentRoundParticipants[0]);
            }
        }

        private void ResumeAfterCombat(TournamentProgressData data)
        {
            bool playerWon = data.IsPlayerWon;

            List<TournamentParticipant> winners = new();

            for (int i = 0; i < m_CurrentRoundParticipants.Count; i += 2)
            {
                var p1 = m_CurrentRoundParticipants[i];
                var p2 = m_CurrentRoundParticipants[i + 1];

                if (p1.IsPlayer || p2.IsPlayer)
                {
                    TournamentParticipant winner;

                    if (playerWon)
                    {
                        winner = p1.IsPlayer ? p1 : p2;

                        // enemy bị loại
                        (p1.IsPlayer ? p2 : p1).IsEliminated = true;
                    }
                    else
                    {
                        winner = p1.IsPlayer ? p2 : p1;

                        // player bị loại
                        (p1.IsPlayer ? p1 : p2).IsEliminated = true;
                    }

                    winners.Add(winner);
                }
                else
                {
                    winners.Add(ResolveNPCMatch(p1, p2));
                }
            }

            // STORE HISTORY
            if (m_CurrentRoundIndex == 0)
            {
                m_SemiFinalParticipants = new List<TournamentParticipant>(winners);
            }
            else if (m_CurrentRoundIndex == 1)
            {
                m_FinalParticipants = new List<TournamentParticipant>(winners);
            }

            // ADVANCE ROUND
            m_CurrentRoundParticipants = winners;
            m_CurrentRoundIndex++;

            data.WaitingForPlayerBattle = false;

            SaveProgress();

            // ================================
            // UPDATE UI
            // ================================

            OpenTournamentBracketUI();
            UpdateBracketUI();

            // ================================
            // TOURNAMENT FINISH
            // ================================

            if (winners.Count == 1)
            {
                OnTournamentFinished(winners[0]);
            }
        }

        private TournamentParticipant ResolveNPCMatch(TournamentParticipant p1, TournamentParticipant p2)
        {
            if (p1.TotalStats >= p2.TotalStats)
            {
                p2.IsEliminated = true;
                return p1;
            }

            p1.IsEliminated = true;
            return p2;
        }

        /// <summary>
        /// Resolve a match between two participants
        /// </summary>
        private TournamentParticipant ResolveMatch(TournamentParticipant p1, TournamentParticipant p2)
        {
            Debug.Log($"Match: {p1.Name} vs {p2.Name}");

            if (p1.IsPlayer || p2.IsPlayer)
            {
                string enemyId = p1.IsPlayer ? p2.Id : p1.Id;

                StartPlayerBattle(enemyId);

                return null;
            }

            if (p1.TotalStats >= p2.TotalStats)
            {
                p2.IsEliminated = true;
                return p1;
            }

            p1.IsEliminated = true;
            return p2;
        }

        #endregion



        // =========================================================
        // UI UPDATE
        // =========================================================

        #region UI Update

        private void UpdateBracketUI()
        {
            if (m_SemiFinalParticipants != null && m_SemiFinalParticipants.Count == 4)
            {
                _bracketFrame.UpdateQuarterFinals(m_SemiFinalParticipants);
            }

            if (m_FinalParticipants != null && m_FinalParticipants.Count == 2)
            {
                _bracketFrame.UpdateSemiFinals(m_FinalParticipants);
            }

            if (m_CurrentRoundParticipants != null && m_CurrentRoundParticipants.Count == 1)
            {
                _bracketFrame.UpdateFinal(m_CurrentRoundParticipants);
            }
        }

        #endregion



        // =========================================================
        // FINISH
        // =========================================================

        #region Finish Tournament

        private void OnTournamentFinished(TournamentParticipant champion)
        {
            Debug.Log($"Tournament Champion: {champion.Name}");

            _bracketFrame.ShowChampion(champion);

            if (champion.IsPlayer)
            {
                Debug.Log("Player wins tournament!");
            }
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            PlayerTrainingSession sessionData = _sessionDAO.GetById(sessionId);

            IReadOnlyList<RewardDataSO> rewardList = GetPlayerRewards();

            GameInventoryManager inventory = ContextManager.Singleton.ResolveGameContext<GameInventoryManager>();
            GameDeckController cardInventory = ContextManager.Singleton.ResolveGameContext<GameDeckController>();

            if (rewardList.Count > 0)
            {
                SessionCharacterData character = _characterRepo.GetCharacterData(sessionId, sessionData.RawCharacterId);
                bool charUpdated = false;

                foreach (var reward in rewardList)
                {
                    Debug.Log($"Received: {reward.RewardTargetId} -amount: {reward.RewardAmount}");

                    switch (reward.RewardType)
                    {
                        case ERewardType.Gold:
                            sessionData.CurrentGoldAmount += reward.RewardAmount;
                            break;
                        case ERewardType.ReputationPoint:
                            charUpdated = true;
                            character.CurrentRP += reward.RewardAmount;
                            break;
                        case ERewardType.Item:
                            inventory.AddItem(reward.RewardTargetId, reward.RewardAmount);
                            break;
                        case ERewardType.Card:
                            cardInventory.AddObtainedCard(reward.RewardTargetId, reward.RewardAmount);
                            break;
                    }
                }

                if (charUpdated) _characterRepo.Update(character);
            }

            Debug.Log("Clear tournament progress");

            Debug.Log("Ready to go back to main menu");
        }

        string dialogId = string.Empty;

        private void DetermineTournamentResult(TournamentSO tournamentSO, TournamentProgressData tournamentData, PlayerTrainingSession sessionData)
        {
            if (tournamentSO.IsCheckpointTournament == true)
            {
                // Check if the player has passed the tournament objective.

                // If the player failed to pass the tournament objective -> game over.

                // If the player success in passing the tournament objective -> continue/victory.

                bool objectiveCompleted = false;

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
                    }
                    else
                    {
                        // Lose the objective
                        objectiveCompleted = false;
                    }
                }

                if (objectiveCompleted == false)
                {
                    if (tournamentSO.IsFinalTournament)
                    {
                        dialogId = GameConstants.GetCh0027Ending();
                    }
                }
                else
                {
                    if (tournamentSO.IsFinalTournament)
                    {
                        switch (sessionData.RawCharacterId)
                        {
                            case "ch_0001":
                                dialogId = GameConstants.GetCh0001Ending();
                                break;
                            case "ch_0002":
                                dialogId = GameConstants.GetCh0002Ending();
                                break;
                            case "ch_0003":
                                dialogId = GameConstants.GetCh0003Ending();
                                break;
                        }
                    }
                    else
                    {
                        // Objective success.
                        if (!string.IsNullOrEmpty(tournamentSO.PassObjectiveDialogId))
                        {
                            dialogId = tournamentSO.PassObjectiveDialogId;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(dialogId))
                {
                    FadingController.Singleton.FadeIn2Out(1f, 1f, () => {
                        SceneLoader.Singleton.StartLoad("Scene.Cutscene", null, new List<LoadTask>
                    {
                        new LoadTask("Load dialog", DelayLoadDialog)
                    });
                    });
                }
            }
        }

        private IEnumerator DelayLoadDialog()
        {
            yield return new WaitForSeconds(0.02f);
            ContextManager.Singleton.ResolveGameContext<NarrativeManager>().StartTree(dialogId);
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



        public void GoBackToMainMenu()
        {
            TournamentProgressData data = _tournamentDAO.GetById(_sessionTournamentId);
            PlayerTrainingSession sesionData = _sessionDAO.GetById(PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID));
            ClearAllPrefKeys();
            DetermineTournamentResult(_currentTournamentSO, data, sesionData);

            //List<LoadTask> loadTaskList = new List<LoadTask>();
            //LoadTask goToNextWeekTask = new LoadTask("", DelayGoToNextWeek);
            //loadTaskList.Add(goToNextWeekTask);

            //SceneLoader.Singleton.StartLoad(GameConstants.SCENE_MAIN_MENU, null, loadTaskList);
        }

        private IEnumerator DelayGoToNextWeek()
        {
            yield return new WaitForSeconds(0.25f);
            _eventManager.Publish(new EndTournamentEvent
            {
                SessionTournamentId = _sessionTournamentId,
            });
        }

        public IReadOnlyList<RewardDataSO> GetPlayerRewards()
        {
            TournamentRewardTier tier = GetPlayerRewardTier();

            IReadOnlyList<RewardDataSO> rewardRanks = GetRewardsByTier(tier);

            if (rewardRanks == null || rewardRanks.Count == 0)
                return new List<RewardDataSO>();

            return rewardRanks;
        }

        private TournamentRewardTier GetPlayerRewardTier()
        {
            bool playerAlive = m_CurrentRoundParticipants.Exists(p => p.IsPlayer);

            if (playerAlive && m_CurrentRoundParticipants.Count == 1)
            {
                return TournamentRewardTier.Champion;
            }

            if (m_CurrentRoundIndex >= 2)
            {
                return TournamentRewardTier.SemiFinal;
            }

            return TournamentRewardTier.Elimination;
        }

        private IReadOnlyList<RewardDataSO> GetRewardsByTier(TournamentRewardTier tier)
        {
            switch (tier)
            {
                case TournamentRewardTier.Champion:
                    return _currentTournamentSO.ChampionRewards;

                case TournamentRewardTier.SemiFinal:
                    return _currentTournamentSO.SemiFinalRewards;

                default:
                    return _currentTournamentSO.EliminationRewards;
            }
        }
        #endregion



        // =========================================================
        // COMBAT
        // =========================================================

        #region Combat

        private void StartPlayerBattle(string enemyId)
        {
            TournamentProgressData data = BuildProgressData();

            data.WaitingForPlayerBattle = true;
            data.PendingEnemyId = enemyId;

            _tournamentDAO.Upsert(data);
            LocalDBOrchestrator.UpdateDBChangeTime();

            PlayerPrefs.SetString(GameConstants.PREF_KEY_COMBAT_TYPE, GameConstants.COMBAT_TYPE_TOURNAMENT);

            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_COMBAT);
        }
        #endregion



        // =========================================================
        // SAVE SYSTEM
        // =========================================================

        #region Save

        private void SaveProgress()
        {
            TournamentProgressData data = BuildProgressData();
            _tournamentDAO.Upsert(data);

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            PlayerTrainingSession sessionData = _sessionDAO.GetById(sessionId);

            if (sessionData != null)
            {
                sessionData.ActiveTournamentId = data.Id;
                if (_sessionDAO.Update(sessionData))
                {
                    LocalDBOrchestrator.UpdateDBChangeTime();
                }
                Debug.Log($"<color=green>[GameTournamentController.SaveProgress]</color> Session's active tournament id is updated!");
            }
            else
            {
                Debug.LogError($"[GameTournamentController.SaveProgress error] Session data with id {sessionId} does not exist in the database.");
            }
        }


        private TournamentProgressData BuildProgressData()
        {
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

            return new TournamentProgressData
            {
                Id = EntityIdConstructor.ConstructDBEntityId(sessionId, _currentTournamentSO.TournamentId),
                SessionId = sessionId,
                RawTournamentId = _currentTournamentSO.TournamentId,
                CurrentRoundIndex = m_CurrentRoundIndex,
                WaitingForPlayerBattle = false,
                Participants = ConvertParticipants(m_CurrentParticipants),
                SemiFinalParticipants = ConvertParticipants(m_SemiFinalParticipants),
                FinalParticipants = ConvertParticipants(m_FinalParticipants),
                CurrentRoundParticipants = ConvertParticipants(m_CurrentRoundParticipants)
            };
        }


        private List<TournamentParticipantData> ConvertParticipants(List<TournamentParticipant> list)
        {
            List<TournamentParticipantData> result = new();

            foreach (var p in list)
            {
                result.Add(new TournamentParticipantData
                {
                    Id = p.Id,
                    CharacterId = p.Id,
                    IsPlayer = p.IsPlayer,
                    SlotIndex = p.SlotIndex,
                    TotalStats = p.TotalStats,
                    IsEliminated = p.IsEliminated
                });
            }

            return result;
        }

        #endregion

        

        public void ClearAllPrefKeys()
        {
            PlayerPrefs.DeleteKey(GameConstants.PREF_KEY_COMBAT_TYPE);
            PlayerPrefs.DeleteKey(GameConstants.PREF_KEY_TOURNAMENT_ID);
        }
    }

    public enum TournamentRewardTier
    {
        Elimination,
        SemiFinal,
        Champion
    }
}