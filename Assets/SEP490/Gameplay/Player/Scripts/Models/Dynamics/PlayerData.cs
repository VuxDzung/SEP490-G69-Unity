using LiteDB;
using System;

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

        /// <summary>
        /// Latest time which the frontend sync with backend.
        /// </summary>
        public DateTime LastSyncTime { get; set; }

        /// <summary>
        /// Lastest time which the local frontend update the database.
        /// </summary>
        public DateTime LastUpdatedTime { get; set; }

        /// <summary>
        /// How many runs has the player played.
        /// </summary>
        public int RunCount { get; set; }
    }
}