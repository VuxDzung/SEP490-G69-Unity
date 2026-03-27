namespace SEP490G69.Exploration
{
    using LiteDB;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GameExploreLocationDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "ExploreLocations";

        public bool Insert(ExploreLocationData entity)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Insert(db, entity));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
        public bool InsertMany(List<ExploreLocationData> entityList)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => InsertMany(db, entityList));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
        public bool Update(ExploreLocationData entity)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Update(db, entity));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
        public bool Upsert(ExploreLocationData entity)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Upsert(db, entity));
            }
            catch (System.Exception e)
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
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteManyById(string entityId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => DeleteManyById(db, entityId));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public ExploreLocationData GetById(string sessionId, string rawLocationId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetById(db, sessionId, rawLocationId));
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public ExploreLocationData GetById(string entityId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetById(db, entityId));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<ExploreLocationData> GetAllById(string sessionId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetAllById(db, sessionId));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new List<ExploreLocationData>();
            }
        }

        #region Core Transactions
        public bool Insert(LiteDatabase db, ExploreLocationData entity)
        {
            try
            {
                var col = GetCollection<ExploreLocationData>(db, COLLECTION_NAME);
                col.Insert(entity);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool InsertMany(LiteDatabase db, List<ExploreLocationData> entityList)
        {
            try
            {
                var col = GetCollection<ExploreLocationData>(db, COLLECTION_NAME);
                int inserted = col.InsertBulk(entityList);
                return inserted == entityList.Count;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(LiteDatabase db, ExploreLocationData entity)
        {
            try
            {
                var col = GetCollection<ExploreLocationData>(db, COLLECTION_NAME);
                col.Update(entity);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(LiteDatabase db, ExploreLocationData entity)
        {
            try
            {
                var col = GetCollection<ExploreLocationData>(db, COLLECTION_NAME);
                col.Upsert(entity);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public ExploreLocationData GetById(LiteDatabase db, string entityId)
        {
            try
            {
                var col = GetCollection<ExploreLocationData>(db, COLLECTION_NAME);

                return col.FindById(entityId);
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool Delete(LiteDatabase db, string entityId)
        {
            try
            {
                var col = GetCollection<ExploreLocationData>(db, COLLECTION_NAME);
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
            try
            {
                var col = GetCollection<ExploreLocationData>(db, COLLECTION_NAME);
                col.DeleteMany(ex => ex.EntityId.StartsWith(sessionId));
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public ExploreLocationData GetById(LiteDatabase db, string sessionId, string rawLocationId)
        {
            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(rawLocationId))
            {
                return null;
            }

            string entityId = EntityIdConstructor.ConstructDBEntityId(sessionId, rawLocationId);

            return GetById(db, entityId);
        }

        public List<ExploreLocationData> GetAllById(LiteDatabase db, string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return null;
            }

            try
            {
                var col = GetCollection<ExploreLocationData>(db, COLLECTION_NAME);

                return col.Find(ex => ex.SessionId == sessionId).ToList();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        #endregion
    }
}