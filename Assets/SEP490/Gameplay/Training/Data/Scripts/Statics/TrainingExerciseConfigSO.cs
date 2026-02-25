namespace SEP490G69.Training
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "TrainingExerciseConfig", menuName = OrganizationConstants.NAMESPACE + "/Training Exercises/Exercises config")]
    public class TrainingExerciseConfigSO : ScriptableObject
    {
        [SerializeField] private List<TrainingExerciseSO> m_Exercises;

        public TrainingExerciseSO[] Exercises => m_Exercises.ToArray();

        public TrainingExerciseSO GetExercise(string exerciseId)
        {
            if (m_Exercises.Count == 0) return null;
            return m_Exercises.FirstOrDefault(ex => ex.ExerciseId.Equals(exerciseId));
        }
    }
}