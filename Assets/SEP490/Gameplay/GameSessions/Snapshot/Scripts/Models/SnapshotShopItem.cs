namespace SEP490G69.GameSessions
{
    public class SnapshotShopItem
    {
        #region Identifiers
        public string SessionItemId { get; set; }
        public string SessionId { get; set; }
        public string RawItemId { get; set; }
        #endregion

        public int RemainAmount { get; set; }
    }
}
