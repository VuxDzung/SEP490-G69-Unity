using LiteDB;

namespace SEP490G69.GameSessions
{
    public class PlayerTrainingSession 
    {
        [BsonId]
        public string SessionId { get; set; }
        public string PlayerId { get; set; }

        public string RawCharacterId { get; set; }

        public int CurrentWeek { get; set; } = 0;
        public int CurrentGoldAmount { get; set; } = 0;

        public int RefreshShopCount { get; set; } = 0;

        public string ActiveTournamentId { get; set; } = string.Empty;
    }
}