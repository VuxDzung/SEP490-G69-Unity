namespace SEP490G69.Economy
{
    using LiteDB;
    using System.Linq;
    using System.Collections.Generic;

    public class GameShopDAO
    {
        public const string COLLECTION_NAME = "SHOP";

        private LiteDatabase _database;
        private ILiteCollection<ShopItemData> _collection;

        public GameShopDAO()
        {
            _database = LocalDBInitiator.GetDatabase();
            _collection = _database.GetCollection<ShopItemData>(COLLECTION_NAME);

            _collection.EnsureIndex(x => x.SessionId);
            _collection.EnsureIndex(x => x.RawItemId);
        }

        public List<ShopItemData> GetAll(string sessionId)
        {
            return _collection.Find(x => x.SessionId == sessionId).ToList();
        }

        public ShopItemData Get(string sessionId, string rawItemId)
        {
            return _collection.FindOne(x =>
                x.SessionId == sessionId &&
                x.RawItemId == rawItemId);
        }

        public void Insert(ShopItemData data)
        {
            _collection.Insert(data);
        }

        public void Update(ShopItemData data)
        {
            _collection.Update(data);
        }

        public void Delete(string sessionItemId)
        {
            _collection.Delete(sessionItemId);
        }

        public void ClearSession(string sessionId)
        {
            _collection.DeleteMany(x => x.SessionId == sessionId);
        }
    }
}