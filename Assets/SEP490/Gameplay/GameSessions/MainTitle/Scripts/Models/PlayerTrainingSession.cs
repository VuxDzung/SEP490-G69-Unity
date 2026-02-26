using LiteDB;

namespace SEP490G69.GameSessions
{
    public class PlayerTrainingSession 
    {
        [BsonId]
        public string SessionId { get; set; }
        public string PlayerId { get; set; }
        public string CharacterId { get; set; }

        /// <summary>
        /// Status incudes
        /// - CHARACTER SELECTION
        /// - IN PROGRESS
        /// - COMPLETED
        /// </summary>
        public string Status { get; set; }

        public int CurrentWeek { get; set; } = 0;
        public int CurrentGoldAmount { get; set; } = 0;
    }
}