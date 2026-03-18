namespace SEP490G69.Training
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class TrainingExerciseDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "SessionExerciseData";

        // =========================
        // AUTO MODE (SAFE WRAPPER)
        // =========================

        #region Auto mode
        public bool Insert(SessionTrainingExercise entity)
        {
            try
            {
                return LocalDBInitiator.Execute(db => Insert(db, entity));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool InsertMany(List<SessionTrainingExercise> entities)
        {
            try
            {
                return LocalDBInitiator.Execute(db => InsertMany(db, entities));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(SessionTrainingExercise entity)
        {
            try
            {
                return LocalDBInitiator.Execute(db => Update(db, entity));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(SessionTrainingExercise entity)
        {
            try
            {
                return LocalDBInitiator.Execute(db => Upsert(db, entity));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Delete(string entityId)
        {
            try
            {
                return LocalDBInitiator.Execute(db => Delete(db, entityId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteAllBySessionId(string sessionId)
        {
            try
            {
                return LocalDBInitiator.Execute(db => DeleteAllBySessionId(db, sessionId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public SessionTrainingExercise GetById(string sessionId, string exerciseId)
        {
            try
            {
                return LocalDBInitiator.Execute(db => GetById(db, sessionId, exerciseId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<SessionTrainingExercise> GetAllBySessionId(string sessionId)
        {
            try
            {
                return LocalDBInitiator.Execute(db => GetAllBySessionId(db, sessionId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<SessionTrainingExercise>();
            }
        }
        #endregion

        // =========================
        // TRANSACTION MODE (CORE)
        // =========================

        #region Transaction mode
        public bool Insert(LiteDatabase db, SessionTrainingExercise entity)
        {
            try
            {
                if (entity == null)
                    return false;

                var col = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                return col.Insert(entity);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool InsertMany(LiteDatabase db, List<SessionTrainingExercise> entities)
        {
            try
            {
                if (entities == null || entities.Count == 0)
                    return true;

                var col = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                int inserted = col.InsertBulk(entities);
                return inserted == entities.Count;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(LiteDatabase db, SessionTrainingExercise entity)
        {
            try
            {
                if (entity == null)
                    return false;

                var col = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                return col.Update(entity);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(LiteDatabase db, SessionTrainingExercise entity)
        {
            try
            {
                if (entity == null)
                    return false;

                var col = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                return col.Upsert(entity);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Delete(LiteDatabase db, string entityId)
        {
            try
            {
                if (string.IsNullOrEmpty(entityId))
                    return false;

                var col = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                return col.Delete(entityId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteAllBySessionId(LiteDatabase db, string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return false;

                var col = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                col.DeleteMany(x => x.SessionId == sessionId);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public SessionTrainingExercise GetById(LiteDatabase db, string sessionId, string exerciseId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(exerciseId))
                    return null;

                string entityId = EntityIdConstructor.ConstructDBEntityId(sessionId, exerciseId);
                var col = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                return col.FindById(entityId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<SessionTrainingExercise> GetAllBySessionId(LiteDatabase db, string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return new List<SessionTrainingExercise>();

                var col = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                return col.Find(x => x.SessionId == sessionId).ToList();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<SessionTrainingExercise>();
            }
        }
        #endregion
    }
}