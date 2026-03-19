namespace SEP490G69.Tournament
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class TournamentProgressDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "tournament_progress";

        // =========================
        // AUTO MODE
        // =========================

        public TournamentProgressData GetById(string id)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetById(db, id));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public TournamentProgressData GetById(string sessionId, string rawTournamentId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetById(db, sessionId, rawTournamentId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<TournamentProgressData> GetAllBySessionId(string sessionId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetAllBySessionId(db, sessionId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<TournamentProgressData>();
            }
        }

        public bool Insert(TournamentProgressData data)
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

        public bool InsertMany(List<TournamentProgressData> tournaments)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => InsertMany(db, tournaments));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(TournamentProgressData data)
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

        public bool Upsert(TournamentProgressData data)
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

        public bool Delete(string id)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Delete(db, id));
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

        public TournamentProgressData GetById(LiteDatabase db, string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return null;

                var col = GetCollection<TournamentProgressData>(db, COLLECTION_NAME);
                return col.FindById(id);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public TournamentProgressData GetById(LiteDatabase db, string sessionId, string rawTournamentId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(rawTournamentId))
                    return null;

                string entityId = EntityIdConstructor.ConstructDBEntityId(sessionId, rawTournamentId);

                var col = GetCollection<TournamentProgressData>(db, COLLECTION_NAME);
                return col.FindById(entityId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<TournamentProgressData> GetAllBySessionId(LiteDatabase db, string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return new List<TournamentProgressData>();

                var col = GetCollection<TournamentProgressData>(db, COLLECTION_NAME);
                return col.Find(x => x.SessionId == sessionId).ToList(); // FIX Equals
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<TournamentProgressData>();
            }
        }

        // --- CREATE ---

        public bool Insert(LiteDatabase db, TournamentProgressData data)
        {
            try
            {
                if (data == null)
                    return false;

                var col = GetCollection<TournamentProgressData>(db, COLLECTION_NAME);
                col.Insert(data);
                return true;
            }
            catch (LiteException e) when (e.ErrorCode == LiteException.INDEX_DUPLICATE_KEY)
            {
                Debug.LogWarning($"[TournamentProgressDAO] Duplicate: {data?.Id}");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool InsertMany(LiteDatabase db, List<TournamentProgressData> tournaments)
        {
            try
            {
                if (tournaments == null || tournaments.Count == 0)
                    return false;

                var col = GetCollection<TournamentProgressData>(db, COLLECTION_NAME);
                col.InsertBulk(tournaments);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(LiteDatabase db, TournamentProgressData data)
        {
            try
            {
                if (data == null)
                    return false;

                var col = GetCollection<TournamentProgressData>(db, COLLECTION_NAME);
                return col.Upsert(data);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- UPDATE ---

        public bool Update(LiteDatabase db, TournamentProgressData data)
        {
            try
            {
                if (data == null)
                    return false;

                var col = GetCollection<TournamentProgressData>(db, COLLECTION_NAME);
                return col.Update(data);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- DELETE ---

        public bool Delete(LiteDatabase db, string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return false;

                var col = GetCollection<TournamentProgressData>(db, COLLECTION_NAME);
                return col.Delete(id);
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

                var col = GetCollection<TournamentProgressData>(db, COLLECTION_NAME);
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