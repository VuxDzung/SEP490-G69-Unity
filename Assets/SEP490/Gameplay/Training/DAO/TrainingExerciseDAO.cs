namespace SEP490G69.Training
{
    using LiteDB;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class TrainingExerciseDAO 
    {
        public const string COLLECTION_NAME = "SessionExerciseData";

        private LiteDatabase _database;
        private ILiteCollection<SessionTrainingExercise> _collection;

        public TrainingExerciseDAO(LiteDatabase database)
        {
            _database = database;
            _collection = _database.GetCollection<SessionTrainingExercise>(COLLECTION_NAME);
        }

        public bool InsertTrainingExercise(SessionTrainingExercise trainingExercise)
        {
            try
            {
                _collection.Insert(trainingExercise);
                return true;
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
        public bool UpdateTrainingExercise(SessionTrainingExercise trainingExercise)
        {
            try
            {
                _collection.Update(trainingExercise);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool DeleteTrainingExercise(string id)
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

        public SessionTrainingExercise GetTrainingExercise(string id)
        {
            try
            {
                return _collection.FindById(id);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public SessionTrainingExercise GetById(string sessionId, string exerciseId)
        {
            try
            {
                return _collection.FindOne(ex => ex.SessionId.Equals(sessionId) && ex.ExerciseId.Equals(exerciseId));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<SessionTrainingExercise> GetAllBySessionId(string sessionId)
        {
            try
            {
                return _collection.Find(ex => ex.SessionId.Equals(sessionId)).ToList();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool DeleteById(string entityId)
        {
            try
            {

                _collection.Delete(entityId);
                return true;
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
                List<SessionTrainingExercise> exercises = GetAllBySessionId(sessionId);
                foreach (var exercise in exercises)
                {
                    DeleteById(exercise.Id);
                }
                return true;
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
                List<SessionTrainingExercise> exercises = _collection.FindAll().ToList();
                foreach (var exercise in exercises)
                {
                    DeleteById(exercise.Id);
                }
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