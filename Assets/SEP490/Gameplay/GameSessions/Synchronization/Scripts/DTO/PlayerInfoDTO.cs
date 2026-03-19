namespace SEP490G69.GameSessions
{
    using System;
    public class PlayerInfoDTO
    {
        public string PlayerId { get; set; } = string.Empty;

        public string PlayerName { get; set; } = string.Empty;
        public string PlayerEmail { get; set; } = string.Empty;
        public int LegacyPoints { get; set; }
        public int CurrentRun { get; set; }
        public DateTime LastSyncedTime { get; set; }

        public DeviceInfo LastSyncedDevice { get; set; } = null;
    }
}