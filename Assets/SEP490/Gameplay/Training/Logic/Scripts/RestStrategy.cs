namespace SEP490G69.Training
{
    using UnityEngine;

    public class RestStrategy : BaseTrainingStrategy
    {
        public override bool StartTraining(CharacterDataHolder character)
        {

            float roll = UnityEngine.Random.Range(0f, 100f);
            float energyRecovery = 0f;


            if (roll < 10f)
            {
                energyRecovery = 70f;
                Debug.Log("Rest: Great Success!");
            }
            else if (roll < 85f)
            {
                energyRecovery = 50f;
                Debug.Log("Rest: Normal Success!");
            }
            else if (roll < 95f)
            {
                energyRecovery = 30f;
                Debug.Log("Rest: Minor Success!");
            }
            else
            {
                energyRecovery = 5f;
                Debug.Log("Rest: Bad Rest...");
            }


            character.SetEnergy(energyRecovery);

            var moodReward = _exerciseDataHolder.GetSuccessRewardByType(EStatusType.Mood);
            if (moodReward.Modifier != null)
            {
                float moodGain = moodReward.Modifier.GetModifiedStatus(character.GetMood());
                character.SetMood(moodGain);
            }

            return true;
        }
    }
}