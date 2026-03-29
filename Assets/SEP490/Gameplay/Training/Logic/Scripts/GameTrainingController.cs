namespace SEP490G69.Training
{
    using System.Collections.Generic;
    using System.Linq;
    using SEP490G69.Calendar;
    using SEP490G69.Economy;
    using SEP490G69.GameSessions;
    using UnityEngine;

    public enum ETrainingType
    {
        Rest,
        Boxing,
        Run,
        Dodge,
        Study,
        Swim
    }

    public class GameTrainingController : MonoBehaviour, ISceneContext
    {
        [SerializeField] private Transform m_CharacterContainer;
        [SerializeField] private GameObject m_MainMenuBG;
        [SerializeField] private GameObject m_TrainingMenuBG;
        [SerializeField] private Transform m_TrainingCharContainer;
        [SerializeField] private Transform m_TrainingAnimContainer;

        [Header("Training Animation Prefabs")]
        [SerializeField] private TrainingAnimationConfigSO m_TrainingAnimConfig;
        private BaseTrainingAnimationController _currentAnimController;

        [Header("UI System")]
        [SerializeField] private GameObject m_OverlayPrefab;
        [SerializeField] private Transform m_UICanvas;

        private GameObject _activeOverlayInstance;

        private EventManager _eventManager;

        private string _sessionId;

        // CONFIGs
        private TrainingExerciseConfigSO _exercisesConfig;
        private CharacterConfigSO _characterConfig;

        // CHARACTER
        private CharacterDataHolder _characterHolder;
        private Animator _characterAnimator;

        // DAOs
        private GameSessionDAO _sessionDAO;
        private TrainingExerciseDAO _exercisesDAO;
        private PlayerCharacterRepository _characterRepo;
        private PlayerCharacterDAO _characterDAO;
        private SupportItemsService _supportItemsService;

        private List<ITrainingStrategy> _exerciseList = new List<ITrainingStrategy>();

        public CharacterDataHolder CharacterData => _characterHolder;

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);

            _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();

            LoadConfigs();
            LoadDAOs();
            LoadExerciseStrategies();

            _sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            if (string.IsNullOrEmpty(_sessionId)) return;

            PlayerTrainingSession sessionData = _sessionDAO.GetById(_sessionId);
            if (sessionData == null) return;

            SessionCharacterData characterData = _characterRepo.GetCharacterData(_sessionId, sessionData.RawCharacterId);
            BaseCharacterSO characterSO = _characterConfig.GetCharacterById(sessionData.RawCharacterId);

            _characterHolder = new CharacterDataHolder.Builder()
                                   .WithCharacterData(characterData)
                                   .WithCharacterSO(characterSO)
                                   .Build();

            _exerciseList.ForEach(ex =>
            {
                TrainingExerciseSO _so = _exercisesConfig.GetExercise(ex.ExerciseId);
                ex.Initialize(_exercisesDAO, _characterDAO, _sessionId, _so);
            });

            _eventManager.Subscribe<UseItemEvent>(HandeUseItemEvent);
            _eventManager.Subscribe<TrainingCompletedEvent>(HandleTrainingCompletedEvent);
            _eventManager.Subscribe<NextWeekEvent>(HandleNextWeekEvent);
        }

        private void Start()
        {
            LoadCharacter();
        }

        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
            _eventManager.Unsubscribe<UseItemEvent>(HandeUseItemEvent);
            _eventManager.Unsubscribe<TrainingCompletedEvent>(HandleTrainingCompletedEvent);
            _eventManager.Unsubscribe<NextWeekEvent>(HandleNextWeekEvent);
        }

        private void LoadExerciseStrategies()
        {
            _exerciseList.Clear();
            _exerciseList.AddRange(GetComponentsInChildren<ITrainingStrategy>());
        }

        private void LoadConfigs()
        {
            _exercisesConfig = ContextManager.Singleton.GetDataSO<TrainingExerciseConfigSO>();
            _characterConfig = ContextManager.Singleton.GetDataSO<CharacterConfigSO>();
        }

        private void LoadDAOs()
        {
            _sessionDAO = new GameSessionDAO();
            _characterRepo = new PlayerCharacterRepository();
            _exercisesDAO = new TrainingExerciseDAO();
            _characterDAO = new PlayerCharacterDAO();
            _supportItemsService = new SupportItemsService();
        }

        private void LoadCharacter()
        {
            string poolName = "Character";
            Transform characterTrans = PoolManager.Pools[poolName].Spawn(_characterHolder.GetPrefab(), m_CharacterContainer);
            _characterAnimator = characterTrans.GetComponent<Animator>();
        }

        public void OpenMainMenuBG()
        {
            m_MainMenuBG.SetActive(true);
            _characterAnimator.gameObject.SetActive(true);
            _characterAnimator.transform.SetParent(m_CharacterContainer.transform, false);
            _characterAnimator.transform.localPosition = Vector3.zero;
        }

        public void HideMainMenuBG()
        {
            m_MainMenuBG.SetActive(false);
        }

        public void OpenTrainingMenuBG()
        {
            m_TrainingMenuBG.SetActive(true);
            _characterAnimator.gameObject.SetActive(true);
            _characterAnimator.transform.SetParent(m_TrainingCharContainer.transform, false);
            _characterAnimator.transform.localPosition = Vector3.zero;
        }

        public void HideTrainingMenuBG()
        {
            m_TrainingMenuBG.SetActive(false);
        }

        public void StartTraining(ETrainingType trainingType)
        {
            ITrainingStrategy strategy = GetExerciseByType(trainingType);
            if (strategy == null) return;
            ProcessTrainingLogic(strategy);
        }

        public void StartTraining(string id)
        {
            ITrainingStrategy strategy = GetExerciseById(id);
            if (strategy == null) return;
            ProcessTrainingLogic(strategy);
        }

        public List<StatChange> GetSimulatedStatChanges(string id)
        {
            ITrainingStrategy strategy = GetExerciseById(id);
            if (strategy != null)
            {
                return strategy.SimulateTrainingRewards(_characterHolder);
            }
            return null;
        }

        private void ProcessTrainingLogic(ITrainingStrategy strategy)
        {
            TrainingResult result = strategy.StartTraining(_characterHolder);

            UITrainingMenuFrame menuFrame = GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_TRAINING_MENU).AsFrame<UITrainingMenuFrame>();

            if (m_OverlayPrefab != null && m_UICanvas != null && _activeOverlayInstance == null)
            {
                _activeOverlayInstance = Instantiate(m_OverlayPrefab, m_UICanvas);
                _activeOverlayInstance.transform.SetAsLastSibling();
            }

            TrainingAnimData animData = m_TrainingAnimConfig.GetById(_characterHolder.GetRawId());

            if (animData != null && animData.prefab != null)
            {
                if (menuFrame != null) menuFrame.HideUIForAnimation();
                _characterAnimator.gameObject.SetActive(false);

                if (_currentAnimController != null)
                {
                    _currentAnimController.StopAllAnimations();
                    Destroy(_currentAnimController.gameObject);
                }

                _currentAnimController = Instantiate(animData.prefab, m_TrainingAnimContainer);

                _currentAnimController.PlayTrainingAnim(strategy.TrainingType, () =>
                {
                    if (_currentAnimController != null)
                    {
                        Destroy(_currentAnimController.gameObject);
                        _currentAnimController = null;
                    }

                    _characterAnimator.gameObject.SetActive(true);
                    if (menuFrame != null) menuFrame.ShowUIAfterAnimation();

                    if (_activeOverlayInstance != null)
                    {
                        _activeOverlayInstance.transform.SetAsLastSibling();
                    }

                    var frame = GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_TRAINING_RESULT).AsFrame<UITrainingResultFrame>();
                    frame.SetResult(strategy.DataHolder.GetName(), result);
                });
            }
            else
            {
                if (_activeOverlayInstance != null) _activeOverlayInstance.transform.SetAsLastSibling();

                var frame = GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_TRAINING_RESULT).AsFrame<UITrainingResultFrame>();
                frame.SetResult(strategy.DataHolder.GetName(), result);
            }
        }

        private void HandleTrainingCompletedEvent(TrainingCompletedEvent ev)
        {
            if (_activeOverlayInstance != null)
            {
                Destroy(_activeOverlayInstance);
                _activeOverlayInstance = null;
            }

            UITrainingMenuFrame menuFrame = GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_TRAINING_MENU) as UITrainingMenuFrame;
            if (menuFrame != null)
            {
                menuFrame.LoadStats();
            }
        }

        public bool CanJoinTraining()
        {
            return _characterHolder.GetEnergy() > 0;
        }

        public ITrainingStrategy[] GetAllTrainings()
        {
            return _exerciseList.ToArray();
        }

        private ITrainingStrategy GetExerciseByType(ETrainingType trainingType)
        {
            return _exerciseList.FirstOrDefault(ex => ex.TrainingType == trainingType);
        }

        private ITrainingStrategy GetExerciseById(string id)
        {
            return _exerciseList.FirstOrDefault(ex => ex.ExerciseId.Equals(id));
        }

        public float GetFailRate()
        {
            return GetFailRate(CharacterData.GetEnergy());
        }

        public float GetFailRate(float currentEnergy)
        {
            if (currentEnergy >= 50f) return 0f;

            float failRate = ((50f - currentEnergy) / 30f) * 100f;
            float finalRawRate = Mathf.Clamp(failRate, 0f, 100f);
            float roundedRate = (float)System.Math.Round(finalRawRate, 2);
            return roundedRate;
        }

        private void HandeUseItemEvent(UseItemEvent ev)
        {
            if (ev == null || ev.ItemData == null)
            {
                return;
            }

            switch (ev.ItemData.GetItemType())
            {
                case EItemType.Consumable:
                    foreach (var mod in ev.ItemData.GetUsableModifiers())
                    {
                        if (mod.StatType == EStatusType.TrainingEffective)
                        {
                            if (_supportItemsService.StackSupportItem(ev.ItemData.GetSessionId(), ev.ItemData.GetRawId()))
                            {
                                Debug.Log($"<color=green>[GameTrainingController]</color> Upsert support training modifier completed");
                            }
                            else
                            {
                                Debug.Log($"<color=red>[GameTrainingController]</color> Upsert support training modifier failed");
                            }
                            continue;
                        }

                        EStatusType statType = mod.StatType;
                        float modValue = mod.GetModifiedStatus(_characterHolder.GetStatus(statType));
                        _characterHolder.SetStatus(statType, modValue);
                    }

                    _characterHolder.UpdateChanges(_characterDAO);
                    LocalDBOrchestrator.UpdateDBChangeTime();
                    break;
                default:
                    Debug.Log($"<color=yellow>[GameTrainingController.HandleUseItemEvent]</color> Unsupported item type {ev.ItemData.GetItemType().ToString()}");
                    break;
            }
        }

        private void HandleNextWeekEvent(NextWeekEvent ev)
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                return;
            }
            _supportItemsService.ClearAllItems(_sessionId);
        }
    }

    public class TrainingCompletedEvent : IEvent { }
}