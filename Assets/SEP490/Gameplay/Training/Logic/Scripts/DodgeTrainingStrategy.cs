namespace SEP490G69.Training
{
    using UnityEngine;

    public class DodgeTrainingStrategy : BaseTrainingStrategy
    {
        public override bool StartTraining(CharacterDataHolder character)
        {
            float currentEnergy = character.GetEnergy();
            float currentMood = character.GetMood();
            int facilityLevel = _exerciseDataHolder.GetSessionData().Level;

            float failRate = GetFailRate(currentEnergy);
            bool isSuccess = UnityEngine.Random.Range(0f, 100f) >= failRate;

            var statReward = _exerciseDataHolder.GetSuccessRewardByType(EStatusType.Agi);
            float rawStatGain = statReward.Modifier != null ? statReward.Modifier.GetModifiedStatus(character.GetAgi()) : 0f;
            float facilityStatGain = rawStatGain + (statReward.BonusPerLevel * (facilityLevel - 1));

            if (isSuccess)
            {
                float moodMultiplier = GetMoodEffectiveness(currentMood);

                var energyReward = _exerciseDataHolder.GetSuccessRewardByType(EStatusType.Energy);
                if (energyReward.Modifier != null)
                    character.SetEnergy(energyReward.Modifier.GetModifiedStatus(currentEnergy));

                character.SetAgi(facilityStatGain * moodMultiplier);
                return true;
            }
            else
            {
                var failEnergyReward = _exerciseDataHolder.GetFailedRewardByType(EStatusType.Energy);
                if (failEnergyReward.Modifier != null)
                    character.SetEnergy(failEnergyReward.Modifier.GetModifiedStatus(currentEnergy));

                var failMoodReward = _exerciseDataHolder.GetFailedRewardByType(EStatusType.Mood);
                if (failMoodReward.Modifier != null)
                    character.SetMood(failMoodReward.Modifier.GetModifiedStatus(currentMood));

                character.SetAgi(facilityStatGain * 0.1f);
                return false;
            }
        }
    }
}