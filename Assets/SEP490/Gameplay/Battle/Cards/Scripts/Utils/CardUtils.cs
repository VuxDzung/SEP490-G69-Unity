namespace SEP490G69.Battle.Cards
{
    public class CardUtils
    {
        public static string ExtractRawCardId(string deckCardId)
        {
            return deckCardId.Split(':')[1];
        }
    }
}