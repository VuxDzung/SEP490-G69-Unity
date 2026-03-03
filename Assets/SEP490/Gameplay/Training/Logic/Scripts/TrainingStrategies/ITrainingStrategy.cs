namespace SEP490G69.Training
{
    public interface ITrainingStrategy 
    {
        public string ExerciseId { get; }
        public ETrainingType TrainingType { get; }
        public TrainingExerciseDataHolder DataHolder { get; }

        public void Initialize(TrainingExerciseDAO dao, string sessionId, TrainingExerciseSO exerciseSO);
        //public bool CanTraining(CharacterDataHolder character);
        public TrainingResult StartTraining(CharacterDataHolder characterHolder);
    }
}