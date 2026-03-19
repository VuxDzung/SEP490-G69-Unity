namespace SEP490G69.Legacy
{
    using LiteDB;
    using System;
    using UnityEngine;

    public class GameLegacyDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "LegacyStats";

        // =========================
        // AUTO MODE
        // =========================

        public LegacyStatData GetById(string entityId)
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

        public LegacyStatData GetById(string playerId, string rawLegacyId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetById(db, playerId, rawLegacyId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool Insert(LegacyStatData data)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Insert(db, data));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(LegacyStatData data)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Update(db, data));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(LegacyStatData data)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Upsert(db, data));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteById(string entityId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => DeleteById(db, entityId));
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

        public LegacyStatData GetById(LiteDatabase db, string entityId)
        {
            try
            {
                if (string.IsNullOrEmpty(entityId))
                    return null;

                var col = GetCollection<LegacyStatData>(db, COLLECTION_NAME);
                return col.FindById(entityId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public LegacyStatData GetById(LiteDatabase db, string playerId, string rawLegacyId)
        {
            try
            {
                if (string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(rawLegacyId))
                    return null;

                string entityId = EntityIdConstructor.ConstructDBEntityId(playerId, rawLegacyId);

                var col = GetCollection<LegacyStatData>(db, COLLECTION_NAME);
                return col.FindById(entityId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        // --- CREATE ---

        public bool Insert(LiteDatabase db, LegacyStatData data)
        {
            try
            {
                if (data == null)
                    return false;

                var col = GetCollection<LegacyStatData>(db, COLLECTION_NAME);
                col.Insert(data);
                return true;
            }
            catch (LiteException e) when (e.ErrorCode == LiteException.INDEX_DUPLICATE_KEY)
            {
                Debug.LogWarning($"[GameLegacyDAO] Duplicate key: {data?.Id}");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(LiteDatabase db, LegacyStatData data)
        {
            try
            {
                if (data == null)
                    return false;

                var col = GetCollection<LegacyStatData>(db, COLLECTION_NAME);
                return col.Upsert(data);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- UPDATE ---

        public bool Update(LiteDatabase db, LegacyStatData data)
        {
            try
            {
                if (data == null)
                    return false;

                var col = GetCollection<LegacyStatData>(db, COLLECTION_NAME);
                return col.Update(data);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- DELETE ---

        public bool DeleteById(LiteDatabase db, string entityId)
        {
            try
            {
                if (string.IsNullOrEmpty(entityId))
                    return false;

                var col = GetCollection<LegacyStatData>(db, COLLECTION_NAME);
                return col.Delete(entityId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}