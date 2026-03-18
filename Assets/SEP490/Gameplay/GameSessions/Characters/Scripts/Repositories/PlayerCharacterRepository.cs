namespace SEP490G69
{
    using LiteDB;
    using UnityEditor.U2D.Animation;
    using UnityEngine;

    public class PlayerCharacterRepository 
    {
        public const string FORMAT_ID = "{0}:{1}";

        private PlayerCharacterDAO _dao;

        public PlayerCharacterRepository()
        {
            _dao = new PlayerCharacterDAO();
        }

        public bool Insert(SessionCharacterData characterData)
        {
            return _dao.Insert(characterData);
        }

        public bool Insert(LiteDatabase db, SessionCharacterData characterData)
        {
            return _dao.Insert(db, characterData);
        }

        public bool Upsert(SessionCharacterData characterData)
        {
            return _dao.Upsert(characterData);
        }

        public bool Upsert(LiteDatabase db, SessionCharacterData characterData)
        {
            return _dao.Upsert(db, characterData);
        }

        public bool Update(SessionCharacterData characterData)
        {
            return _dao.Update(characterData);
        }

        public bool Update(LiteDatabase db, SessionCharacterData characterData)
        {
            return _dao.Update(db, characterData);
        }

        public bool Delete(string entityId)
        {
            return _dao.Delete(entityId);
        }

        public bool Delete(LiteDatabase db, string entityId)
        {
            return _dao.Delete(db, entityId);
        }

        public bool DeleteManyBySessionId(string entityId)
        {
            return _dao.DeleteManyBySessionId(entityId);
        }

        public bool DeleteManyBySessionId(LiteDatabase db, string entityId)
        {
            return _dao.DeleteManyBySessionId(db, entityId);
        }

        public bool TryCreateNewCharacter(string sessionId, BaseCharacterSO characterSO, BonusStarterStats bonusStarterStats)
        {
            SessionCharacterData characterData = new SessionCharacterData();

            string id = string.Format(FORMAT_ID, sessionId, characterSO.CharacterId);

            if (_dao.GetById(id) != null )
            {
                Debug.Log($"Character {id} has already existed!");
                if (!_dao.Delete(id))
                {
                    Debug.LogError($"Failed to delete character {id}");
                    return false;
                }
            }

            characterData.Id = id;
            characterData.CurrentMaxVitality = characterSO.BaseVit + bonusStarterStats.BonusVit;
            characterData.CurrentPower = characterSO.BasePow + bonusStarterStats.BonusPow;
            characterData.CurrentIntelligence = characterSO.BaseInt + bonusStarterStats.BonusInt;
            characterData.CurrentStamina = characterSO.BaseSta + bonusStarterStats.BonusSta;
            characterData.CurrentDef = characterSO.BaseDef + bonusStarterStats.BonusDef;
            characterData.CurrentAgi = characterSO.BaseAgi + bonusStarterStats.BonusAgi;
            characterData.CurrentEnergy = characterSO.BaseEnergy;
            characterData.CurrentMood = characterSO.BaseMood;
            characterData.CurrentRP = characterSO.BaseRP;

            return _dao.Insert(characterData);
        }

        public SessionCharacterData GetCharacterData(string sessionId, string characterId)
        {
            try
            {
                string queryId = string.Format(FORMAT_ID, sessionId, characterId);
                return _dao.GetById(queryId);
            }
            catch(System.Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }
    }

    public class BonusStarterStats
    {
        public int BonusVit { get; set; }
        public int BonusPow { get; set; }
        public int BonusInt { get; set; }
        public int BonusSta { get; set; }
        public int BonusDef { get; set; }
        public int BonusAgi { get; set; }
    }
}