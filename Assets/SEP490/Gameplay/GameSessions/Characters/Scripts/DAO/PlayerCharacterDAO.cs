namespace SEP490G69
{
    using LiteDB;
    using UnityEngine;

    public class PlayerCharacterDAO
    {
        public const string COLLECTION_NAME = "PlayerCharacterData";

        private LiteDatabase _database;
        private ILiteCollection<SessionCharacterData> _collection;

        public PlayerCharacterDAO(LiteDatabase database)
        {
            _database = database;
            _collection = _database.GetCollection<SessionCharacterData>(COLLECTION_NAME);
        }

        public SessionCharacterData GetCharacter(string id)
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

        public bool TryCreateCharacter(SessionCharacterData characterData)
        {
            try
            {
                _collection.Insert(characterData);
                return true;
            }
            catch (System.Exception e)
            {
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
                return false;
            }
        }
    }
}