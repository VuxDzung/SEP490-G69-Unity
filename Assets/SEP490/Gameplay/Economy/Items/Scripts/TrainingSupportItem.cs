namespace SEP490G69
{
    using LiteDB;
    using UnityEngine;

    [System.Serializable]
    public class TrainingSupportItem 
    {
        [BsonId]
        public string EntityId { get; set; }

        public string SessionId { get; set; }
        public string RawItemId { get; set; }

        public int RemainAmount { get; set; }
    }
}