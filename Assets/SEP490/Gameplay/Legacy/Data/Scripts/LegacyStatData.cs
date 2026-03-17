namespace SEP490G69.Legacy
{
    using LiteDB;

    public class LegacyStatData 
    {
        /// <summary>
        /// The legacy stat data id consists of: <PLAYER_ID>_<RAW_LEGACY_STAT_ID>
        /// </summary>
        [BsonId]
        public string Id { get; set; }

        public string PlayerId { get; set; }

        public string RawLegacyStatId { get; set; }

        public int Level { get; set; }
    }
}