namespace SEP490G69.Training
{
    using UnityEngine;

    public class RunTrainingStrategy : BaseTrainingStrategy
    {
        public override TrainingResult StartTraining(CharacterDataHolder character)
        {
            TrainingResult result = new TrainingResult();

            float currentEnergy = character.GetStatus(EStatusType.Energy);
            float currentMood = character.GetStatus(EStatusType.Mood);
            int facilityLevel = _exerciseDataHolder.GetSessionData().Level;

            float failRate = GetFailRate(currentEnergy);
            bool isSuccess = UnityEngine.Random.Range(0f, 100f) >= failRate;

            result.IsSuccess = isSuccess;

            float moodMultiplier = GetMoodEffectiveness(currentMood);

            var rewards = isSuccess
                ? _exerciseDataHolder.GetSuccessRewards()
                : _exerciseDataHolder.GetFailedRewards();

            ApplyRewards(
                character,
                rewards,
                isSuccess,
                moodMultiplier,
                facilityLevel,
                result
            );

            return result;
        }
    }
}