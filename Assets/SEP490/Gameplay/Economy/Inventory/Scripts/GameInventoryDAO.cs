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

        public GameInventoryDAO() { }

        public ItemData GetItem(string sessionId, string rawItemId)
        {
            try
            {
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<ItemData>(db, COLLECTION_NAME);
                    return col.FindOne(x => x.SessionId == sessionId && x.RawItemId == rawItemId);
                }
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
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<ItemData>(db, COLLECTION_NAME);
                    return col.Find(x => x.SessionId == sessionId).ToList();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool Insert(ItemData item)
        {
            try
            {
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<ItemData>(db, COLLECTION_NAME);
                    col.Insert(item);
                    return true;
                }
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
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<ItemData>(db, COLLECTION_NAME);
                    col.Update(item);
                    return true;
                }
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
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<ItemData>(db, COLLECTION_NAME);
                    col.Delete(sessionItemId);
                    return true;
                }
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
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<ItemData>(db, COLLECTION_NAME);
                    return col.Find(x => x.SessionId == sessionId)
                              .OfType<EquipmentData>()
                              .FirstOrDefault(x => x.Slot == slot);
                }
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
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<ItemData>(db, COLLECTION_NAME);
                    return col.Find(x => x.SessionId == sessionId && x.RawItemId == relicId)
                              .OfType<EquipmentData>()
                              .FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }
    }
}