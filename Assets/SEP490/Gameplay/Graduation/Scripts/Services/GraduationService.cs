namespace SEP490G69.Graduation
{
    using UnityEngine;

    public class GraduationService : IGraduationService
    {
        public float CalculateFinalRating(SessionCharacterData characterData, int totalCardCount, int totalRelicCount)
        {
            float totalStats = characterData.CurrentMaxVitality + characterData.CurrentPower + characterData.CurrentIntelligence + characterData.CurrentStamina + characterData.CurrentDef + characterData.CurrentAgi;
            float rating = totalStats + (totalCardCount * 50f) + (totalRelicCount * 50f);

            return (float)System.Math.Round(rating, 2);
        }

        public int CalculateLPGained(float rating)
        {
            return Mathf.RoundToInt(rating * 0.02f);
        }
    }
}