namespace SEP490G69.Training
{
    using System.Collections.Generic;
    using System.Linq;
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

        [Header("Training Animation Prefabs")]
        [SerializeField] private SakuraTrainingAnimationController _sakuraAnimPrefab;

        // THAY ĐỔI: Chuyển sang dùng Prefab và sinh ra động
        [Header("UI System")]
        [SerializeField] private GameObject m_OverlayPrefab;     // Kéo Prefab Overlay vào đây
        [SerializeField] private Transform m_UICanvas;           // Kéo UICanvas trên Scene vào đây

        private GameObject _activeOverlayInstance;               // Biến lưu trữ Overlay đang hiển thị

        private EventManager _eventManager;

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

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            if (string.IsNullOrEmpty(sessionId)) return;

            PlayerTrainingSession sessionData = _sessionDAO.GetById(sessionId);
            if (sessionData == null) return;

            SessionCharacterData characterData = _characterRepo.GetCharacterData(sessionId, sessionData.RawCharacterId);
            BaseCharacterSO characterSO = _characterConfig.GetCharacterById(sessionData.RawCharacterId);

            _characterHolder = new CharacterDataHolder.Builder()
                                   .WithCharacterData(characterData)
                                   .WithCharacterSO(characterSO)
                                   .Build();

            _exerciseList.ForEach(ex =>
            {
                TrainingExerciseSO _so = _exercisesConfig.GetExercise(ex.ExerciseId);
                ex.Initialize(_exercisesDAO, _characterDAO, sessionId, _so);
            });

            _eventManager.Subscribe<UseItemEvent>(HandeUseItemEvent);
            _eventManager.Subscribe<TrainingCompletedEvent>(HandleTrainingCompletedEvent);
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

        // ================== LOGIC GỌI TRAINING VÀ HOẠT ẢNH ==================
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

        private void ProcessTrainingLogic(ITrainingStrategy strategy)
        {
            TrainingResult result = strategy.StartTraining(_characterHolder);
            UITrainingMenuFrame menuFrame = GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_TRAINING_MENU) as UITrainingMenuFrame;

            // 1. SPAWN OVERLAY VÀO CANVAS NGAY KHI VỪA BẤM TẬP
            if (m_OverlayPrefab != null && m_UICanvas != null && _activeOverlayInstance == null)
            {
                // Instantiate sinh ra Overlay và tự động nhét vào làm con của UICanvas
                _activeOverlayInstance = Instantiate(m_OverlayPrefab, m_UICanvas);
                _activeOverlayInstance.transform.SetAsLastSibling(); // Ép xuống đáy để che mọi thứ
            }

            if (_characterHolder.GetRawId() == "ch_0003" && _sakuraAnimPrefab != null)
            {
                if (menuFrame != null) menuFrame.HideUIForAnimation();
                _characterAnimator.gameObject.SetActive(false);

                SakuraTrainingAnimationController animInstance = Instantiate(_sakuraAnimPrefab);

                animInstance.PlayTrainingAnim(strategy.TrainingType, () =>
                {
                    Destroy(animInstance.gameObject);

                    _characterAnimator.gameObject.SetActive(true);
                    if (menuFrame != null) menuFrame.ShowUIAfterAnimation();

                    // 2. ÉP OVERLAY XUỐNG DƯỚI CÙNG 1 LẦN NỮA TRƯỚC KHI BUNG POPUP
                    if (_activeOverlayInstance != null)
                    {
                        _activeOverlayInstance.transform.SetAsLastSibling();
                    }

                    // Hệ thống UIManager sẽ tự động nhét Popup này nằm DƯỚI cái Overlay trong Hierarchy (nghĩa là nổi lên trên cùng)
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

        // ================== HỦY OVERLAY KHI ĐÓNG POPUP ==================
        private void HandleTrainingCompletedEvent(TrainingCompletedEvent ev)
        {
            // Xóa sổ cái Overlay khỏi Scene
            if (_activeOverlayInstance != null)
            {
                Destroy(_activeOverlayInstance);
                _activeOverlayInstance = null;
            }

            // Cập nhật lại thanh máu
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
            if (ev.ItemData.GetItemType() == EItemType.Consumable)
            {
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
            }
        }
    }

    public class TrainingCompletedEvent : IEvent { }
}