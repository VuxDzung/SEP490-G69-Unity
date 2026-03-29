using LiteDB;

namespace SEP490G69.GameSessions
{
    public class PlayerTrainingSession 
    {
        [BsonId]
        public string SessionId { get; set; }
        public string PlayerId { get; set; }

        // Character
        public string RawCharacterId { get; set; }

        // Time & Money
        public int CurrentWeek { get; set; } = 0;
        public int CurrentGoldAmount { get; set; } = 0;

        // Shop
        public int RefreshShopCount { get; set; } = 0;


        // Tournaments
        public string ActiveTournamentId { get; set; } = string.Empty;
        public int WinTournamentCount { get; set; } = 0;
    }
}