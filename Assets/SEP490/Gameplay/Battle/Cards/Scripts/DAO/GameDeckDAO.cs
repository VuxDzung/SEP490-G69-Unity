namespace SEP490G69.Battle.Cards
{
    using LiteDB;
    using UnityEngine;

    public class GameDeckDAO
    {
        public const string COLLECTION_NAME = "PlayerDeck";

        private readonly LiteDatabase _database;

        private readonly ILiteCollection<SessionPlayerDeck> _collection;

        public GameDeckDAO()
        {
            _database = LocalDBInitiator.GetDatabase();
            _collection = _database.GetCollection<SessionPlayerDeck>(COLLECTION_NAME);
        }

        public GameDeckDAO(LiteDatabase database)
        {
            _database = database;
            _collection = _database.GetCollection<SessionPlayerDeck>(COLLECTION_NAME);
        }

        public SessionPlayerDeck GetById(string sessionId)
        {
            try
            {
                return _collection.FindById(sessionId);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        // --- CREATE & UPDATE ---
        /// <summary>
        /// Thêm mới nếu chưa có, cập nhật ghi đè nếu đã tồn tại.
        /// </summary>
        public bool Upsert(SessionPlayerDeck deck)
        {
            try
            {
                return _collection.Upsert(deck);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- DELETE ---
        public bool Delete(string sessionId)
        {
            try
            {
                return _collection.Delete(sessionId);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}