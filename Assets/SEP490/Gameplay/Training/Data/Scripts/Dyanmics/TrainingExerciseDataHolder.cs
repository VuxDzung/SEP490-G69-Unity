namespace SEP490G69.Training
{
    using System.Linq;
    using UnityEngine;

    public class TrainingExerciseDataHolder 
    {
        private TrainingExerciseSO _so;
        private SessionTrainingExercise _trainingData;

        public bool CanTrainingSuccess(float energy, float mood)
        {
            return true;
        }

        public StatusModifierSO GetSuccessModifierByType(EStatusType statType)
        {
            return _so.SuccessModifiers.FirstOrDefault(_so => _so.StatType == statType);
        }
        public StatusModifierSO GetFailedModifierByType(EStatusType statType)
        {
            return _so.FailedModifiers.FirstOrDefault(_so => _so.StatType == statType);
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