namespace SEP490G69.GameSessions
{
    using LiteDB;
    using System.Collections.Generic;
    using UnityEngine;

    public class GameSessionDAO 
    {
        private LiteDatabase _database;

        public GameSessionDAO(LiteDatabase database)
        {
            _database = database;
        }

        public bool InsertSession(PlayerTrainingSession session)
        {
            try
            {
                ILiteCollection<PlayerTrainingSession> cl = _database.GetCollection<PlayerTrainingSession>("PlayerTrainingSessions");
                cl.Insert(session);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public List<PlayerTrainingSession> GetAllSessions()
        {
            try
            {
                ILiteCollection<PlayerTrainingSession> cl = _database.GetCollection<PlayerTrainingSession>("PlayerTrainingSessions");
                List<PlayerTrainingSession> list = cl.Query().ToList();
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