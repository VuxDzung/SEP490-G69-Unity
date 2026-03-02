namespace SEP490G69.Training
{
    using UnityEngine;

    public class RestStrategy : BaseTrainingStrategy
    {
        public override bool StartTraining(CharacterDataHolder character)
        {

            float currentEnergy = character.GetEnergy();
            float currentMood = character.GetMood();

            float roll = UnityEngine.Random.Range(0f, 100f);
            float energyRecovery = 0f;
            float moodChange = 0f;

            if (roll < 10f)
            {
                energyRecovery = 70f;
                moodChange = 20f; 
                Debug.Log("Rest: Great Success!");
            }
            else if (roll < 85f)
            {
                energyRecovery = 50f;
                moodChange = 10f;
                Debug.Log("Rest: Normal Success!");
            }
            else if (roll < 95f)
            {
                energyRecovery = 30f;
                moodChange = 0f;
                Debug.Log("Rest: Minor Success!");
            }
            else
            {
                energyRecovery = 5f;
                moodChange = -10f;
                Debug.Log("Rest: Bad Rest...");
            }

            float finalEnergy = currentEnergy + energyRecovery;
            character.SetEnergy(finalEnergy);

            float finalMood = currentMood + moodChange;
            character.SetMood(finalMood);

            return true;
        }
    }
}