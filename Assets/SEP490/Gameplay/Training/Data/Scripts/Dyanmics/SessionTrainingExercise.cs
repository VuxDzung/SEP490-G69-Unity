using LiteDB;

namespace SEP490G69
{
    public class SessionTrainingExercise 
    {
        [BsonId]
        public string Id { get; set; }

        public string ExerciseId {  get; set; }
        public string SessionId { get; set; }
        public int Level { get; set; }
    }
}