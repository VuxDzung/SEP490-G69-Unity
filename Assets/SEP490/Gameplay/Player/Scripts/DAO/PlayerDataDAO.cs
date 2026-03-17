namespace SEP490G69
{
    using UnityEngine;
    using LiteDB;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Document here: https://www.litedb.org/docs/getting-started/
    /// </summary>
    public class PlayerDataDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "Player";

        public PlayerDataDAO() { }

        public bool Insert(PlayerData playerData)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<PlayerData> collection = GetCollection<PlayerData>(db, COLLECTION_NAME);
                    collection.Insert(playerData);
                    return true;
                }
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(PlayerData playerData)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<PlayerData> collection = GetCollection<PlayerData>(db, COLLECTION_NAME);
                    return collection.Update(playerData);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public List<PlayerData> GetAll()
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<PlayerData> collection = GetCollection<PlayerData>(db, COLLECTION_NAME);
                    List<PlayerData> result = collection.Query().ToList();
                    return result;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public PlayerData GetById(string id)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<PlayerData> collection = GetCollection<PlayerData>(db, COLLECTION_NAME);
                    return collection.FindById(id);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }
    }
}