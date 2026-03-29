namespace SEP490G69.GameSessions
{
    using System.Collections.Generic;
    using LiteDB;

    [System.Serializable]
    public class SessionSnapshotData
    {
        /// <summary>
        /// Snapshot id includes: <SESSION_ID>:<SNAPSHOT_RAW_ID>
        /// </summary>
        [BsonId]
        public string SnapshotId {  get; set; } = string.Empty;

        public string SessionId { get; set; } = string.Empty;

        // Character data
        public SnapshotCharacterData CharacterData { get; set; }

        // Training exercises data.
        public List<SnapshotExerciseData> Exercises { get; set; } = new List<SnapshotExerciseData>();

        // Deck data
        public SnapshotDeckData Deck {  get; set; }

        // Card list.
        public List<SnapshotCardData> Cards { get; set; } = new List<SnapshotCardData>();

        // Item list
        public List<SnapshotInventoryItem> InventoryItems { get; set; } = new List<SnapshotInventoryItem>();

        // Shop item list
        public List<SnapshotShopItem> ShopItems { get; set; } = new List<SnapshotShopItem>();

        // Explore locations
        public List<SnapshotExploreLocationData> ExploreLocations { get; set; } = new List<SnapshotExploreLocationData>();

        // Pending training support items.

        // No need tournament progression data.
        // By default, after the player pass the tournament checkpoint,
        // the tournament data has already been deleted and the progression is preparing to move to a brand new next week.
    }
}