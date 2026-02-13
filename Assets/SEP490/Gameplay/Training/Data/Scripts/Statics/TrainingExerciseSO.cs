namespace SEP490G69
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "TrainingEx_", menuName = OrganizationConstants.NAMESPACE + "/Training Exercises/Exercise data")]
    public class TrainingExerciseSO : ScriptableObject
    {
        [SerializeField] private string exerciseId;
        [SerializeField] private string exerciseName;
        [SerializeField] private string exerciseDescription;
        [SerializeField] private Sprite exerciseIcon;

        public string ExerciseId => exerciseId;
        public string ExerciseName => exerciseName;
        public string ExerciseDescription => exerciseDescription;
        public Sprite ExerciseIcon => exerciseIcon;
    }
}