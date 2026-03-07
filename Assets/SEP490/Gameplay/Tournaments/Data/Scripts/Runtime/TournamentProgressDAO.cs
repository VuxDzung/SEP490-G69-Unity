namespace SEP490G69.Tournament
{
    using LiteDB;
    using UnityEngine;

    public class TournamentProgressDAO 
    {
        public const string COLLECTION_NAME = "tournament_progress";

        private LiteDatabase _database;
        private ILiteCollection<TournamentProgressData> _collection;

        public TournamentProgressDAO()
        {
            _database = LocalDBInitiator.GetDatabase();
            _collection = _database.GetCollection<TournamentProgressData>(COLLECTION_NAME);
        }

        public TournamentProgressData Get(string id)
        {
            return _collection.FindById(id);
        }

        public void Upsert(TournamentProgressData data)
        {
            _collection.Upsert(data);
        }

        public void Delete(string id)
        {
            _collection.Delete(id);
        }
    }
}