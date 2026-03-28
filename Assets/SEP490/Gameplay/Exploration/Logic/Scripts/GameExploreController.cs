namespace SEP490G69.Exploration
{
    using System.Collections.Generic;
    using System.Linq;
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.GameSessions;
    using UnityEngine;
    using System.Collections;
    using SEP490G69.Battle.Cards;

    public class GameExploreController : MonoBehaviour, ISceneContext
    {
        private const int EVENT_ID_INDEX = 0;
        private const int CHOICE_INDEX = 1;
        private const int OUTCOME_INDEX = 2;

        private const float EVENT_DISPLAY_MIN = 2f;
        private const float EVENT_DISPLAY_MAX = 4f;

        [SerializeField] private SceneCharacterLoader m_CharacterLoader;
        [SerializeField] private Material m_DefaultBgMaterial;
        [SerializeField] private Transform m_RunningManContainer;
        [SerializeField] private ScrollingBackground m_Scroller;
        [SerializeField] private ExplorePoolConfigSO m_PoolConfig;

        private readonly Timer _delayEventTimer = new Timer();
        private readonly Timer _delayCombatTimer = new Timer();

        private GameSessionDAO _sessionDAO;
        private GameExploreLocationDAO _explorationDAO;
        private PlayerCharacterRepository _characterRepo;

        private IBossContactPercentCalculator _bossContactCalculator;

        private string _sessionId;
        private string _selectedLocationId;
        private string _characterId;

        private EventManager _eventManager;
        private GameInventoryManager _inventoryManager;
        private GameDeckController _deckController;
        private CharacterDataHolder _characterHolder;

        private CharacterConfigSO _characterConfig;
        private ExplorationConfigSO _exploreConfig;

        #region Lazy properties
        private ExplorationConfigSO ExploreConfig
        {
            get
            {
                if (_exploreConfig == null)
                {
                    _exploreConfig = ContextManager.Singleton.GetDataSO<ExplorationConfigSO>();
                }
                return _exploreConfig;
            }
        }
        private CharacterConfigSO CharacterConfig
        {
            get
            {
                if (_characterConfig == null)
                {
                    _characterConfig = ContextManager.Singleton.GetDataSO<CharacterConfigSO>();
                }
                return _characterConfig;
            }
        }
        private GameInventoryManager InventoryController
        {
            get
            {
                if (_inventoryManager == null)
                {
                    _inventoryManager = ContextManager.Singleton.ResolveGameContext<GameInventoryManager>();
                }
                return _inventoryManager;
            }
        }
        private GameDeckController DeckController
        {
            get
            {
                if (_deckController == null)
                {
                    _deckController = ContextManager.Singleton.ResolveGameContext<GameDeckController>();
                }
                return _deckController;
            }
        }
        private EventManager EventManager
        {
            get
            {
                if (_eventManager == null)
                {
                    _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();
                }
                return _eventManager;
            }
        }
        #endregion

        #region Unity methods

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);
            m_Scroller.CloseBg();
            TimerManager.AddTimer(_delayEventTimer);
            TimerManager.AddTimer(_delayCombatTimer);

            _delayEventTimer.OnExpired += OnTimerExpired;
        }
        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
            _delayEventTimer.OnExpired -= OnTimerExpired;
            TimerManager.RemoveTimer(_delayEventTimer);
            TimerManager.RemoveTimer(_delayCombatTimer);
        }

        private void Start()
        {
            LoadDAOs();
            LoadServices();
            SetupDBLocations();

            bool isInExplore = PlayerPrefs.GetInt(GameConstants.PREF_KEY_IS_IN_EXPLORE, 0) == 1;
            if (isInExplore == false)
            {
                LoadCharacter();

                GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_EXPLORATION);
            }
            else
            {
                HandleCombatResult();
            }
        }
        #endregion

        private void LoadServices()
        {
            _bossContactCalculator = new BossContactPercentCalculator();
        }

        private void LoadDAOs()
        {
            _sessionDAO = new GameSessionDAO();
            _explorationDAO = new GameExploreLocationDAO();
            _characterRepo = new PlayerCharacterRepository();

            _sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
        }

        private void LoadCharacter()
        {
            if (m_CharacterLoader == null ||
                string.IsNullOrEmpty(_sessionId))
            {
                Debug.LogError("[GameExploreController.LoadCharacter fatal error] Session id is empty!");
                return;
            }

            PlayerTrainingSession sessionData = _sessionDAO.GetById(_sessionId);

            if (sessionData == null)
            {
                Debug.LogError("[GameExploreController.LoadCharacter fatal error] Session data is null");
                return;
            }
            if (string.IsNullOrEmpty(sessionData.RawCharacterId))
            {
                Debug.LogError("[GameExploreController.LoadCharacter fatal error] Raw character id in session data is empty.");
                return;
            }

            BaseCharacterSO characterSO = CharacterConfig.GetCharacterById(sessionData.RawCharacterId);
            SessionCharacterData characterData = _characterRepo.GetCharacterData(_sessionId, sessionData.RawCharacterId);

            if (characterSO == null || characterData == null)
            {
                Debug.LogError("[GameExploreController.LoadCharacter fatal error] CharacterSO/CharacterDBEntity is null!");
                return;
            }

            _characterHolder = new CharacterDataHolder.Builder().
                                   WithCharacterData(characterData).
                                   WithCharacterSO(characterSO).Build();

            m_CharacterLoader.LoadPlayerCharacter(characterSO);
        }

        /// <summary>
        /// Initialize the locations for the first time.
        /// </summary>
        private void SetupDBLocations()
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                return;
            }
            List<ExploreLocationData> locations = _explorationDAO.GetAllById(_sessionId);

            if (locations == null || locations.Count == 0)
            {
                IReadOnlyList<ExplorationSO> locationSOList = ExploreConfig.ExplorationLocations;
                List<ExploreLocationData> locationDataList = new List<ExploreLocationData>();

                foreach (var locationSo in locationSOList)
                {
                    string entityId = EntityIdConstructor.ConstructDBEntityId(_sessionId, locationSo.ExplorationId);
                    ExploreLocationData locationData = new ExploreLocationData
                    {
                        EntityId = entityId,
                        SessionId = _sessionId,
                        ExplorationCount = 0
                    };
                    locationDataList.Add(locationData);
                }
                _explorationDAO.InsertMany(locationDataList);
            }
        }

        private void HandleCombatResult()
        {
            string battleType = PlayerPrefs.GetString(GameConstants.PREF_KEY_COMBAT_TYPE, string.Empty);

            if (string.IsNullOrEmpty(battleType) ||
                battleType != GameConstants.COMBAT_TYPE_EXPLORATION)
            {
                return;
            }

            _selectedLocationId = PlayerPrefs.GetString(GameConstants.PREF_KEY_EXPLORE_ENEMY_ID, string.Empty);
            if (string.IsNullOrEmpty(_selectedLocationId))
            {
                Debug.LogError("location id is empty");
                return;
            }

            bool isBattleWon = PlayerPrefs.GetInt(GameConstants.PREF_KEY_IS_BATTLE_WON, 0) == 1;

            string outcomeId = PlayerPrefs.GetString(GameConstants.PREF_KEY_OUTCOME_ID, string.Empty);

            string[] idParts = outcomeId.Split(':');
            string eventId = idParts[EVENT_ID_INDEX];
            int choiceIndex = int.Parse(idParts[CHOICE_INDEX]);
            int outcomeIndex = int.Parse(idParts[OUTCOME_INDEX]);

            string rawLocationId = EntityIdConstructor.ExtractRawEntityId(_selectedLocationId);

            ExplorationSO locationSO = ExploreConfig.GetById(rawLocationId);

            if (locationSO == null)
            {
                Debug.LogError("location so is empty");

                return;
            }

            ExploreEventSO eventSO = null;

            if (locationSO.BossEvent != null && locationSO.BossEvent.EventId == eventId)
            {
                eventSO = locationSO.BossEvent;
            }
            else
            {
                eventSO = locationSO.OtherEvents.FirstOrDefault(e => e.EventId == eventId);
            }

            if (eventSO == null)
            {
                Debug.LogError("event so is empty");

                return;
            }

            ExploreEventChoiceData choiceData = eventSO.Choices[choiceIndex];
            ExploreEventOutcomeData outcomeData = choiceData.Outcomes[outcomeIndex];
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_CHOICE_OUTCOME).AsFrame<UIExploreChoiceOutcomeFrame>().ShowOutcomeMessage(outcomeData.OutcomeMsg);

            if (isBattleWon)
            {
                if (outcomeData.DisplayRewardOrder == ERewardPenaltyOrder.DisplayAfterBattle)
                {
                    Dictionary<ERewardType, string[]> extraRewards = new Dictionary<ERewardType, string[]>();

                    if (outcomeData.GetMoreFromPool)
                    {
                        foreach (var pool in outcomeData.RewardPoolList)
                        {
                            GetRewardIdFromPool(pool.PoolId, pool.AmountFromPool, out ERewardType rewardCategory, out string[] rewardIdArray);
                            extraRewards.Add(rewardCategory, rewardIdArray);
                        }
                    }

                    GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_CHOICE_OUTCOME)
                           .AsFrame<UIExploreChoiceOutcomeFrame>()
                           .LoadRewards(outcomeData.Rewards, extraRewards);
                    ReceiveRewards(outcomeData.Rewards, extraRewards);
                    StartCoroutine(DelayBeforeEnd());
                }
            }
            else
            {
                if (outcomeData.DisplayPenaltyOrder == ERewardPenaltyOrder.DisplayAfterBattle)
                {
                    GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_CHOICE_OUTCOME)
                           .AsFrame<UIExploreChoiceOutcomeFrame>()
                           .LoadPenalties(outcomeData.PenatyModifiers);
                    ReceivePenalties(outcomeData.PenatyModifiers);
                    StartCoroutine(DelayBeforeEnd());
                }
            }
        }

        private IEnumerator DelayBeforeEnd()
        {
            yield return new WaitForSeconds(1f);
            IncreaseExploreCount();
            FadingController.Singleton.FadeIn2Out(0.5f, 0.5f, () =>
            {
                OnEventCompleted();
            });
        }

        public void StartExplore(string exploreSessionId = "", bool continuation = false)
        {
            if (string.IsNullOrEmpty(_sessionId)) return;

            if (string.IsNullOrEmpty(exploreSessionId))
            {
                return;
            }
            string rawId = EntityIdConstructor.ExtractRawEntityId(exploreSessionId);

            ExplorationSO exploreSO = ExploreConfig.GetById(rawId);
            if (exploreSO == null)
            {
                return;
            }

            PlayerTrainingSession sessionData = _sessionDAO.GetById(_sessionId);
            if (sessionData == null)
            {
                return;
            }
            _selectedLocationId = exploreSessionId;

            m_Scroller.SetMaterial(exploreSO != null ? exploreSO.ScrollableMat : m_DefaultBgMaterial);
            // Spawn character's running prefab here.
            // Scoll the background.
            // Start a countdown for about 2 - 4 seconds.
            // Roll 2 random events.

            FadingController.Singleton.FadeIn2Out(1f, 1f, () =>
            {
                m_Scroller.OpenBg();
                BaseCharacterSO characterSO = CharacterConfig.GetCharacterById(sessionData.RawCharacterId);
                if (characterSO != null && characterSO.RunningPrefab != null)
                {
                    PoolManager.Pools["Character"].Spawn(characterSO.RunningPrefab, m_RunningManContainer);
                }
                m_CharacterLoader.DespawnCharacter();
                m_Scroller.StartScrolling();
                GameUIManager.Singleton.HideFrame(GameConstants.FRAME_ID_EXPLORATION);
                PlayerPrefs.SetInt(GameConstants.PREF_KEY_IS_IN_EXPLORE, 1);
                PlayerPrefs.SetString(GameConstants.PREF_KEY_EXPLORE_LOCATION_ID, _selectedLocationId);
            }, () =>
            {
                GenerateEvent();
            });
        }

        public IReadOnlyList<ExploreLocationDataHolder> GetAllLocations()
        {
            List<ExploreLocationDataHolder> locationHolders = new List<ExploreLocationDataHolder>();

            foreach (var locationSO in ExploreConfig.ExplorationLocations)
            {
                string entityId = EntityIdConstructor.ConstructDBEntityId(_sessionId, locationSO.ExplorationId);
                ExploreLocationData data = _explorationDAO.GetById(entityId);
                ExploreLocationDataHolder holder = new ExploreLocationDataHolder.Builder().WithData(data).WithSO(locationSO).Build();
                locationHolders.Add(holder);
            }
            return locationHolders;
        }

        private void GenerateEvent()
        {
            float delay = Random.Range(EVENT_DISPLAY_MIN, EVENT_DISPLAY_MAX);
            _delayEventTimer.StartTimer(delay);
        }

        private void OnTimerExpired(Timer t)
        {
            // Shuffle event here.
            ShuffleEvent();
        }

        private void ShuffleEvent()
        {
            string rawLocationId = EntityIdConstructor.ExtractRawEntityId(_selectedLocationId);
            ExplorationSO locationSO = ExploreConfig.GetById(rawLocationId);

            if (locationSO == null) return;

            ExploreLocationData locationData = _explorationDAO.GetById(_selectedLocationId);
            if (locationData == null) return;

            List<ExploreEventSO> resultEvents = new List<ExploreEventSO>();

            // =========================
            // 1. Roll Boss
            // =========================
            float bossEncounterPercent = _bossContactCalculator.CalculateContactPercent(locationData.ExplorationCount);
            float bossRoll = Random.value;


            if (bossRoll <= bossEncounterPercent && !locationData.IsBossDefeated && locationSO.BossEvent != null)
            {
                resultEvents.Add(locationSO.BossEvent);
            }

            // =========================
            // 2. Prepare Other Events Pool
            // =========================
            List<ExploreEventSO> pool = new List<ExploreEventSO>(locationSO.OtherEvents);

            if (pool == null || pool.Count == 0)
            {
                Debug.LogWarning("No other events available!");
                return;
            }

            // =========================
            // 3. Shuffle pool (Fisher-Yates)
            // =========================
            for (int i = 0; i < pool.Count; i++)
            {
                int randIndex = Random.Range(i, pool.Count);
                (pool[i], pool[randIndex]) = (pool[randIndex], pool[i]);
            }

            // =========================
            // 4. Pick remaining slots
            // =========================
            int needed = 2 - resultEvents.Count;

            for (int i = 0; i < pool.Count && resultEvents.Count < 2; i++)
            {
                resultEvents.Add(pool[i]);
            }

            // =========================
            // 5. Fail-safe (edge case)
            // =========================
            if (resultEvents.Count < 2)
            {
                Debug.LogWarning("Not enough events to fill slots!");
                return;
            }

            // =========================
            // 6. TODO: Trigger UI / flow
            // =========================
            Debug.Log($"Event 1: {resultEvents[0].name}");
            Debug.Log($"Event 2: {resultEvents[1].name}");

            // ShowEventChoices(resultEvents);
            GameUIManager.Singleton.ShowFrame("Frame.ExploreEventSelection").AsFrame<UIExploreEventSelectionFrame>().DisplayEvents(new List<ExploreEventSO> {
                resultEvents[0], resultEvents[1]
            });
        }

        /// <summary>
        /// When the player select an event type, the system shuffles a random event based on the selected event type.
        /// </summary>
        /// <param name="eventType"></param>
        public void SelectEventType(EExploreEventType eventType)
        {
            string rawLocationId = EntityIdConstructor.ExtractRawEntityId(_selectedLocationId);
            ExplorationSO locationSO = ExploreConfig.GetById(rawLocationId);

            if (locationSO == null) return;

            ExploreEventSO eventSO = null;

            switch(eventType)
            {
                case EExploreEventType.Boss:
                    eventSO = locationSO.BossEvent;
                    break;
                case EExploreEventType.Encounter:
                case EExploreEventType.Chest:
                case EExploreEventType.Combat:
                    IReadOnlyList<ExploreEventSO> events = locationSO.OtherEvents;

                    if (events.Count == 0) return;

                    eventSO = events[Random.Range(0, events.Count)];
                    break;
                default:
                    Debug.LogError($"Unsupported event type {eventType.ToString()}");
                    break;
            }

            if (eventSO == null) return;

            GameUIManager.Singleton.ShowFrame("Frame.ChoiceEvent").AsFrame<UIEventChoiceSelectFrame>().LoadChoices(eventSO);
        }

        public void SelectChoiceOfEvent(string eventId, int choiceIndex)
        {
            string rawLocationId = EntityIdConstructor.ExtractRawEntityId(_selectedLocationId);
            ExplorationSO locationSO = ExploreConfig.GetById(rawLocationId);

            if (locationSO == null) return;
            ExploreEventSO eventSO = null;

            if (eventId.Contains("BOSS"))
            {
                eventSO = locationSO.BossEvent;
            }
            else
            {
                eventSO = locationSO.OtherEvents.FirstOrDefault(e => e.EventId == eventId);
            }

            if (eventSO == null)
            {
                return;
            }

            if (choiceIndex < 0 || choiceIndex > eventSO.Choices.Count - 1)
            {
                return;
            }

            ExploreEventChoiceData choiceData = eventSO.Choices[choiceIndex];

            switch (choiceData.Condition.SuccessCondition)
            {
                case EChoiceSuccessType.ByStatus:
                    HandleStatConditionOutcome(eventId, choiceIndex, choiceData);
                    break;
                case EChoiceSuccessType.Random:
                    HandleRandomConditionOutcome(eventId, choiceIndex, choiceData);
                    break;
                case EChoiceSuccessType.None:
                    // If the choice has at least an outcome, perform that outcome.
                    HandleOutcome(eventId, choiceIndex, 0, choiceData.Outcomes[0]);
                    break;
            }
        }

        private void HandleRandomConditionOutcome(string eventId, int choiceIndex, ExploreEventChoiceData choice)
        {
            bool success = CheckRandomCondition(choice);

            if (success)
            {
                ExploreEventOutcomeData successOutcome = choice.Outcomes[0];
                HandleOutcome(eventId, choiceIndex, 0, successOutcome);
            }
            else
            {
                ExploreEventOutcomeData failedOutcome = choice.Outcomes[1];
                HandleOutcome(eventId, choiceIndex, 1, failedOutcome);
            }
        }

        private void HandleStatConditionOutcome(string eventId, int choiceIndex, ExploreEventChoiceData choice)
        {
            bool success = CheckStatCondition(choice);

            if (success)
            {
                ExploreEventOutcomeData successOutcome = choice.Outcomes[0];
                HandleOutcome(eventId, choiceIndex, 0, successOutcome);
            }
            else
            {
                ExploreEventOutcomeData failedOutcome = choice.Outcomes[1];
                HandleOutcome(eventId, choiceIndex, 1, failedOutcome);
            }
        }

        private bool CheckStatCondition(ExploreEventChoiceData choiceData)
        {
            float requiredStat = _characterHolder.GetStatus(choiceData.Condition.ConditionStat);
            return requiredStat >= choiceData.Condition.RequiredValue;
        }

        private bool CheckRandomCondition(ExploreEventChoiceData choiceData)
        {
            float percent = choiceData.Condition.RandomValue;
            float randomValue = Random.Range(0, 1f);
            return randomValue <= percent;
        }

        private void HandleOutcome(string eventId, int choiceIndex, int outcomeIndex, ExploreEventOutcomeData outcome)
        {
            bool hasCombat = false;
            switch (outcome.OutcomeType)
            {
                case EExploreEventOutcome.Combat:
                    // Required combat pending.
                    hasCombat = true;
                    _delayCombatTimer.StartTimer(3f);
                    _delayCombatTimer.OnExpired = (timer) =>
                    {
                        string outcomeId = ExplorationHelper.ConstructPendingEventOutcomeId(eventId, choiceIndex, outcomeIndex);
                        string enemyId = outcome.EnemyId;
                        if (outcome.RandomEnemyFromPool)
                        {
                            enemyId = GetEnemyFromPool(enemyId);

                            if (string.IsNullOrEmpty(enemyId))
                            {
                                Debug.LogError($"[GameExploreController.HandleOutcome fatal error] Pool with id {enemyId} returns an empty id element");
                                return;
                            }
                        }
                        PlayerPrefs.SetString(GameConstants.PREF_KEY_OUTCOME_ID, outcomeId);
                        PlayerPrefs.SetString(GameConstants.PREF_KEY_EXPLORE_ENEMY_ID, enemyId);
                        PlayerPrefs.SetString(GameConstants.PREF_KEY_COMBAT_TYPE, GameConstants.COMBAT_TYPE_EXPLORATION);

                        SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_COMBAT);
                    };
                    break;
            }

            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_CHOICE_OUTCOME)
                                   .AsFrame<UIExploreChoiceOutcomeFrame>()
                                   .ShowOutcomeMessage(outcome.OutcomeMsg);

            if (outcome.DisplayRewardOrder == ERewardPenaltyOrder.DisplayBeforeBattle ||
                outcome.DisplayRewardOrder == ERewardPenaltyOrder.Immedite)
            {
                Dictionary<ERewardType, string[]> extraRewards = new Dictionary<ERewardType, string[]>();

                if (outcome.GetMoreFromPool)
                {
                    foreach (var pool in outcome.RewardPoolList)
                    {
                        GetRewardIdFromPool(pool.PoolId, pool.AmountFromPool, out ERewardType rewardCategory, out string[] rewardIdArray);
                        extraRewards.Add(rewardCategory, rewardIdArray);
                    }
                }

                GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_CHOICE_OUTCOME)
                       .AsFrame<UIExploreChoiceOutcomeFrame>()
                       .LoadRewards(outcome.Rewards, extraRewards);
                ReceiveRewards(outcome.Rewards, extraRewards);
            }

            if (outcome.DisplayPenaltyOrder == ERewardPenaltyOrder.DisplayBeforeBattle ||
                outcome.DisplayRewardOrder == ERewardPenaltyOrder.Immedite)
            {
                GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_CHOICE_OUTCOME)
                       .AsFrame<UIExploreChoiceOutcomeFrame>()
                       .LoadPenalties(outcome.PenatyModifiers);
                ReceivePenalties(outcome.PenatyModifiers);
            }

            if (hasCombat == true)
            {
                _delayCombatTimer.OnChange = (eventId, cur) =>
                {
                    GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_CHOICE_OUTCOME)
                       .AsFrame<UIExploreChoiceOutcomeFrame>()
                       .ShowPendingCombat(cur);
                };
            }
            else
            {
                StartCoroutine(DelayBeforeEnd());
            }
        }

        private void OnEventCompleted()
        {
            _selectedLocationId = string.Empty;
            LoadCharacter();
            ClearAllExplorePrefs();
            CloseRunningBg();
            GameUIManager.Singleton.HideFrame(GameConstants.FRAME_ID_CHOICE_OUTCOME);
            GameUIManager.Singleton.HideFrame(GameConstants.FRAME_ID_EXPLORE_EVENTS_SELECT);
            GameUIManager.Singleton.HideFrame(GameConstants.FRAME_ID_EXPLORE_CHOICE_SELECT);
            //GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_EXPLORATION);

            List<LoadTask> postLoadTasks = new List<LoadTask>();
            postLoadTasks.Add(new LoadTask("Go to next week", DelayIncreaseWeek));

            SceneLoader.Singleton.StartLoad(GameConstants.SCENE_MAIN_MENU, null, postLoadTasks);
        }

        private IEnumerator DelayIncreaseWeek()
        {
            yield return new WaitForSeconds(0.1f);
            EventManager.Publish(new ExploreCompleteEvent());
        }

        private void CloseRunningBg()
        {
            m_Scroller.StopScrolling();
            m_Scroller.CloseBg();
        }

        public void ClearAllExplorePrefs()
        {
            PlayerPrefs.DeleteKey(GameConstants.PREF_KEY_EXPLORE_LOCATION_ID);
            PlayerPrefs.DeleteKey(GameConstants.PREF_KEY_OUTCOME_ID);
            PlayerPrefs.DeleteKey(GameConstants.PREF_KEY_IS_IN_EXPLORE);
            PlayerPrefs.DeleteKey(GameConstants.PREF_KEY_IS_BATTLE_WON);
            PlayerPrefs.DeleteKey(GameConstants.PREF_KEY_COMBAT_TYPE);
            PlayerPrefs.DeleteKey(GameConstants.PREF_KEY_EXPLORE_ENEMY_ID);
        }

        public void GetRewardIdFromPool(string poolId, int amount, out ERewardType type, out string[] rewardIdArray)
        {
            type = ERewardType.None;
            rewardIdArray = new string[0];

            ExplorePoolSO poolSO = m_PoolConfig.GetById(poolId);

            if (poolSO == null)
            {
                return;
            }

            type = poolSO.RewardCategory;
            rewardIdArray = poolSO.GetRandomUniqueElements(amount).ToArray();

            if (rewardIdArray == null)
            {
                rewardIdArray = new string[0];
            }

            //return new KeyValuePair<ERewardType, string[]>(poolSO.RewardCategory, idArray);
        }

        public string GetEnemyFromPool(string poolId)
        {
            ExplorePoolSO poolSO = m_PoolConfig.GetById(poolId);

            if (poolSO == null)
            {
                return string.Empty;
            }

            return poolSO.GetRandomElement();
        }

        private void IncreaseExploreCount()
        {
            if (string.IsNullOrEmpty(_selectedLocationId))
            {
                return;
            }
            ExploreLocationData location = _explorationDAO.GetById(_selectedLocationId);
            IncreaseExploreCount(location);
        }

        private void IncreaseExploreCount(ExploreLocationData location)
        {
            if (location == null)
            {
                return;
            }
            location.ExplorationCount++;
            _explorationDAO.Update(location);
        }

        #region Result methods
        private void ReceiveRewards(IReadOnlyList<RewardDataSO> rewardList, Dictionary<ERewardType, string[]> extraRewards)
        {
            foreach (var reward in rewardList)
            {
                ReceiveReward(reward.RewardType, reward.RewardTargetId, reward.RewardAmount, reward.StatModifiers);
            }

            foreach (var rewardPool in extraRewards)
            {
                if (rewardPool.Value != null && rewardPool.Value.Length > 0)
                {
                    foreach (var rewardID in rewardPool.Value)
                    {
                        ReceiveReward(rewardPool.Key, rewardID, 1, null);
                    }
                }
            }
        }

        private void ReceivePenalties(IReadOnlyList<StatusModifierSO> modifiers)
        {
            UpdateCharacterStatChanges(modifiers);
        }

        private void ReceiveReward(ERewardType rewardType, string rewardId, int amount, IReadOnlyList<StatusModifierSO> modifiers)
        {
            switch (rewardType)
            {
                case ERewardType.Gold:
                    UpdateGold(amount);
                    break;
                case ERewardType.ReputationPoint:
                    UpdateCharacterRP(amount);
                    break;
                case ERewardType.Stats:
                    if (modifiers != null && modifiers.Count > 0)
                    {
                        UpdateCharacterStatChanges(modifiers);
                    }
                    break;
                case ERewardType.Item:
                    InventoryController.AddItem(rewardId, amount);
                    break;
                case ERewardType.Card:
                    DeckController.AddObtainedCard(rewardId, amount);
                    break;
                default:
                    Debug.LogError($"[GameExploreController error] Unsupported reward type {rewardType}");
                    break;
            }
        }

        private void UpdateCharacterStatChanges(IReadOnlyList<StatusModifierSO> modifiers)
        {
            PlayerTrainingSession sessionData = _sessionDAO.GetById(_sessionId);
            if (sessionData == null)
            {
                return;
            }
            SessionCharacterData characterData = _characterRepo.GetCharacterData(_sessionId, sessionData.RawCharacterId);
            CharacterDataHolder characterHolder = new CharacterDataHolder.Builder().WithCharacterData(characterData).Build();

            if (characterData == null)
            {
                return;
            }
            foreach (var modifier in modifiers)
            {
                float modValue = modifier.GetModifiedStatus(characterHolder.GetStatus(modifier.StatType));
                characterHolder.SetStatus(modifier.StatType, modValue);
            }
            characterHolder.UpdateChanges(_characterRepo);
        }

        private void UpdateCharacterRP(int amount)
        {
            PlayerTrainingSession sessionData = _sessionDAO.GetById(_sessionId);
            if (sessionData == null)
            {
                return;
            }
            SessionCharacterData characterData = _characterRepo.GetCharacterData(_sessionId, sessionData.RawCharacterId);
            CharacterDataHolder characterHolder = new CharacterDataHolder.Builder().WithCharacterData(characterData).Build();

            if (characterData == null)
            {
                return;
            }
            characterHolder.SetStatus(EStatusType.RP, characterHolder.GetRP() + amount);
            characterHolder.UpdateChanges(_characterRepo);
        }

        private void UpdateGold(int amount)
        {
            PlayerTrainingSession sessionData = _sessionDAO.GetById(_sessionId);
            if (sessionData == null)
            {
                return;
            }
            sessionData.CurrentGoldAmount += amount;
            _sessionDAO.Update(sessionData);
        }
        #endregion
    }
}