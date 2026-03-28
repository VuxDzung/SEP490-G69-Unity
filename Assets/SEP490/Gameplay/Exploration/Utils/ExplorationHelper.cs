namespace SEP490G69.Exploration
{
    public static class ExplorationHelper
    {
        public const string FORMAT_PENDING_EVENT_OUTCOME_ID = "{0}:{1}:{2}";

        public static string ConstructPendingEventOutcomeId(string eventId, int choiceIndex, int outcomeIndex)
        {
            string outcomeId = string.Format(FORMAT_PENDING_EVENT_OUTCOME_ID, eventId, choiceIndex, outcomeIndex);// $"{eventId}:{choiceIndex}:{outcomeIndex}";
            return outcomeId;
        }
    }
}