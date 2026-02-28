namespace SEP490G69
{
    using LiteDB;

    public class SessionPlayerDeck 
    {
        [BsonId]
        public string SessionId { get; set; }
        public string[] CardIds { get; set; }
    }
}