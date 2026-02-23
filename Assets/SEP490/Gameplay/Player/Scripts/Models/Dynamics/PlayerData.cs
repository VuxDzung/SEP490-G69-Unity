using LiteDB;

namespace SEP490G69
{
    public class PlayerData 
    {
        [BsonId]
        public string PlayerId { get; set; }

        public string PlayerName { get; set; }
        public string PlayerEmail { get; set; }
        public int LegacyPoints { get; set; }
        public int TotalPlayTime { get; set; }
        public bool IsSynced { get; set; }
    }
}