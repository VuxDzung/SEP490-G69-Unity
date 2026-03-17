using LiteDB;

namespace SEP490G69.GameSessions
{
    public class PlayerTrainingSession 
    {
        [BsonId]
        public string SessionId { get; set; }
        public string PlayerId { get; set; }

        /// <summary>
        /// This is the raw character id
        /// Ex: ch_0001, ch_0002, etc.
        /// </summary>
        public string RawCharacterId { get; set; }

        public int CurrentWeek { get; set; } = 0;
        public int CurrentGoldAmount { get; set; } = 0;

        public string ActiveTournamentId { get; set; } = string.Empty;
    }
}