namespace SEP490G69.Training
{
    using LiteDB;
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

        public SessionTrainingExercise GetByIdAndSessionId(string sessionId, string exerciseId)
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
    }
}