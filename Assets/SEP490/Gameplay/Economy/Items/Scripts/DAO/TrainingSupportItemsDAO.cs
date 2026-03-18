namespace SEP490G69
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class TrainingSupportItemsDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "TrainingSupportItems";

        public TrainingSupportItemsDAO() { }

        public TrainingSupportItem GetById(string entityId)
        {
            try
            {
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<TrainingSupportItem>(db, COLLECTION_NAME);

                    return col.FindById(entityId);
                }
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);  
                return null;
            }
        }

        public TrainingSupportItem GetById(string sessionId, string rawItemId)
        {
            try
            {
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<TrainingSupportItem>(db, COLLECTION_NAME);

                    return col.FindOne(itm => itm.SessionId.Equals(sessionId) && itm.RawItemId.Equals(rawItemId));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<TrainingSupportItem> GetAllBySessionId(string sessionId)
        {
            try
            {
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<TrainingSupportItem>(db, COLLECTION_NAME);

                    return col.Find(itm => itm.SessionId.Equals(sessionId)).ToList();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool Update(TrainingSupportItem item)
        {
            try
            {
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<TrainingSupportItem>(db, COLLECTION_NAME);

                    col.Update(item);   

                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Insert(TrainingSupportItem item)
        {
            try
            {
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<TrainingSupportItem>(db, COLLECTION_NAME);

                    col.Insert(item);

                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Delete(string entityId)
        {
            try
            {
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<TrainingSupportItem>(db, COLLECTION_NAME);

                    col.Delete(entityId);

                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteAllBySessionId(string sessionId)
        {
            try
            {
                using (var db = LocalDBInitiator.GetDatabase())
                {
                    var col = GetCollection<TrainingSupportItem>(db, COLLECTION_NAME);

                    col.DeleteMany(itm => itm.SessionId.Equals(sessionId));

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