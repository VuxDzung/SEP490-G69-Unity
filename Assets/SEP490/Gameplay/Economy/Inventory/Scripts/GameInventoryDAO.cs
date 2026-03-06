namespace SEP490G69.Economy
{
    using LiteDB;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GameInventoryDAO 
    {
        public const string COLLECTION_NAME = "INVENTORY";

        private LiteDatabase _database;
        private ILiteCollection<ItemData> _collection;

        public GameInventoryDAO()
        {
            _database = LocalDBInitiator.GetDatabase();
            _collection = _database.GetCollection<ItemData>(COLLECTION_NAME);

            _collection.EnsureIndex(x => x.SessionId);
            _collection.EnsureIndex(x => x.RawItemId);
        }

        public ItemData GetItem(string sessionId, string rawItemId)
        {
            return _collection.FindOne(
                x => x.SessionId == sessionId && x.RawItemId == rawItemId);
        }

        public List<ItemData> GetAllItems(string sessionId)
        {
            return _collection.Find(x => x.SessionId == sessionId).ToList();
        }

        public void Insert(ItemData item)
        {
            _collection.Insert(item);
        }

        public void Update(ItemData item)
        {
            _collection.Update(item);
        }

        public void Delete(string sessionItemId)
        {
            _collection.Delete(sessionItemId);
        }

        public EquipmentData GetEquippedRelic(string sessionId, int slot)
        {
            return _collection
                .Find(x => x.SessionId == sessionId)
                .OfType<EquipmentData>()
                .FirstOrDefault(x => x.Slot == slot);
        }

        public EquipmentData GetRelic(string sessionId, string relicId)
        {
            return _collection
                .Find(x => x.SessionId == sessionId && x.RawItemId == relicId)
                .OfType<EquipmentData>()
                .FirstOrDefault();
        }
    }
}