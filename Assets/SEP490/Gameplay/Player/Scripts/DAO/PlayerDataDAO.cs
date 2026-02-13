namespace SEP490G69
{
    using UnityEngine;
    using LiteDB;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Document here: https://www.litedb.org/docs/getting-started/
    /// </summary>
    public class PlayerDataDAO : MonoBehaviour
    {
        private LiteDatabase _database;

        public const string PLAYER_DATA_COLLECTION = "Player";

        private void Start()
        {
            _database = new LiteDatabase(Application.persistentDataPath + "/database/GameData.db");
        }

        public void InsertNewPlayer(PlayerData playerData)
        {
            var col = _database.GetCollection<PlayerData>(PLAYER_DATA_COLLECTION);
            col.Insert(playerData);
        }

        public void UpdatePlayer(PlayerData playerData)
        {
            var col = _database.GetCollection<PlayerData>(PLAYER_DATA_COLLECTION);
            col.Update(playerData);
        }

        public List<PlayerData> GetPlayers()
        {
            var col = _database.GetCollection<PlayerData>(PLAYER_DATA_COLLECTION);
            List<PlayerData> result = col.Query().ToList();
            return result;
        }

        public PlayerData GetPlayerById(int id)
        {
            var col = _database.GetCollection<PlayerData>(PLAYER_DATA_COLLECTION);
            PlayerData player = col.Query().ToList().FirstOrDefault(p => p.PlayerId == id);
            return player;
        }
    }
}