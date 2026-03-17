namespace SEP490G69.GameSessions
{
    using LiteDB;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GameSessionDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "PlayerTrainingSessions";

        public bool Insert(PlayerTrainingSession session)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<PlayerTrainingSession> collection = GetCollection<PlayerTrainingSession>(db, COLLECTION_NAME);
                    collection.Insert(session);
                    return true;
                }

            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        public bool Update(PlayerTrainingSession session)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<PlayerTrainingSession> collection = GetCollection<PlayerTrainingSession>(db, COLLECTION_NAME);
                    collection.Update(session);
                    return true;
                }

            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
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
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<PlayerTrainingSession> collection = GetCollection<PlayerTrainingSession>(db, COLLECTION_NAME);
                    collection.Delete(sessionId);
                    return true;
                }

            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        public PlayerTrainingSession GetById(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return null;
            }

            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<PlayerTrainingSession> collection = GetCollection<PlayerTrainingSession>(db, COLLECTION_NAME);
                    return collection.FindById(sessionId);
                }

            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }

        public List<PlayerTrainingSession> GetAllByPlayerId(string playerId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<PlayerTrainingSession> collection = GetCollection<PlayerTrainingSession>(db, COLLECTION_NAME);
                    return collection.Query()
                                     .Where(x => x.PlayerId == playerId)
                                     .ToList();
                }

            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<PlayerTrainingSession> GetAll()
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<PlayerTrainingSession> collection = GetCollection<PlayerTrainingSession>(db, COLLECTION_NAME);
                    List<PlayerTrainingSession> list = collection.Query().ToList();
                    return list;
                }

            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }
    }
}