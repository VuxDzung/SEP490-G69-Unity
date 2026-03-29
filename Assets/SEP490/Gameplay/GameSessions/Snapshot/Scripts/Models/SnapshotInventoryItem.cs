using LiteDB;

namespace SEP490G69.GameSessions
{
    public class SnapshotInventoryItem
    {
        #region Identifier
        public string SessionItemId { get; set; }
        public string SessionId { get; set; }
        public string RawItemId { get; set; }
        #endregion

        /// <summary>
        /// Stack item amount
        /// </summary>
        public int RemainAmount { get; set; }
    }
}
