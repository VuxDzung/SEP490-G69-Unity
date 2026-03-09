using LiteDB;

namespace SEP490G69
{
    /// <summary>
    /// Represent the player's obtained card.
    /// </summary>
    public class SessionCardData 
    {
        /// <summary>
        /// Session card id apply the following format
        /// <SESSION_ID>:<RAW_CARD_ID>
        /// </summary>
        [BsonId]
        public string SessionCardId { get; set; }
        public string RawCardId { get; set; }
        public string SessionId {  get; set; }

        public int ObtainedAmount { get; set; }
    }
}