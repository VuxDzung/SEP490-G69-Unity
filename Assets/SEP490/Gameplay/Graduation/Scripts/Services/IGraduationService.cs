namespace SEP490G69
{
    public interface IGraduationService 
    {
        public float CalculateFinalRating(SessionCharacterData characterData, int totalCardCount, int totalRelicCount);
        public int CalculateLPGained(float rating);
    }
}