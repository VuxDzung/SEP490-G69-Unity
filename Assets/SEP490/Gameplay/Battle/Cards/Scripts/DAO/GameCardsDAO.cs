namespace SEP490G69.Battle.Cards
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GameCardsDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "PlayerCards";

        // =========================
        // AUTO MODE (SAFE WRAPPER)
        // =========================

        public SessionCardData GetById(string sessionId, string rawCardId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetById(db, sessionId, rawCardId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public SessionCardData GetById(string sessionCardId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetById(db, sessionCardId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<SessionCardData> GetAllBySessionId(string sessionId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetAllBySessionId(db, sessionId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<SessionCardData>();
            }
        }

        public bool Insert(SessionCardData card)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Insert(db, card));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool InsertMany(List<SessionCardData> cards)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => InsertMany(db, cards));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(SessionCardData card)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Upsert(db, card));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(SessionCardData card)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Update(db, card));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Delete(string sessionCardId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Delete(db, sessionCardId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteAllBySessionId(string sessionId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => DeleteAllBySessionId(db, sessionId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // =========================
        // TRANSACTION MODE (CORE)
        // =========================

        public SessionCardData GetById(LiteDatabase db, string sessionId, string rawCardId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(rawCardId))
                    return null;

                string entityId = EntityIdConstructor.ConstructDBEntityId(sessionId, rawCardId);
                var col = GetCollection<SessionCardData>(db, COLLECTION_NAME);
                return col.FindById(entityId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public SessionCardData GetById(LiteDatabase db, string sessionCardId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionCardId))
                    return null;

                var col = GetCollection<SessionCardData>(db, COLLECTION_NAME);
                return col.FindById(sessionCardId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<SessionCardData> GetAllBySessionId(LiteDatabase db, string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return new List<SessionCardData>();

                var col = GetCollection<SessionCardData>(db, COLLECTION_NAME);
                return col.Find(card => card.SessionId == sessionId).ToList();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<SessionCardData>();
            }
        }

        // --- CREATE ---

        public bool Insert(LiteDatabase db, SessionCardData card)
        {
            try
            {
                if (card == null)
                    return false;

                var col = GetCollection<SessionCardData>(db, COLLECTION_NAME);
                col.Insert(card);
                return true;
            }
            catch (LiteException e) when (e.ErrorCode == LiteException.INDEX_DUPLICATE_KEY)
            {
                Debug.LogWarning($"[GameCardsDAO] Duplicate key: {card?.SessionCardId}");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool InsertMany(LiteDatabase db, List<SessionCardData> cards)
        {
            try
            {
                if (cards == null || cards.Count == 0)
                    return true;

                var col = GetCollection<SessionCardData>(db, COLLECTION_NAME);
                int inserted = col.InsertBulk(cards);
                return inserted == cards.Count;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(LiteDatabase db, SessionCardData card)
        {
            try
            {
                if (card == null)
                    return false;

                var col = GetCollection<SessionCardData>(db, COLLECTION_NAME);
                return col.Upsert(card);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- UPDATE ---

        public bool Update(LiteDatabase db, SessionCardData card)
        {
            try
            {
                if (card == null)
                    return false;

                var col = GetCollection<SessionCardData>(db, COLLECTION_NAME);
                return col.Update(card);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- DELETE ---

        public bool Delete(LiteDatabase db, string sessionCardId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionCardId))
                    return false;

                var col = GetCollection<SessionCardData>(db, COLLECTION_NAME);
                return col.Delete(sessionCardId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteAllBySessionId(LiteDatabase db, string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return false;

                var col = GetCollection<SessionCardData>(db, COLLECTION_NAME);
                col.DeleteMany(x => x.SessionId == sessionId);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}