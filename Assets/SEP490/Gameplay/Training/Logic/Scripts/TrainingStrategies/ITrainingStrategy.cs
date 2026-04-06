using System.Collections.Generic;

namespace SEP490G69.Training
{
    public interface ITrainingStrategy 
    {
        public string ExerciseId { get; }
        public ETrainingType TrainingType { get; }
        public TrainingExerciseDataHolder DataHolder { get; }

        public void Initialize(TrainingExerciseDAO dao, PlayerCharacterDAO characterDAO, string sessionId, TrainingExerciseSO exerciseSO);
        public TrainingResult StartTraining(CharacterDataHolder characterHolder, int bindedItemAmount);
        public List<StatChange> SimulateTrainingRewards(CharacterDataHolder characterHolder, int bindedItemAmount);
    }
}