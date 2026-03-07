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
            _database = LocalDBInitiator.GetDatabase();
            _collection = _database.GetCollection<SessionPlayerDeck>(COLLECTION_NAME);
        }

        public SessionPlayerDeck GetById(string sessionId)
        {
            try
            {
                return _collection.FindById(sessionId);
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }
    }
}