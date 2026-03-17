namespace SEP490G69
{
    using LiteDB;
    using UnityEngine;

    public class PlayerCharacterDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "PlayerCharacterData";

        public PlayerCharacterDAO() { }

        public SessionCharacterData GetCharacterById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionCharacterData> collection = GetCollection<SessionCharacterData>(db, COLLECTION_NAME);
                    return collection.FindById(id);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public SessionCharacterData GetCharacterById(string sessionId, string rawCharacterId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionCharacterData> collection = GetCollection<SessionCharacterData>(db, COLLECTION_NAME);
                    return collection.FindOne(x => x.SessionId.Equals(sessionId) &&
                                           x.RawCharacterId.Equals(rawCharacterId));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool TryCreateCharacter(SessionCharacterData characterData)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionCharacterData> collection = GetCollection<SessionCharacterData>(db, COLLECTION_NAME);
                    collection.Insert(characterData);
                    return true;
                }
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
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionCharacterData> collection = GetCollection<SessionCharacterData>(db, COLLECTION_NAME);
                    collection.Update(characterData);
                    return true;
                }
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
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionCharacterData> collection = GetCollection<SessionCharacterData>(db, COLLECTION_NAME);
                    collection.Delete(id);
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