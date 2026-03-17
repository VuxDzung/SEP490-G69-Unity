namespace SEP490G69.Training
{
    using LiteDB;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class TrainingExerciseDAO : BaseDAO
    {
        public const string COLLECTION_NAME = "SessionExerciseData";


        public TrainingExerciseDAO() { }

        public bool InsertTrainingExercise(SessionTrainingExercise trainingExercise)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionTrainingExercise> collection = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                    collection.Insert(trainingExercise);
                    return true;
                }

            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        public bool UpdateTrainingExercise(SessionTrainingExercise trainingExercise)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionTrainingExercise> collection = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                    collection.Update(trainingExercise);
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        public bool DeleteTrainingExercise(string id)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionTrainingExercise> collection = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                    collection.Delete(id);
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        public SessionTrainingExercise GetTrainingExercise(string id)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionTrainingExercise> collection = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                    return collection.FindById(id);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }

        public SessionTrainingExercise GetById(string sessionId, string exerciseId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionTrainingExercise> collection = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                    return collection.FindOne(ex => ex.SessionId.Equals(sessionId) && ex.ExerciseId.Equals(exerciseId));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }

        public List<SessionTrainingExercise> GetAllBySessionId(string sessionId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionTrainingExercise> collection = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                    return collection.Find(ex => ex.SessionId.Equals(sessionId)).ToList();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }

        public bool DeleteById(string entityId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionTrainingExercise> collection = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                    collection.Delete(entityId);
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        public bool DeleteAllBySessionId(string sessionId)
        {
            try
            {
                using (LiteDatabase db = LocalDBInitiator.GetDatabase())
                {
                    ILiteCollection<SessionTrainingExercise> collection = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                    collection.DeleteMany(x => x.SessionId.Equals(sessionId));
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
                    ILiteCollection<SessionTrainingExercise> collection = GetCollection<SessionTrainingExercise>(db, COLLECTION_NAME);
                    List<SessionTrainingExercise> exercises = collection.FindAll().ToList();
                    foreach (var exercise in exercises)
                    {
                        DeleteById(exercise.Id);
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