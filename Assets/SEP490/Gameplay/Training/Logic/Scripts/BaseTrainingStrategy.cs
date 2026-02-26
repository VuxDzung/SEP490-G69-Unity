namespace SEP490G69.Training
{
    using UnityEngine;

    public abstract class BaseTrainingStrategy : MonoBehaviour, ITrainingStrategy
    {
        [SerializeField] protected string m_ExerciseId;
        [SerializeField] protected ETrainingType m_TrainingType;

        protected TrainingExerciseDataHolder _exerciseDataHolder;
        protected TrainingExerciseDAO _trainingDAO;

        public ETrainingType TrainingType => m_TrainingType;
        public string ExerciseId => m_ExerciseId;
        public TrainingExerciseDataHolder DataHolder => _exerciseDataHolder;

        /// <summary>
        /// Logic dùng chung để khởi tạo dữ liệu bài tập từ DAO
        /// </summary>
        public virtual void Initialize(TrainingExerciseDAO dao, string sessionId, TrainingExerciseSO exerciseSO)
        {
            _trainingDAO = dao;
            SessionTrainingExercise exerciseData = _trainingDAO.GetByIdAndSessionId(sessionId, exerciseSO.ExerciseId);

            if (exerciseData == null)
            {
                Debug.Log($"Existed data does not exist. Create new data for training exercise {exerciseSO.ExerciseId}");
                string id = $"{sessionId}:{exerciseSO.ExerciseId}";
                exerciseData = new SessionTrainingExercise
                {
                    Id = id,
                    SessionId = sessionId,
                    ExerciseId = exerciseSO.ExerciseId,
                    Level = GameConstants.TRAINING_STARTER_LEVEL,
                };

                _trainingDAO.InsertTrainingExercise(exerciseData);
            }

            _exerciseDataHolder = new TrainingExerciseDataHolder.Builder()
                                  .WithExerciseSO(exerciseSO)
                                  .WithSessionTrainingData(exerciseData)
                                  .Build();
        }

        public abstract bool StartTraining(CharacterDataHolder character);

        /// <summary>
        /// Tính tỷ lệ thất bại 
        /// </summary>
        protected float GetFailRate(float currentEnergy)
        {
            if (currentEnergy >= 50f) return 0f; 

            float failRate = ((50f - currentEnergy) / 30f) * 100f;
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