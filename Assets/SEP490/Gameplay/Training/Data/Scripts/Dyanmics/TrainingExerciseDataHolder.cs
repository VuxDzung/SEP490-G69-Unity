namespace SEP490G69.Training
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class TrainingExerciseDataHolder 
    {
        private TrainingExerciseSO _so;
        private SessionTrainingExercise _trainingData;

        public SessionTrainingExercise GetSessionData()
        {
            return _trainingData;
        }

        public bool CanTrainingSuccess(float energy, float mood)
        {
            return true;
        }

        public float CalculateTrainingSuccessPercentage()
        {
            return 0;
        }

        public TrainingRewardConfig GetSuccessRewardByType(EStatusType statType)
        {
            return _so.SuccessModifiers.FirstOrDefault(config => config.Modifier != null && config.Modifier.StatType == statType);
        }

        public TrainingRewardConfig GetFailedRewardByType(EStatusType statType)
        {
            return _so.FailedModifiers.FirstOrDefault(config => config.Modifier != null && config.Modifier.StatType == statType);
        }

        public List<TrainingRewardConfig> GetAllSuccessModifiers()
        {
            return _so.SuccessModifiers;
        }

        public List<TrainingRewardConfig> GetAllFailedModifiers()
        {
            return _so.FailedModifiers;
        }

        public class Builder
        {
            private TrainingExerciseSO _exerciseSO;
            private SessionTrainingExercise _trainingData;

            public Builder WithExerciseSO(TrainingExerciseSO exerciseSO)
            {
                _exerciseSO = exerciseSO;
                return this;
            }
            public Builder WithSessionTrainingData(SessionTrainingExercise trainingData)
            {
                _trainingData = trainingData;
                return this;
            }

            public TrainingExerciseDataHolder Build()
            {
                TrainingExerciseDataHolder holder = new TrainingExerciseDataHolder
                {
                    _so = _exerciseSO,
                    _trainingData = _trainingData
                };
                return holder;
            }
        }
    }
}