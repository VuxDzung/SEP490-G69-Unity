namespace SEP490G69.Economy
{
    using LiteDB;
    using UnityEngine;

    public class ShopItemData 
    {
        #region Identifiers
        [BsonId] 
        public string SessionItemId { get; set; }
        public string SessionId { get; set; }
        public string RawItemId { get; set; }
        #endregion

        public int RemainAmount { get; set; }
    }
}