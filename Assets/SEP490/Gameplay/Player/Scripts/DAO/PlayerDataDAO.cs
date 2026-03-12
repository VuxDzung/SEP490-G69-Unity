namespace SEP490G69
{
    using UnityEngine;
    using LiteDB;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Document here: https://www.litedb.org/docs/getting-started/
    /// </summary>
    public class PlayerDataDAO 
    {
        private LiteDatabase _database;

        public const string PLAYER_DATA_COLLECTION = "Player";
        private readonly ILiteCollection<PlayerData> _collection;

        public PlayerDataDAO(LiteDatabase database)
        {
            _database = database;
            _collection = _database.GetCollection<PlayerData>(PLAYER_DATA_COLLECTION);
        }

        public bool Insert(PlayerData playerData)
        {
            try
            {
                _collection.Insert(playerData);
                return true;
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
                return _collection.Update(playerData);
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
                List<PlayerData> result = _collection.Query().ToList();
                return result;
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
                return _collection.FindById(id);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }
    }
}