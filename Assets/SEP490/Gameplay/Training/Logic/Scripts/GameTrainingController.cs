namespace SEP490G69.Training
{
    using System.Collections.Generic;
    using System.Linq;
    using SEP490G69.GameSessions;
    using UnityEngine;

    public enum ETrainingType
    {
        Rest,
        Boxing,
        Run,
        Dodge,
        Study,
        Yoga,
    }

    public class GameTrainingController : MonoBehaviour, ISceneContext
    {
        [SerializeField] private Transform m_CharacterContainer;
        [SerializeField] private GameObject m_MainMenuBG;
        [SerializeField] private GameObject m_TrainingMenuBG;
        [SerializeField] private Transform m_TrainingCharContainer;

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

        private List<ITrainingStrategy> _exerciseList = new List<ITrainingStrategy>();

        public CharacterDataHolder CharacterData => _characterHolder;

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);

            LoadConfigs();
            LoadDAOs();
            LoadExerciseStrategies();

            _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            if (string.IsNullOrEmpty(sessionId))
            {
                return;
            }
            PlayerTrainingSession sessionData = _sessionDAO.GetSession(sessionId);

            if (sessionData == null)
            {
                return;
            }
            SessionCharacterData characterData = _characterRepo.GetCharacterData(sessionId, sessionData.CharacterId);

            Debug.Log($"Character: {characterData.Id}");

            BaseCharacterSO characterSO = _characterConfig.GetCharacter(sessionData.CharacterId);

            _characterHolder = new CharacterDataHolder.Builder()
                                   .WithCharacterData(characterData)
                                   .WithCharacterSO(characterSO)
                                   .Build();

            // Initialize exercises strategies.
            _exerciseList.ForEach(ex =>
            {
                TrainingExerciseSO _so = _exercisesConfig.GetExercise(ex.ExerciseId);
                ex.Initialize(_exercisesDAO, sessionId, _so);
            });

        }

        private void Start()
        {
            LoadCharacter();
        }

        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
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
            _sessionDAO = new GameSessionDAO(LocalDBInitiator.GetDatabase());
            _characterRepo = new PlayerCharacterRepository(LocalDBInitiator.GetDatabase());
            _exercisesDAO = new TrainingExerciseDAO(LocalDBInitiator.GetDatabase());
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

            strategy.StartTraining(_characterHolder);

            _eventManager.Publish<TrainingCompletedEvent>(new TrainingCompletedEvent());
        }

        public void StartTraining(string id)
        {
            ITrainingStrategy strategy = GetExerciseById(id);
            if (strategy == null) return;

            strategy.StartTraining(_characterHolder);
            _eventManager.Publish<TrainingCompletedEvent>(new TrainingCompletedEvent());
        }

        public bool CanJoinTraining()
        {
            return _characterHolder.GetEnergy() > 0;
        }

        private ITrainingStrategy GetExerciseByType(ETrainingType trainingType)
        {
            return _exerciseList.FirstOrDefault(ex => ex.TrainingType == trainingType);
        }
        private ITrainingStrategy GetExerciseById(string id)
        {
            return _exerciseList.FirstOrDefault(ex => ex.ExerciseId.Equals(id));
        }
    }

    public class TrainingCompletedEvent : IEvent
    {
        
    }
}