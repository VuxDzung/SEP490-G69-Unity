namespace SEP490G69
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class TrainingSupportItemsDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "TrainingSupportItems";

        // =========================
        // AUTO MODE
        // =========================

        public TrainingSupportItem GetById(string entityId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetById(db, entityId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public TrainingSupportItem GetById(string sessionId, string rawItemId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetById(db, sessionId, rawItemId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<TrainingSupportItem> GetAllBySessionId(string sessionId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetAllBySessionId(db, sessionId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<TrainingSupportItem>(); // FIX null
            }
        }

        public bool Insert(TrainingSupportItem item)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Insert(db, item));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(TrainingSupportItem item)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Update(db, item));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(TrainingSupportItem item)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Upsert(db, item));
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
                return LocalDBOrchestrator.Execute(db => Delete(db, entityId));
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
                return LocalDBOrchestrator.Execute(db => DeleteAllBySessionId(db, sessionId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // =========================
        // TRANSACTION MODE (CORE)
        // =========================

        public TrainingSupportItem GetById(LiteDatabase db, string entityId)
        {
            try
            {
                if (string.IsNullOrEmpty(entityId))
                    return null;

                var col = GetCollection<TrainingSupportItem>(db, COLLECTION_NAME);
                return col.FindById(entityId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public TrainingSupportItem GetById(LiteDatabase db, string sessionId, string rawItemId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(rawItemId))
                    return null;

                string entityId = EntityIdConstructor.ConstructDBEntityId(sessionId, rawItemId);

                var col = GetCollection<TrainingSupportItem>(db, COLLECTION_NAME);
                return col.FindById(entityId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<TrainingSupportItem> GetAllBySessionId(LiteDatabase db, string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return new List<TrainingSupportItem>();

                var col = GetCollection<TrainingSupportItem>(db, COLLECTION_NAME);
                return col.Find(x => x.SessionId == sessionId).ToList();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<TrainingSupportItem>();
            }
        }

        // --- CREATE ---

        public bool Insert(LiteDatabase db, TrainingSupportItem item)
        {
            try
            {
                if (item == null)
                {
                    return false;
                }

                var col = GetCollection<TrainingSupportItem>(db, COLLECTION_NAME);
                col.Insert(item);
                return true;
            }
            catch (LiteException e) when (e.ErrorCode == LiteException.INDEX_DUPLICATE_KEY)
            {
                Debug.LogWarning($"[TrainingSupportItemsDAO] Duplicate: {item?.EntityId}");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(LiteDatabase db, TrainingSupportItem item)
        {
            try
            {
                if (item == null)
                    return false;

                var col = GetCollection<TrainingSupportItem>(db, COLLECTION_NAME);
                return col.Upsert(item);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- UPDATE ---

        public bool Update(LiteDatabase db, TrainingSupportItem item)
        {
            try
            {
                if (item == null)
                    return false;

                var col = GetCollection<TrainingSupportItem>(db, COLLECTION_NAME);
                return col.Update(item);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- DELETE ---

        public bool Delete(LiteDatabase db, string entityId)
        {
            try
            {
                if (string.IsNullOrEmpty(entityId))
                    return false;

                var col = GetCollection<TrainingSupportItem>(db, COLLECTION_NAME);
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

                var col = GetCollection<TrainingSupportItem>(db, COLLECTION_NAME);
                col.DeleteMany(x => x.SessionId == sessionId); // FIX Equals
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}