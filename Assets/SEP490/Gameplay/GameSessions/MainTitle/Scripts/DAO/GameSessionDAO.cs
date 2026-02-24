namespace SEP490G69.GameSessions
{
    using LiteDB;
    using System.Collections.Generic;
    using UnityEngine;

    public class GameSessionDAO 
    {
        public const string COLLECTION_NAME = "PlayerTrainingSessions";

        private LiteDatabase _database;
        private ILiteCollection<PlayerTrainingSession> _collection;

        public GameSessionDAO(LiteDatabase database)
        {
            _database = database;
            _collection = _database.GetCollection<PlayerTrainingSession>(COLLECTION_NAME);
        }

        public bool InsertSession(PlayerTrainingSession session)
        {
            try
            {
                _collection.Insert(session);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public bool UpdateSession(PlayerTrainingSession session)
        {
            try
            {
                _collection.Update(session);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public bool DeleteSession(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
                return false;

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

        public PlayerTrainingSession GetSession(string sessionId)
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

        public List<PlayerTrainingSession> GetAllSessions()
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