namespace SEP490G69.Training
{
    using UnityEngine;

    public class BoxingTrainingStrategy : BaseTrainingStrategy
    {

        public override bool StartTraining(CharacterDataHolder character)
        {
            float currentEnergy = character.GetEnergy();
            float currentMood = character.GetMood();
            int facilityLevel = _exerciseDataHolder.GetSessionData().Level;

            float failRate = GetFailRate(currentEnergy);
            bool isSuccess = UnityEngine.Random.Range(0f, 100f) >= failRate;

            if (isSuccess)
            {
                // Tính Mood Multiplier
                float moodMultiplier = GetMoodEffectiveness(currentMood);

                // --- 1. TRỪ ENERGY ---
                var energyReward = _exerciseDataHolder.GetSuccessRewardByType(EStatusType.Energy);
                float rawEnergyGain = energyReward.Modifier.GetModifiedStatus(character.GetEnergy());
                // Energy thường không có BonusPerLevel hoặc Mood, nên cộng thẳng
                character.SetEnergy(rawEnergyGain);

                // --- 2. CỘNG POWER TỪ FACILITY ---
                var powerReward = _exerciseDataHolder.GetSuccessRewardByType(EStatusType.Power);
                float rawPowerGain = powerReward.Modifier.GetModifiedStatus(character.GetPower());

                // Công thức: Tổng Power = (Base + (Level - 1) * Bonus) * Mood
                float facilityPowerGain = rawPowerGain + (powerReward.BonusPerLevel * (facilityLevel - 1));
                float finalPowerGain = facilityPowerGain * moodMultiplier;

                character.SetPower(finalPowerGain);

                return true;
            }
            else
            {
                // Xử lý nhánh Fail tương tự, lấy FailedRewardByType, trừ Energy, trừ Mood...
                // Và nhớ: finalPowerGain nhánh fail = facilityPowerGain * 0.1f;
                return false;
            }
        }
    }
}