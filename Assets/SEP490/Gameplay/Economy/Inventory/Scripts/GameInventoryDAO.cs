namespace SEP490G69.Economy
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GameInventoryDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "INVENTORY";

        // =========================
        // AUTO MODE (SAFE WRAPPER)
        // =========================

        public ItemData GetItem(string sessionId, string rawItemId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetItem(db, sessionId, rawItemId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<ItemData> GetAllItems(string sessionId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetAllItems(db, sessionId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<ItemData>();
            }
        }

        public bool Insert(ItemData item)
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

        public bool InsertMany(List<ItemData> items)
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

        public bool Update(ItemData item)
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

        public bool Upsert(ItemData item)
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

        public EquipmentData GetEquippedRelic(string sessionId, int slot)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetEquippedRelic(db, sessionId, slot));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public EquipmentData GetRelic(string sessionId, string relicId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetRelic(db, sessionId, relicId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        // =========================
        // TRANSACTION MODE (CORE)
        // =========================

        public ItemData GetItem(LiteDatabase db, string sessionId, string rawItemId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(rawItemId))
                    return null;

                string entityId = EntityIdConstructor.ConstructDBEntityId(sessionId, rawItemId);
                var col = GetCollection<ItemData>(db, COLLECTION_NAME);
                return col.FindById(entityId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<ItemData> GetAllItems(LiteDatabase db, string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return new List<ItemData>();

                var col = GetCollection<ItemData>(db, COLLECTION_NAME);
                return col.Find(x => x.SessionId == sessionId).ToList();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<ItemData>();
            }
        }

        public bool Insert(LiteDatabase db, ItemData item)
        {
            try
            {
                if (item == null)
                    return false;

                var col = GetCollection<ItemData>(db, COLLECTION_NAME);
                return col.Insert(item);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool InsertMany(LiteDatabase db, List<ItemData> items)
        {
            try
            {
                if (items == null || items.Count == 0)
                    return true;

                var col = GetCollection<ItemData>(db, COLLECTION_NAME);
                int inserted = col.InsertBulk(items);
                return inserted == items.Count;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(LiteDatabase db, ItemData item)
        {
            try
            {
                if (item == null)
                    return false;

                var col = GetCollection<ItemData>(db, COLLECTION_NAME);
                return col.Update(item);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(LiteDatabase db, ItemData item)
        {
            try
            {
                if (item == null)
                    return false;

                var col = GetCollection<ItemData>(db, COLLECTION_NAME);
                return col.Upsert(item);
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

                var col = GetCollection<ItemData>(db, COLLECTION_NAME);
                return col.Delete(entityId);
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

                var col = GetCollection<ItemData>(db, COLLECTION_NAME);
                col.DeleteMany(x => x.SessionId == sessionId);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public EquipmentData GetEquippedRelic(LiteDatabase db, string sessionId, int slot)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return null;

                var col = GetCollection<ItemData>(db, COLLECTION_NAME);

                return col.Find(x => x.SessionId == sessionId)
                          .OfType<EquipmentData>()
                          .FirstOrDefault(x => x.Slot == slot);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public EquipmentData GetRelic(LiteDatabase db, string sessionId, string relicId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(relicId))
                    return null;

                var col = GetCollection<ItemData>(db, COLLECTION_NAME);

                return col.Find(x => x.SessionId == sessionId && x.RawItemId == relicId)
                          .OfType<EquipmentData>()
                          .FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }
    }
}