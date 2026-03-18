namespace SEP490G69.Legacy
{
    using LiteDB;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GameLegacyDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "LegacyStats";

        public GameLegacyDAO() { }  

        public LegacyStatData GetById(string entityId)
        {
            try
            {
                LiteDatabase db = LocalDBInitiator.GetDatabase();
                var col = GetCollection<LegacyStatData>(db, COLLECTION_NAME);

                return col.FindById(entityId);
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public LegacyStatData GetById(string playerId, string rawLegacyId)
        {
            try
            {
                LiteDatabase db = LocalDBInitiator.GetDatabase();
                var col = GetCollection<LegacyStatData>(db, COLLECTION_NAME);

                return col.FindOne(l => l.PlayerId.Equals(playerId) && l.RawLegacyStatId.Equals(rawLegacyId));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool Insert(LegacyStatData data)
        {
            try
            {
                LiteDatabase db = LocalDBInitiator.GetDatabase();
                ILiteCollection<LegacyStatData> col = GetCollection<LegacyStatData>(db, COLLECTION_NAME);

                col.Insert(data);

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Update(LegacyStatData data)
        {
            try
            {
                LiteDatabase db = LocalDBInitiator.GetDatabase();
                ILiteCollection<LegacyStatData> col = GetCollection<LegacyStatData>(db, COLLECTION_NAME);

                col.Update(data);

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteById(string entityId)
        {
            try
            {
                LiteDatabase db = LocalDBInitiator.GetDatabase();
                ILiteCollection<LegacyStatData> col = GetCollection<LegacyStatData>(db, COLLECTION_NAME);

                col.Delete(entityId);

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