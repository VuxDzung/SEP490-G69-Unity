namespace SEP490G69
{
    using LiteDB;
    using UnityEngine;

    public class PlayerCharacterDAO
    {
        public const string COLLECTION_NAME = "PlayerCharacterData";

        private LiteDatabase _database;
        private ILiteCollection<SessionCharacterData> _collection;

        public PlayerCharacterDAO()
        {
            _database = LocalDBInitiator.GetDatabase();
            _collection = _database.GetCollection<SessionCharacterData>(COLLECTION_NAME);
            _collection.EnsureIndex(x => x.SessionId);
            _collection.EnsureIndex(_ => _.RawCharacterId);
        }

        public PlayerCharacterDAO(LiteDatabase database)
        {
            _database = database;
            _collection = _database.GetCollection<SessionCharacterData>(COLLECTION_NAME);
            _collection.EnsureIndex(x => x.SessionId);
            _collection.EnsureIndex(_ => _.RawCharacterId);
        }

        public SessionCharacterData GetCharacterById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            try
            {
                return _collection.FindById(id);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }
        public SessionCharacterData GetCharacterById(string sessionId, string rawCharacterId)
        {
            try
            {
                return _collection.FindOne(x => x.SessionId.Equals(sessionId) && 
                                           x.RawCharacterId.Equals(rawCharacterId));
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }

        public bool TryCreateCharacter(SessionCharacterData characterData)
        {
            try
            {
                _collection.Insert(characterData);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool TryUpdateCharacter(SessionCharacterData characterData)
        {
            try
            {
                _collection.Update(characterData);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool TryDeleteCharacter(string id)
        {
            try
            {
                _collection.Delete(id);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}