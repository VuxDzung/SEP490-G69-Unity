namespace SEP490G69
{
    using LiteDB;
    using UnityEngine;

    public class PlayerCharacterRepository 
    {
        public const string FORMAT_ID = "{0}:{1}";

        private PlayerCharacterDAO _dao;

        public PlayerCharacterRepository(LiteDatabase db)
        {
            _dao = new PlayerCharacterDAO(db);
        }

        public bool TryCreateNewCharacter(string sessionId, BaseCharacterSO characterSO)
        {
            SessionCharacterData characterData = new SessionCharacterData();

            string id = string.Format(FORMAT_ID, sessionId, characterSO.CharacterId);

            if (_dao.GetCharacter(id) != null )
            {
                Debug.Log($"Character {id} has already existed!");
                if (!_dao.TryDeleteCharacter(id))
                {
                    Debug.LogError($"Failed to delete character {id}");
                    return false;
                }
            }

            characterData.Id = id;
            characterData.CurrentMaxVitality = characterSO.BaseVit;
            characterData.CurrentPower = characterSO.BasePow;
            characterData.CurrentIntelligence = characterSO.BaseInt;
            characterData.CurrentStamina = characterSO.BaseSta;
            characterData.CurrentDef = characterSO.BaseDef;
            characterData.CurrentAgi = characterSO.BaseAgi;
            characterData.CurrentEnergy = characterSO.BaseEnergy;
            characterData.CurrentMood = characterSO.BaseMood;
            characterData.CurrentRP = 0;

            return _dao.TryCreateCharacter(characterData);
        }

        public SessionCharacterData GetCharacterData(string sessionId, string characterId)
        {
            try
            {
                string queryId = string.Format(FORMAT_ID, sessionId, characterId);
                return _dao.GetCharacter(queryId);
            }
            catch(System.Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }
    }
}