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

        private void OnEnable()
        {
            ContextManager.Singleton.AddSceneContext(this);
        }
        private void OnDisable()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
        }

        private void Start()
        {
            LoadConfigs();
            LoadDAOs();
            LoadExerciseStrategies();

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

            LoadCharacter();
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

        public void StartTraining(ETrainingType trainingType)
        {
            ITrainingStrategy strategy = GetExerciseByType(trainingType);
            if (strategy == null) return;

            strategy.StartTraining(_characterHolder);
        }
        public void StartTraining(string id)
        {
            ITrainingStrategy strategy = GetExerciseById(id);
            if (strategy == null) return;

            strategy.StartTraining(_characterHolder);
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
}