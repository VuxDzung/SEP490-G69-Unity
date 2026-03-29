namespace SEP490G69.Graduation
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GraduateRecordsDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "GraduateRecords";

        public GraduateRecordsDAO() { }

        // =========================
        // AUTO MODE
        // =========================

        public EndGameRecordData GetById(string recordId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetById(db, recordId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<EndGameRecordData> GetAllByPlayerId(string playerId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetAllByPlayerId(db, playerId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<EndGameRecordData>();
            }
        }

        public bool Insert(EndGameRecordData data)
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

        public bool InsertMany(List<EndGameRecordData> records)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => InsertMany(db, records));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(EndGameRecordData data)
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

        public bool Upsert(EndGameRecordData data)
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

        public bool DeleteById(string recordId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => DeleteById(db, recordId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteAllByPlayerId(string playerId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => DeleteAllByPlayerId(db, playerId));
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

        public EndGameRecordData GetById(LiteDatabase db, string recordId)
        {
            try
            {
                if (string.IsNullOrEmpty(recordId))
                    return null;

                var col = GetCollection<EndGameRecordData>(db, COLLECTION_NAME);
                return col.FindById(recordId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<EndGameRecordData> GetAllByPlayerId(LiteDatabase db, string playerId)
        {
            try
            {
                if (string.IsNullOrEmpty(playerId))
                    return new List<EndGameRecordData>();

                var col = GetCollection<EndGameRecordData>(db, COLLECTION_NAME);

                return col
                    .Find(x => x.PlayerId == playerId)
                    .OrderByDescending(x => x.RunChallegeCount) // optional sort
                    .ToList();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<EndGameRecordData>();
            }
        }

        // --- CREATE ---

        public bool Insert(LiteDatabase db, EndGameRecordData data)
        {
            try
            {
                if (data == null)
                    return false;

                var col = GetCollection<EndGameRecordData>(db, COLLECTION_NAME);
                col.Insert(data);
                return true;
            }
            catch (LiteException e) when (e.ErrorCode == LiteException.INDEX_DUPLICATE_KEY)
            {
                Debug.LogWarning($"[GraduateRecordsDAO] Duplicate key: {data?.RecordId}");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool InsertMany(LiteDatabase db, List<EndGameRecordData> records)
        {
            try
            {
                if (records == null || records.Count == 0)
                    return false;

                var col = GetCollection<EndGameRecordData>(db, COLLECTION_NAME);
                col.InsertBulk(records);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(LiteDatabase db, EndGameRecordData data)
        {
            try
            {
                if (data == null)
                    return false;

                var col = GetCollection<EndGameRecordData>(db, COLLECTION_NAME);
                return col.Upsert(data);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- UPDATE ---

        public bool Update(LiteDatabase db, EndGameRecordData data)
        {
            try
            {
                if (data == null)
                    return false;

                var col = GetCollection<EndGameRecordData>(db, COLLECTION_NAME);
                return col.Update(data);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- DELETE ---

        public bool DeleteById(LiteDatabase db, string recordId)
        {
            try
            {
                if (string.IsNullOrEmpty(recordId))
                    return false;

                var col = GetCollection<EndGameRecordData>(db, COLLECTION_NAME);
                return col.Delete(recordId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteAllByPlayerId(LiteDatabase db, string playerId)
        {
            try
            {
                if (string.IsNullOrEmpty(playerId))
                    return false;

                var col = GetCollection<EndGameRecordData>(db, COLLECTION_NAME);
                int deleted = col.DeleteMany(x => x.PlayerId == playerId);
                return deleted >= 0;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}