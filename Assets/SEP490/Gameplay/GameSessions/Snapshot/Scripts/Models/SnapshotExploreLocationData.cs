namespace SEP490G69.GameSessions
{
    public class SnapshotExploreLocationData
    {
        public string EntityId { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;

        public bool IsBossDefeated { get; set; } = false;
        public int ExplorationCount { get; set; } = 0;
    }
}
