namespace SEP490G69.Economy
{
    using LiteDB;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    public class GameShopDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "SHOP";

        // =========================
        // AUTO MODE
        // =========================

        public List<ShopItemData> GetAll(string sessionId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetAll(db, sessionId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<ShopItemData>();
            }
        }

        public ShopItemData Get(string sessionId, string rawItemId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Get(db, sessionId, rawItemId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool InsertMany(List<ShopItemData> items)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => InsertMany(db, items));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Insert(ShopItemData data)
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

        public bool Upsert(ShopItemData data)
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

        public bool Update(ShopItemData data)
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

        public bool Delete(string sessionItemId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Delete(db, sessionItemId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteManyBySessionId(string sessionId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => DeleteManyBySessionId(db, sessionId));
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

        public List<ShopItemData> GetAll(LiteDatabase db, string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return new List<ShopItemData>();

                var col = GetCollection<ShopItemData>(db, COLLECTION_NAME);
                return col.Find(x => x.SessionId == sessionId).ToList();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<ShopItemData>();
            }
        }

        public ShopItemData Get(LiteDatabase db, string sessionId, string rawItemId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(rawItemId))
                    return null;

                var col = GetCollection<ShopItemData>(db, COLLECTION_NAME);
                string entityId = EntityIdConstructor.ConstructDBEntityId(sessionId, rawItemId);
                return col.FindById(entityId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        // --- CREATE ---

        public bool Insert(LiteDatabase db, ShopItemData data)
        {
            try
            {
                if (data == null)
                    return false;

                var col = GetCollection<ShopItemData>(db, COLLECTION_NAME);
                col.Insert(data);
                return true;
            }
            catch (LiteException e) when (e.ErrorCode == LiteException.INDEX_DUPLICATE_KEY)
            {
                Debug.LogWarning($"[GameShopDAO] Duplicate item: {data?.SessionItemId}");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool InsertMany(LiteDatabase db, List<ShopItemData> items)
        {
            try
            {
                if (items == null || items.Count == 0)
                    return true;

                var col = GetCollection<ShopItemData>(db, COLLECTION_NAME);
                int inserted = col.InsertBulk(items);
                return inserted == items.Count;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(LiteDatabase db, ShopItemData data)
        {
            try
            {
                if (data == null)
                    return false;

                var col = GetCollection<ShopItemData>(db, COLLECTION_NAME);
                return col.Upsert(data);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- UPDATE ---

        public bool Update(LiteDatabase db, ShopItemData data)
        {
            try
            {
                if (data == null)
                    return false;

                var col = GetCollection<ShopItemData>(db, COLLECTION_NAME);
                return col.Update(data);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- DELETE ---

        public bool Delete(LiteDatabase db, string sessionItemId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionItemId))
                    return false;

                var col = GetCollection<ShopItemData>(db, COLLECTION_NAME);
                return col.Delete(sessionItemId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteManyBySessionId(LiteDatabase db, string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return false;

                var col = GetCollection<ShopItemData>(db, COLLECTION_NAME);
                col.DeleteMany(x => x.SessionId == sessionId);
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