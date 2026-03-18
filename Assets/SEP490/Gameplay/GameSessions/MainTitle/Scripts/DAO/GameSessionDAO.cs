namespace SEP490G69.GameSessions
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GameSessionDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "PlayerTrainingSessions";

        // =========================
        // AUTO MODE
        // =========================

        public bool Insert(PlayerTrainingSession session)
        {
            try
            {
                return LocalDBInitiator.Execute(db => Insert(db, session));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(PlayerTrainingSession session)
        {
            try
            {
                return LocalDBInitiator.Execute(db => Update(db, session));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(PlayerTrainingSession session)
        {
            try
            {
                return LocalDBInitiator.Execute(db => Upsert(db, session));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteById(string sessionId)
        {
            try
            {
                return LocalDBInitiator.Execute(db => DeleteById(db, sessionId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public PlayerTrainingSession GetById(string sessionId)
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

        public List<PlayerTrainingSession> GetAllByPlayerId(string playerId)
        {
            try
            {
                return LocalDBInitiator.Execute(db => GetAllByPlayerId(db, playerId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<PlayerTrainingSession>();
            }
        }

        public List<PlayerTrainingSession> GetAll()
        {
            try
            {
                return LocalDBInitiator.Execute(db => GetAll(db));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<PlayerTrainingSession>();
            }
        }

        // =========================
        // TRANSACTION MODE (CORE)
        // =========================

        public bool Insert(LiteDatabase db, PlayerTrainingSession session)
        {
            try
            {
                if (session == null)
                    return false;

                var col = GetCollection<PlayerTrainingSession>(db, COLLECTION_NAME);
                col.Insert(session);
                return true;
            }
            catch (LiteException e) when (e.ErrorCode == LiteException.INDEX_DUPLICATE_KEY)
            {
                Debug.LogWarning($"[GameSessionDAO] Duplicate session: {session?.SessionId}");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(LiteDatabase db, PlayerTrainingSession session)
        {
            try
            {
                if (session == null)
                    return false;

                var col = GetCollection<PlayerTrainingSession>(db, COLLECTION_NAME);
                return col.Update(session);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(LiteDatabase db, PlayerTrainingSession session)
        {
            try
            {
                if (session == null)
                    return false;

                var col = GetCollection<PlayerTrainingSession>(db, COLLECTION_NAME);
                return col.Upsert(session);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteById(LiteDatabase db, string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return false;

                var col = GetCollection<PlayerTrainingSession>(db, COLLECTION_NAME);
                return col.Delete(sessionId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public PlayerTrainingSession GetById(LiteDatabase db, string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return null;

                var col = GetCollection<PlayerTrainingSession>(db, COLLECTION_NAME);
                return col.FindById(sessionId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<PlayerTrainingSession> GetAllByPlayerId(LiteDatabase db, string playerId)
        {
            try
            {
                if (string.IsNullOrEmpty(playerId))
                    return new List<PlayerTrainingSession>();

                var col = GetCollection<PlayerTrainingSession>(db, COLLECTION_NAME);
                return col.Find(x => x.PlayerId == playerId).ToList();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<PlayerTrainingSession>();
            }
        }

        public List<PlayerTrainingSession> GetAll(LiteDatabase db)
        {
            try
            {
                var col = GetCollection<PlayerTrainingSession>(db, COLLECTION_NAME);
                return col.FindAll().ToList();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<PlayerTrainingSession>();
            }
        }
    }
}