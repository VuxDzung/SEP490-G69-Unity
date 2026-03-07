namespace SEP490G69
{
    using LiteDB;

    /// <summary>
    /// Represent the player's deck.
    /// </summary>
    public class SessionPlayerDeck 
    {
        [BsonId]
        public string SessionId { get; set; }
        public string[] CardIds { get; set; }
    }
}