namespace SEP490G69.Training
{
    using SEP490G69.Economy;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public abstract class BaseTrainingStrategy : MonoBehaviour, ITrainingStrategy
    {
        [SerializeField] protected string m_ExerciseId;
        [SerializeField] protected ETrainingType m_TrainingType;

        protected TrainingExerciseDataHolder _exerciseDataHolder;
        protected TrainingExerciseDAO _trainingDAO;
        protected PlayerCharacterDAO _characterDAO;
        private SupportItemsService _supportItemsRepository;
        protected string _sessionId;

        public ETrainingType TrainingType => m_TrainingType;
        public string ExerciseId => m_ExerciseId;
        public TrainingExerciseDataHolder DataHolder => _exerciseDataHolder;

        /// <summary>
        /// Logic dùng chung để khởi tạo dữ liệu bài tập từ DAO
        /// </summary>
        public virtual void Initialize(TrainingExerciseDAO dao, PlayerCharacterDAO characterDAO, string sessionId, TrainingExerciseSO exerciseSO)
        {
            _trainingDAO = dao;
            _characterDAO = characterDAO;
            _supportItemsRepository = new SupportItemsService();

            _sessionId = sessionId;

            SessionTrainingExercise exerciseData = _trainingDAO.GetById(_sessionId, exerciseSO.ExerciseId);

            if (exerciseData == null)
            {
                Debug.Log($"Existed data does not exist. Create new data for training exercise {exerciseSO.ExerciseId}");
                string id = EntityIdConstructor.ConstructDBEntityId(_sessionId, exerciseSO.ExerciseId); 
                exerciseData = new SessionTrainingExercise
                {
                    Id = id,
                    SessionId = _sessionId,
                    ExerciseId = exerciseSO.ExerciseId,
                    Level = GameConstants.TRAINING_STARTER_LEVEL,
                };

                _trainingDAO.Insert(exerciseData);
            }

            _exerciseDataHolder = new TrainingExerciseDataHolder.Builder()
                                  .WithExerciseSO(exerciseSO)
                                  .WithSessionTrainingData(exerciseData)
                                  .Build();
        }

        public abstract TrainingResult StartTraining(CharacterDataHolder character);

        protected void ApplyRewards(CharacterDataHolder character, List<TrainingRewardConfig> rewards, bool isSuccess, float moodMultiplier, int facilityLevel, TrainingResult result)
        {
            foreach (var reward in rewards)
            {
                EStatusType statType = reward.Modifier.StatType;

                float before = character.GetStatus(statType);

                // Step 1: Base delta from reward
                float delta = reward.Modifier.GetDelta(before);

                // Step 2: Facility scaling
                delta += reward.BonusPerLevel * (facilityLevel - 1);

                // Step 3: Success / Fail
                if (isSuccess)
                    delta *= moodMultiplier;
                else
                    delta *= 0.1f; // Fail hiệu suất 10%

                // STEP 4: Character modifier
                StatusModifierSO charModifier = character.GetModifierByType(statType);
                if (charModifier != null)
                {
                    float modifierDelta = charModifier.GetDelta(delta);
                    delta += modifierDelta;
                }

                float after = before + delta;

                character.SetStatus(statType, after);
                character.UpdateChanges(_characterDAO);

                result.Changes.Add(new StatChange(statType, before, delta));
            }
        }

        /// <summary>
        /// Tính tỷ lệ thất bại 
        /// </summary>
        protected float GetFailRate(float currentEnergy)
        {
            if (currentEnergy >= 50f) return 0f; 

            float failRate = ((50f - currentEnergy) / 30f) * 100f;

            TrainingSupportItem supportItem = _supportItemsRepository.GetAllBySessionId(_sessionId).FirstOrDefault();
            ItemDataSO itemSO = _supportItemsRepository.ItemConfig.GetItemById(supportItem.RawItemId);
            StatusModifierSO trainingEffectiveMod = itemSO.GetModifiersByStatType(EStatusType.TrainingEffective).FirstOrDefault();

            failRate -= trainingEffectiveMod.Value;

            return Mathf.Clamp(failRate, 0f, 100f);
        }

        /// <summary>
        /// Tính hệ số hiệu quả theo tâm trạng 
        /// </summary>
        protected float GetMoodEffectiveness(float currentMood)
        {
            if (currentMood >= 80f) return 1.10f; // Great: +10%
            if (currentMood >= 60f) return 1.05f; // Good: +5%
            if (currentMood >= 40f) return 1.00f; // Neutral: 0%
            if (currentMood >= 20f) return 0.95f; // Bad: -5%
            return 0.90f;                         // Awful: -10%
        }
    }
}