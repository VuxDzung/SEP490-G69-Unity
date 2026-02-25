namespace SEP490G69.Training
{
    using UnityEngine;

    public class RunTrainingStrategy : MonoBehaviour, ITrainingStrategy
    {
        [SerializeField] private string m_ExerciseId;
        [SerializeField] private ETrainingType m_TrainingType;

        private TrainingExerciseDataHolder _exerciseDataHolder;
        private TrainingExerciseDAO _trainingDAO;

        public ETrainingType TrainingType => m_TrainingType;
        public string ExerciseId => m_ExerciseId;

        /// <summary>
        /// Get the current training exercise data of the session. 
        /// If it does not exist, it means that this is the new session.
        /// At that point, the system creates a new training exercise data for the session.
        /// </summary>
        /// <param name="sessionId"></param>
        public void Initialize(TrainingExerciseDAO dao, string sessionId, TrainingExerciseSO exerciseSO)
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

        public bool CanTraining(CharacterDataHolder character)
        {
            throw new System.NotImplementedException();
        }

        public bool StartTraining(CharacterDataHolder characterHolder)
        {
            throw new System.NotImplementedException();
        }
    }
}