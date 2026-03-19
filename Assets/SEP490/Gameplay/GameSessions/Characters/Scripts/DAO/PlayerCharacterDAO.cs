namespace SEP490G69
{
    using LiteDB;
    using System;
    using UnityEngine;

    public class PlayerCharacterDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "PlayerCharacterData";

        // =========================
        // AUTO MODE
        // =========================

        public SessionCharacterData GetById(string id)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetById(db, id));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public SessionCharacterData GetById(string sessionId, string rawCharacterId)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => GetById(db, sessionId, rawCharacterId));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool Insert(SessionCharacterData characterData)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Insert(db, characterData));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(SessionCharacterData characterData)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Update(db, characterData));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(SessionCharacterData characterData)
        {
            try
            {
                return LocalDBOrchestrator.Execute(db => Upsert(db, characterData));
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

        // =========================
        // TRANSACTION MODE (CORE)
        // =========================

        public SessionCharacterData GetById(LiteDatabase db, string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return null;

                var col = GetCollection<SessionCharacterData>(db, COLLECTION_NAME);
                return col.FindById(id);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public SessionCharacterData GetById(LiteDatabase db, string sessionId, string rawCharacterId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(rawCharacterId))
                    return null;

                string entityId = EntityIdConstructor.ConstructDBEntityId(sessionId, rawCharacterId);

                var col = GetCollection<SessionCharacterData>(db, COLLECTION_NAME);
                return col.FindById(entityId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        // --- CREATE ---

        public bool Insert(LiteDatabase db, SessionCharacterData characterData)
        {
            try
            {
                if (characterData == null)
                    return false;

                var col = GetCollection<SessionCharacterData>(db, COLLECTION_NAME);
                col.Insert(characterData);
                return true;
            }
            catch (LiteException e) when (e.ErrorCode == LiteException.INDEX_DUPLICATE_KEY)
            {
                Debug.LogWarning($"[PlayerCharacterDAO] Duplicate character: {characterData?.Id}");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Upsert(LiteDatabase db, SessionCharacterData characterData)
        {
            try
            {
                if (characterData == null)
                    return false;

                var col = GetCollection<SessionCharacterData>(db, COLLECTION_NAME);
                return col.Upsert(characterData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- UPDATE ---

        public bool Update(LiteDatabase db, SessionCharacterData characterData)
        {
            try
            {
                if (characterData == null)
                    return false;

                var col = GetCollection<SessionCharacterData>(db, COLLECTION_NAME);
                return col.Update(characterData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        // --- DELETE ---

        public bool Delete(LiteDatabase db, string entityId)
        {
            try
            {
                if (string.IsNullOrEmpty(entityId))
                    return false;

                var col = GetCollection<SessionCharacterData>(db, COLLECTION_NAME);
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

                var col = GetCollection<SessionCharacterData>(db, COLLECTION_NAME);
                col.DeleteMany(c => c.SessionId == sessionId);
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