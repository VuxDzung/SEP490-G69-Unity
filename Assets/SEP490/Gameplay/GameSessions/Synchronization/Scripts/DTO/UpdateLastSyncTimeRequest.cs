namespace SEP490G69.GameSessions
{
    using System;

    public class UpdateLastSyncTimeRequest
    {
        public string PlayerId { get; set; }
        public DateTime LastSyncTime { get; set; }
    }
}