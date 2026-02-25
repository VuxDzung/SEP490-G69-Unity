namespace SEP490G69
{
    using System.Collections.Generic;
    using Unity.VisualScripting.Antlr3.Runtime.Misc;
    using UnityEngine;

    [CreateAssetMenu(fileName = "TrainingEx_", menuName = OrganizationConstants.NAMESPACE + "/Training Exercises/Exercise data")]
    public class TrainingExerciseSO : ScriptableObject
    {
        [SerializeField] private string exerciseId;
        [SerializeField] private string exerciseName;
        [SerializeField] private string exerciseDescription;
        [SerializeField] private Sprite exerciseIcon;

        [SerializeField] private List<StatusModifierSO> m_SuccessModifiers;
        [SerializeField] private List<StatusModifierSO> m_FailedModifiers;

        public string ExerciseId => exerciseId;
        public string ExerciseName => exerciseName;
        public string ExerciseDescription => exerciseDescription;
        public Sprite ExerciseIcon => exerciseIcon;
        public List<StatusModifierSO> SuccessModifiers => m_SuccessModifiers;
        public List<StatusModifierSO> FailedModifiers => m_FailedModifiers;
    }
}