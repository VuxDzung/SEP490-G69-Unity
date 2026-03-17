namespace SEP490G69.Economy
{
    using LiteDB;
    using System.Linq;
    using System.Collections.Generic;
    using SEP490G69.GameSessions;
    using UnityEngine;

    public class GameShopDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "SHOP";

        public GameShopDAO() { }

        public List<ShopItemData> GetAll(string sessionId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<ShopItemData> collection = GetCollection<ShopItemData>(db, COLLECTION_NAME);
                    List<ShopItemData> list = collection.Find(x => x.SessionId == sessionId).ToList();
                    return list;
                }

            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public ShopItemData Get(string sessionId, string rawItemId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<ShopItemData> collection = GetCollection<ShopItemData>(db, COLLECTION_NAME);
                    ShopItemData item = collection.FindOne(x =>x.SessionId == sessionId && x.RawItemId == rawItemId);
                    return item;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool Insert(ShopItemData data)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<ShopItemData> collection = GetCollection<ShopItemData>(db, COLLECTION_NAME);
                    collection.Insert(data);
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(ShopItemData data)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<ShopItemData> collection = GetCollection<ShopItemData>(db, COLLECTION_NAME);
                    collection.Update(data);
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Delete(string sessionItemId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<ShopItemData> collection = GetCollection<ShopItemData>(db, COLLECTION_NAME);
                    collection.Delete(sessionItemId);
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool ClearSession(string sessionId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<ShopItemData> collection = GetCollection<ShopItemData>(db, COLLECTION_NAME);
                    collection.DeleteMany(x => x.SessionId == sessionId);
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}