using LiteDB;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SEP490G69.GameSessions
{
    public class SnapshotCheckpointDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "SnapshotCheckpoints";

        public SnapshotCheckpointDAO() { }

        public List<SessionSnapshotData> GetAllBySessionId(string sessionId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetAllBySessionId(db, sessionId));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public SessionSnapshotData GetById(string snapshotId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetById(db, snapshotId));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public SessionSnapshotData GetById(string sessionId, string rawId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetById(db, sessionId, rawId));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool Insert(SessionSnapshotData snapshotData)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Insert(db, snapshotData));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(SessionSnapshotData snapshotData)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Update(db, snapshotData));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(SessionSnapshotData snapshotData)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Upsert(db, snapshotData));
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Delete(string snapshotId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Delete(db, snapshotId));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteMany(string sessionId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => DeleteManyById(db, sessionId));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public SessionSnapshotData GetById(LiteDatabase db, string snapshotId)
        {
            try
            {
                var col = GetCollection<SessionSnapshotData>(db, COLLECTION_NAME);
                return col.FindById(snapshotId);
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public SessionSnapshotData GetById(LiteDatabase db, string sessionId, string rawId)
        {
            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(rawId))
                return null;

            string sessionSnapshotId = EntityIdConstructor.ConstructDBEntityId(sessionId, rawId);

            return GetById(db, sessionSnapshotId);
        }

        public List<SessionSnapshotData> GetAllBySessionId(LiteDatabase db, string sessionId)
        {
            try
            {
                var col = GetCollection<SessionSnapshotData>(db, COLLECTION_NAME);
                return col.Find(snapshot => snapshot.SessionId == sessionId).ToList();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool Insert(LiteDatabase db, SessionSnapshotData sessionSnapshotData)
        {
            try
            {
                var col = GetCollection<SessionSnapshotData>(db, COLLECTION_NAME);
                col.Insert(sessionSnapshotData);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(LiteDatabase db, SessionSnapshotData sessionSnapshotData)
        {
            try
            {
                var col = GetCollection<SessionSnapshotData>(db, COLLECTION_NAME);
                col.Update(sessionSnapshotData);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(LiteDatabase db, SessionSnapshotData sessionSnapshotData)
        {
            try
            {
                var col = GetCollection<SessionSnapshotData>(db, COLLECTION_NAME);
                col.Upsert(sessionSnapshotData);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Delete(LiteDatabase db, string entityId)
        {
            if (string.IsNullOrEmpty(entityId))
            {
                return false;
            }

            try
            {
                var col = GetCollection<SessionSnapshotData>(db, COLLECTION_NAME);
                col.Delete(entityId);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteManyById(LiteDatabase db, string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return false;
            }

            try
            {
                var col = GetCollection<SessionSnapshotData>(db, COLLECTION_NAME);
                int deleted = col.DeleteMany(snapshot => snapshot.SessionId == sessionId);
                return deleted >= 0;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}
