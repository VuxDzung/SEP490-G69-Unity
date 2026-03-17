namespace SEP490G69.Battle.Cards
{
    using LiteDB;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GameCardsDAO : BaseDAO
    {
        /// <summary>
        /// Format id <SESSION_ID>:<RAW_CARD_ID>
        /// </summary>
        public const string FORMAT_OBTAINED_CARD_ID = "{0}:{1}";

        public const string COLLECTION_NAME = "PlayerCards";

        public GameCardsDAO() { }

        public SessionCardData GetById(string sessionId, string rawCardId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionCardData> collection = GetCollection<SessionCardData>(db, COLLECTION_NAME);
                    return collection.FindOne(card => card.SessionId.Equals(sessionId) &&
                                               card.RawCardId.Equals(rawCardId));
                }

            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }

        public SessionCardData GetById(string sessionCardId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionCardData> collection = GetCollection<SessionCardData>(db, COLLECTION_NAME);
                    return collection.FindById(sessionCardId);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Lấy toàn bộ thẻ bài mà người chơi (Session) đang sở hữu.
        /// </summary>
        public List<SessionCardData> GetAllBySessionId(string sessionId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionCardData> collection = GetCollection<SessionCardData>(db, COLLECTION_NAME);
                    return collection.Find(card => card.SessionId.Equals(sessionId)).ToList();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return new List<SessionCardData>();
            }
        }

        // --- CREATE ---
        public bool Insert(SessionCardData card)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionCardData> collection = GetCollection<SessionCardData>(db, COLLECTION_NAME);
                    collection.Insert(card);
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        // --- UPDATE ---
        public bool Update(SessionCardData card)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionCardData> collection = GetCollection<SessionCardData>(db, COLLECTION_NAME);
                    collection.Update(card);
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        // --- DELETE ---
        public bool Delete(string sessionCardId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionCardData> collection = GetCollection<SessionCardData>(db, COLLECTION_NAME);
                    return collection.Delete(sessionCardId);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        public bool DeleteAllBySessionId(string sessionId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionCardData> collection = GetCollection<SessionCardData>(db, COLLECTION_NAME);
                    collection.DeleteMany(x => x.SessionId.Equals(sessionId));
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }
    }
}