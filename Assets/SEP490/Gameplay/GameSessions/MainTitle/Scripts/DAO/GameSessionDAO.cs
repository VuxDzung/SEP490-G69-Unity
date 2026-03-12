namespace SEP490G69.GameSessions
{
    using LiteDB;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GameSessionDAO 
    {
        public const string COLLECTION_NAME = "PlayerTrainingSessions";

        private LiteDatabase _database;
        private ILiteCollection<PlayerTrainingSession> _collection;

        public GameSessionDAO()
        {
            _database = LocalDBInitiator.GetDatabase();
            _collection = _database.GetCollection<PlayerTrainingSession>(COLLECTION_NAME);
        }

        public GameSessionDAO(LiteDatabase database)
        {
            _database = database;
            _collection = _database.GetCollection<PlayerTrainingSession>(COLLECTION_NAME);
        }

        public bool Insert(PlayerTrainingSession session)
        {
            try
            {
                _collection.Insert(session);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        public bool Update(PlayerTrainingSession session)
        {
            try
            {
                _collection.Update(session);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        public bool DeleteById(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                Debug.LogError("Session id is null");
                return false;
            }

            try
            {
                return _collection.Delete(sessionId);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        public PlayerTrainingSession GetById(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
                return null;
            try
            {
                return _collection.FindById(sessionId);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }

        public List<PlayerTrainingSession> GetAllByPlayerId(string playerId)
        {
            try
            {
                return _collection.Query()
                    .Where(x => x.PlayerId == playerId)
                    .ToList();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }
        public List<PlayerTrainingSession> GetAll()
        {
            try
            {
                List<PlayerTrainingSession> list = _collection.Query().ToList();
                return list;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }
    }
}