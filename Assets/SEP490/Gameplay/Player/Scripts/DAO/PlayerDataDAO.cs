namespace SEP490G69
{
    using UnityEngine;
    using LiteDB;
    using System.Collections.Generic;
    using System;

    /// <summary>
    /// Document here: https://www.litedb.org/docs/getting-started/
    /// </summary>
    public class PlayerDataDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "Player";

        // =========================
        // AUTO MODE
        // =========================

        public bool Insert(PlayerData playerData)
        {
            try
            {
                return LocalDBInitiator.Execute(db => Insert(db, playerData));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(PlayerData playerData)
        {
            try
            {
                return LocalDBInitiator.Execute(db => Update(db, playerData));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(PlayerData playerData)
        {
            try
            {
                return LocalDBInitiator.Execute(db => Upsert(db, playerData));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public List<PlayerData> GetAll()
        {
            try
            {
                return LocalDBInitiator.Execute(db => GetAll(db));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<PlayerData>();
            }
        }

        public PlayerData GetById(string id)
        {
            try
            {
                return LocalDBInitiator.Execute(db => GetById(db, id));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool Delete(string id)
        {
            try
            {
                return LocalDBInitiator.Execute(db => Delete(db, id));
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

        public bool Insert(LiteDatabase db, PlayerData playerData)
        {
            try
            {
                if (playerData == null)
                    return false;

                var col = GetCollection<PlayerData>(db, COLLECTION_NAME);
                col.Insert(playerData);
                return true;
            }
            catch (LiteException e) when (e.ErrorCode == LiteException.INDEX_DUPLICATE_KEY)
            {
                Debug.LogWarning($"[PlayerDataDAO] Duplicate player: {playerData?.PlayerId}");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(LiteDatabase db, PlayerData playerData)
        {
            try
            {
                if (playerData == null)
                    return false;

                var col = GetCollection<PlayerData>(db, COLLECTION_NAME);
                return col.Update(playerData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(LiteDatabase db, PlayerData playerData)
        {
            try
            {
                if (playerData == null)
                    return false;

                var col = GetCollection<PlayerData>(db, COLLECTION_NAME);
                return col.Upsert(playerData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public List<PlayerData> GetAll(LiteDatabase db)
        {
            try
            {
                var col = GetCollection<PlayerData>(db, COLLECTION_NAME);
                return col.Query().ToList();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new List<PlayerData>();
            }
        }

        public PlayerData GetById(LiteDatabase db, string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return null;

                var col = GetCollection<PlayerData>(db, COLLECTION_NAME);
                return col.FindById(id);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool Delete(LiteDatabase db, string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return false;

                var col = GetCollection<PlayerData>(db, COLLECTION_NAME);
                return col.Delete(id);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}