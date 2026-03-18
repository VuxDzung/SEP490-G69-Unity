namespace SEP490G69.Battle.Cards
{
    using LiteDB;
    using System;
    using UnityEngine;

    public class GameDeckDAO : BaseDAO
    {
        /// <summary>
        /// Format includes: <SESSION_ID>:<RAW_CARD_ID>:<CARD_VARIANT>
        /// </summary>
        public const string FORMAT_IN_DECK_CARD_ID = "{0}:{1}:{2}";

        public const string COLLECTION_NAME = "PlayerDeck";

        // =========================
        // AUTO MODE
        // =========================

        public SessionPlayerDeck GetById(string sessionId)
        {
            try
            {
                return LocalDBInitiator.Execute(db => GetById(db, sessionId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool Insert(SessionPlayerDeck deck)
        {
            try
            {
                return LocalDBInitiator.Execute(db => Insert(db, deck));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(SessionPlayerDeck deck)
        {
            try
            {
                return LocalDBInitiator.Execute(db => Upsert(db, deck));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(SessionPlayerDeck deck)
        {
            try
            {
                return LocalDBInitiator.Execute(db => Update(db, deck));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Delete(string sessionId)
        {
            try
            {
                return LocalDBInitiator.Execute(db => Delete(db, sessionId));
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

        public SessionPlayerDeck GetById(LiteDatabase db, string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return null;

                var col = GetCollection<SessionPlayerDeck>(db, COLLECTION_NAME);
                return col.FindById(sessionId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool Insert(LiteDatabase db, SessionPlayerDeck deck)
        {
            try
            {
                if (deck == null)
                    return false;

                var col = GetCollection<SessionPlayerDeck>(db, COLLECTION_NAME);
                col.Insert(deck);
                return true;
            }
            catch (LiteException e) when (e.ErrorCode == LiteException.INDEX_DUPLICATE_KEY)
            {
                Debug.LogWarning($"[GameDeckDAO] Duplicate deck for session: {deck?.SessionId}");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(LiteDatabase db, SessionPlayerDeck deck)
        {
            try
            {
                if (deck == null)
                    return false;

                var col = GetCollection<SessionPlayerDeck>(db, COLLECTION_NAME);
                return col.Upsert(deck);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(LiteDatabase db, SessionPlayerDeck deck)
        {
            try
            {
                if (deck == null)
                    return false;

                var col = GetCollection<SessionPlayerDeck>(db, COLLECTION_NAME);
                return col.Update(deck);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Delete(LiteDatabase db, string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return false;

                var col = GetCollection<SessionPlayerDeck>(db, COLLECTION_NAME);
                return col.Delete(sessionId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}