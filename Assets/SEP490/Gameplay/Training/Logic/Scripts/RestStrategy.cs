namespace SEP490G69.Training
{
    using UnityEngine;

    public class RestStrategy : BaseTrainingStrategy
    {
        public override bool StartTraining(CharacterDataHolder character)
        {
            float currentEnergy = character.GetEnergy();
            float currentMood = character.GetMood();

            var energyReward = _exerciseDataHolder.GetSuccessRewardByType(EStatusType.Energy);
            if (energyReward.Modifier != null)
                character.SetEnergy(energyReward.Modifier.GetRawStatGain(currentEnergy));

            var moodReward = _exerciseDataHolder.GetSuccessRewardByType(EStatusType.Mood);
            if (energyReward.Modifier != null)
                character.SetMood(energyReward.Modifier.GetRawStatGain(currentMood));

            return true;
        }
    }
}