namespace SEP490G69.Battle.Cards
{
    using LiteDB;
    using UnityEngine;

    public class GameCardsDAO 
    {
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
    }
}