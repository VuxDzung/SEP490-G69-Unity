namespace SEP490G69.Battle.Cards
{
    public class CardUtils
    {
        public static string ExtractRawCardId(string deckCardId)
        {
            string[] parts = deckCardId.Split(':');
            if (parts.Length > 1)
                return parts[1];
            else
                return parts[0];
        }
    }
}