namespace SEP490G69.Battle.Cards
{
    using LiteDB;
    using System;
    using UnityEngine;

    public class GameDeckDAO : BaseDAO
    {
        /// <summary>
        /// Format includes: <SESSION_ID>:<RAW_CARD_ID>:<CARD_VARIANT>
        /// Card variant is used to handle the case which there're 2 same cards in a deck.
        /// </summary>
        public const string FORMAT_IN_DECK_CARD_ID = "{0}:{1}:{2}";

        public const string COLLECTION_NAME = "PlayerDeck";

        public SessionPlayerDeck GetById(string sessionId)
        {
            try
            {
                return LocalDBInitiator.Execute(db =>
                {
                    return db.GetCollection<SessionPlayerDeck>(COLLECTION_NAME).FindById(sessionId);
                });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool Upsert(SessionPlayerDeck deck)
        {
            try
            {
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<SessionPlayerDeck>(db, COLLECTION_NAME);
                    return col.Upsert(deck);
                }
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
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<SessionPlayerDeck>(db, COLLECTION_NAME);
                    return col.Update(deck);
                }
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
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<SessionPlayerDeck>(db, COLLECTION_NAME);

                    if (col.FindById(sessionId) != null)
                    {
                        return col.Delete(sessionId);
                    }

                    Debug.LogError($"[GameDeckDAO]: Deck of session {sessionId} does not exist");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}