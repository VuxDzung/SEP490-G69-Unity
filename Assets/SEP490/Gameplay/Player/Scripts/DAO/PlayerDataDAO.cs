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

        public PlayerDataDAO(LiteDatabase database)
        {
            _database = database;
        }

        public void InsertNewPlayer(PlayerData playerData)
        {
            var col = _database.GetCollection<PlayerData>(PLAYER_DATA_COLLECTION);
            col.Insert(playerData);
        }

        public bool UpdatePlayer(PlayerData playerData)
        {
            var col = _database.GetCollection<PlayerData>(PLAYER_DATA_COLLECTION);
            return col.Update(playerData);
        }

        public List<PlayerData> GetPlayers()
        {
            var col = _database.GetCollection<PlayerData>(PLAYER_DATA_COLLECTION);
            List<PlayerData> result = col.Query().ToList();
            return result;
        }

        public PlayerData GetPlayerById(string id)
        {
            var col = _database.GetCollection<PlayerData>(PLAYER_DATA_COLLECTION);
            return col.FindById(id);
        }
    }
}