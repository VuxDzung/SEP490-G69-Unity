namespace SEP490G69.Tournament
{
    using LiteDB;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class TournamentProgressDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "tournament_progress";

        public TournamentProgressDAO() { }

        public TournamentProgressData GetById(string id)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<TournamentProgressData> collection = GetCollection<TournamentProgressData>(db, COLLECTION_NAME);
                    return collection.FindById(id);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public TournamentProgressData GetById(string sessionId, string rawTournamentId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<TournamentProgressData> collection = GetCollection<TournamentProgressData>(db, COLLECTION_NAME);
                    return collection.FindOne(x => x.SessionId.Equals(sessionId) &&
                                                x.RawTournamentId.Equals(rawTournamentId));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<TournamentProgressData> GetAllBySessionId(string sessionId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<TournamentProgressData> collection = GetCollection<TournamentProgressData>(db, COLLECTION_NAME);
                    return collection.Find(x => x.SessionId.Equals(sessionId)).ToList();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool Upsert(TournamentProgressData data)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<TournamentProgressData> collection = GetCollection<TournamentProgressData>(db, COLLECTION_NAME);
                    collection.Upsert(data);
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Delete(string id)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<TournamentProgressData> collection = GetCollection<TournamentProgressData>(db, COLLECTION_NAME);
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

        public bool DeleteAllBySessionId(string sessionId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<TournamentProgressData> collection = GetCollection<TournamentProgressData>(db, COLLECTION_NAME);
                    collection.DeleteMany(s => s.SessionId.Equals(sessionId));
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteAll()
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<TournamentProgressData> collection = GetCollection<TournamentProgressData>(db, COLLECTION_NAME);
                    List<TournamentProgressData> progresses = collection.FindAll().ToList();
                    if (progresses == null || progresses.Count == 0) return true;
                    foreach (var progress in progresses)
                    {
                        Delete(progress.Id);
                    }
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