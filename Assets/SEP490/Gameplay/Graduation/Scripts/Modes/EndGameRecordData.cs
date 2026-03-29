using LiteDB;
namespace SEP490G69.Graduation
{
    public class EndGameRecordData
    {
        /// <summary>
        /// Format <PLAYER_ID>:<RUN_COUNT>
        /// </summary>
        [BsonId]
        public string RecordId { get; set; }
        public string PlayerId { get; set; }
        public int RunChallegeCount { get; set; }
        public string RawCharacterId { get; set; }

        public string Title { get; set; }
        public string EndingType { get; set; }
        public float Rating { get; set; }
        public int ObtainedCardCount { get; set; }
        public int ObtainedRelicCount { get; set; }
        public int LPGained {  get; set; }
        public int WinTournamentCount { get; set; }
        public System.DateTime EndGameDate { get; set; } = default;
    }
}