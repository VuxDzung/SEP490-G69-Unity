namespace SEP490G69.Tournament
{
    using LiteDB;
    using System.Collections.Generic;
    using System.Linq;
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
            _collection.EnsureIndex(x => x.SessionId);
            _collection.EnsureIndex(x => x.RawTournamentId);
        }

        public TournamentProgressData GetById(string id)
        {
            return _collection.FindById(id);
        }
        public TournamentProgressData GetById(string sessionId, string rawTournamentId)
        {
            try
            {
                return _collection.FindOne(x => x.SessionId.Equals(sessionId) && 
                                                x.RawTournamentId.Equals(rawTournamentId));
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                return null;
            }
        }

        public List<TournamentProgressData> GetAllBySessionId(string sessionId)
        {
            try
            {
                return _collection.Find(x => x.SessionId.Equals(sessionId)).ToList();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                return null;
            }
        }

        public void Upsert(TournamentProgressData data)
        {
            _collection.Upsert(data);
        }

        public void Delete(string id)
        {
            _collection.Delete(id);
        }

        public bool DeleteAllBySessionId(string sessionId)
        {
            try
            {
                List<TournamentProgressData> progresses = GetAllBySessionId(sessionId);
                if (progresses == null || progresses.Count == 0) return true;
                foreach (var progress in progresses)
                {
                    Delete(progress.Id);
                }
                return true;
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.ToString());
                return false;
            }
        }

        public bool DeleteAll()
        {
            try
            {
                List<TournamentProgressData> progresses = _collection.FindAll().ToList();
                if (progresses == null || progresses.Count == 0) return true;
                foreach (var progress in progresses)
                {
                    Delete(progress.Id);
                }
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                return false;
            }
        }
    }
}