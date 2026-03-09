namespace SEP490G69.Battle.Cards
{
    using LiteDB;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GameCardsDAO
    {
        /// <summary>
        /// Format id <SESSION_ID>:<RAW_CARD_ID>
        /// </summary>
        public const string FORMAT_OBTAINED_CARD_ID = "{0}:{1}";

        public const string COLLECTION_NAME = "PlayerCards";
        private readonly LiteDatabase _database;
        private readonly ILiteCollection<SessionCardData> _collection;

        public GameCardsDAO()
        {
            _database = LocalDBInitiator.GetDatabase();
            _collection = _database.GetCollection<SessionCardData>(COLLECTION_NAME);
            _collection.EnsureIndex(x => x.SessionId);
            _collection.EnsureIndex(x => x.RawCardId);
        }

        public GameCardsDAO(LiteDatabase database)
        {
            _database = database;
            _collection = _database.GetCollection<SessionCardData>(COLLECTION_NAME);
        }

        public SessionCardData GetById(string sessionId, string rawCardId)
        {
            return _collection.FindOne(card => card.SessionId.Equals(sessionId) &&
                                               card.RawCardId.Equals(rawCardId));
        }
        public SessionCardData GetById(string sessionCardId)
        {
            return _collection.FindById(sessionCardId);
        }

        /// <summary>
        /// Lấy toàn bộ thẻ bài mà người chơi (Session) đang sở hữu.
        /// </summary>
        public List<SessionCardData> GetAllBySessionId(string sessionId)
        {
            try
            {
                return _collection.Find(card => card.SessionId.Equals(sessionId)).ToList();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new List<SessionCardData>();
            }
        }

        // --- CREATE ---
        public bool Insert(SessionCardData card)
        {
            try
            {
                _collection.Insert(card);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- UPDATE ---
        public bool Update(SessionCardData card)
        {
            try
            {
                return _collection.Update(card);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- DELETE ---
        public bool Delete(string sessionCardId)
        {
            try
            {
                return _collection.Delete(sessionCardId);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteAllBySessionId(string sessionId)
        {
            try
            {
                _collection.DeleteMany(x => x.SessionId.Equals(sessionId));
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}