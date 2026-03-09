namespace SEP490G69.Battle.Cards
{
    using LiteDB;
    using UnityEngine;

    public class GameDeckDAO
    {
        /// <summary>
        /// Format includes: <SESSION_ID>:<RAW_CARD_ID>:<CARD_VARIANT>
        /// Card variant is used to handle the case which there're 2 same cards in a deck.
        /// </summary>
        public const string FORMAT_IN_DECK_CARD_ID = "{0}:{1}:{2}";

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

        public bool Update(SessionPlayerDeck deck)
        {
            try
            {
                return _collection.Update(deck);
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
                if (GetById(sessionId) != null)
                {
                    return _collection.Delete(sessionId);
                }
                else
                {
                    Debug.LogError($"[GameDeckDAO]: Deck of session {sessionId} does not existed");
                    return false;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}